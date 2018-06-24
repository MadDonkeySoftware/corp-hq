// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Data;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using RedLockNet.SERedis;
    using Runner.Data;
    using Runner.Jobs;
    using Runner.Model;

    /// <summary>
    /// The class for monitoring RabbitMQ queues
    /// </summary>
    public class Monitor : IDisposable
    {
        private static readonly object Padlock = new object();
        private static Monitor instance = null;
        private static IConnectionFactory connectionFactory;
        private static IDbFactory dbFactory;
        private static RedLockFactory redLockFactory;
        private IModel channel;
        private IConnection connection;
        private bool isDisposed = false; // To detect redundant calls
        private EventingBasicConsumer controlConsumer;
        private EventingBasicConsumer jobsConsumer;
        private string controlQueueName;

        internal Monitor()
        {
        }

        internal event EventHandler Disposed;

        /// <summary>
        /// Gets the monitor instance.
        /// </summary>
        public static Monitor Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (instance == null)
                    {
                        instance = new Monitor();
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="dbFactory">The database factory</param>
        /// <param name="redLockFactory">The redis lock factory</param>
        public static void Initialize(IConnectionFactory connectionFactory, IDbFactory dbFactory, RedLockFactory redLockFactory)
        {
            Monitor.connectionFactory = connectionFactory;
            Monitor.dbFactory = dbFactory;
            Monitor.redLockFactory = redLockFactory;
        }

        /// <summary>
        /// Starts this monitor
        /// </summary>
        public void Start()
        {
            var settingRepo = (ISettingRepository)Bootstrap.ServiceProvider.GetService(typeof(ISettingRepository));
            var rabbitSettings = settingRepo.FetchSetting<Common.Model.RabbitMQ.Root>("rabbitConnection");

            this.ConnectToMessageQueue(rabbitSettings);

            this.channel = this.connection.CreateModel();
            this.channel.BasicQos(0, 1, false);

            // Get queue name for runner-specific purposes.
            this.controlQueueName = this.channel.QueueDeclare().QueueName;

            // Create job queue if it doesn't exist
            this.channel.QueueDeclare(
                queue: "jobs",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Add ourself to the runner collection
            var col = dbFactory.GetCollection<TaskRunner>(CollectionNames.Runners);
            col.InsertOne(new TaskRunner { Name = this.controlQueueName, ExpireAt = DateTime.Now.AddMinutes(rabbitSettings.RecordTtl) });

            // Setup heartbeat
            var t = Task.Run(() =>
            {
                while (!this.isDisposed)
                {
                    // Since we want to do a partial update we have to use the builder
                    col.UpdateOne(
                        r => r.Name == this.controlQueueName,
                        Builders<TaskRunner>.Update.Set("expireAt", DateTime.Now.AddMinutes(rabbitSettings.RecordTtl)));

                    Thread.Sleep(1000 * 60 * rabbitSettings.RecordHeartbeatInterval);
                }
            });

            // create consumers
            this.controlConsumer = new EventingBasicConsumer(this.channel);
            this.jobsConsumer = new EventingBasicConsumer(this.channel);

            // Set up the control queue
            this.controlConsumer.Received += this.HandleControlMessage;
            this.channel.BasicConsume(
                queue: this.controlQueueName,
                consumer: this.controlConsumer);

            // Setup the Jobs listener
            this.jobsConsumer.Received += this.HandleJobMessage;
            this.channel.BasicConsume(
                queue: "jobs",
                autoAck: false,
                consumer: this.jobsConsumer);

            Console.WriteLine("{0} is now accepting messages.", this.controlQueueName);
        }

        /// <summary>
        /// Disposes of managed and unmanaged resources referenced by this class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of managed and unmanaged resources referenced by this class.
        /// </summary>
        /// <param name="disposing">True to disposing managed and unmanaged resources. False to only cleanup unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Clean up managed resources here
                    this.connection.Dispose();
                    this.channel.Dispose();

                    // HACK: Need to move this.
                    Monitor.redLockFactory.Dispose();
                }

                // Clean up any unmanaged resources here.
                this.isDisposed = true;
                if (this.Disposed != null)
                {
                    this.Disposed(this, new EventArgs());
                }
            }
        }

        private void HandleControlMessage(object sender, BasicDeliverEventArgs e)
        {
            // TODO: Handle actual control messages here.
            Console.WriteLine(" Exit Received!");
            this.channel.BasicCancel(this.jobsConsumer.ConsumerTag);

            // Give time for any in-flight jobs to finish if they are near completion.
            for (var i = 5; i > 0; i--)
            {
                Console.WriteLine(" Exiting in {0}...", i);
                Thread.Sleep(1000);
            }

            var col = dbFactory.GetCollection<TaskRunner>(CollectionNames.Runners);
            col.FindOneAndDelete(r => r.Name == this.controlQueueName);
            this.Dispose();
        }

        private void HandleJobMessage(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var uuid = new Guid(e.Body).ToString();
                Console.WriteLine(" [x] Received  {0}", uuid);

                /* TODO: Actual job processing logic here. */
                var col = dbFactory.GetCollection<JobSpecLite>(CollectionNames.Jobs);

                var jobSpec = col.AsQueryable().Where(j => j.Uuid == uuid)
                                               .FirstOrDefault();

                var job = JobFactory.AcquireJob(jobSpec);
                if (job != null)
                {
                    job.Start();
                }

                this.channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                Console.WriteLine(" [x] Completed {0}", uuid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine("{0}", ex.StackTrace);
            }
        }

        private void ConnectToMessageQueue(Common.Model.RabbitMQ.Root settings)
        {
            connectionFactory.UserName = settings.Username;
            connectionFactory.Password = settings.Password;

            var timeout = DateTime.Now.AddSeconds(30);
            do
            {
                try
                {
                    this.connection = connectionFactory.CreateConnection(settings.Hosts.Select(h => h.Address).ToList());
                }
                catch (BrokerUnreachableException)
                {
                }
            }
            while (DateTime.Now < timeout && this.connection == null);

            if (this.connection == null)
            {
                throw new TimeoutException("Could not connect to RabbitMQ instance.");
            }
        }
    }
}