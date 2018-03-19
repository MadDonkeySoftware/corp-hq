// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Threading;
    using Common.Data;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
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
        private IModel channel;
        private IConnection connection;
        private bool isDisposed = false; // To detect redundant calls
        private EventingBasicConsumer controlConsumer;
        private EventingBasicConsumer jobsConsumer;
        private string controlQueueName;

        internal Monitor(IConnectionFactory connectionFactory)
        {
            this.connection = connectionFactory.CreateConnection();
            this.channel = this.connection.CreateModel();
        }

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
                        instance = new Monitor(Monitor.connectionFactory);
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
        public static void Initialize(IConnectionFactory connectionFactory, IDbFactory dbFactory)
        {
            Monitor.connectionFactory = connectionFactory;
            Monitor.dbFactory = dbFactory;
        }

        /// <summary>
        /// Starts this monitor
        /// </summary>
        public void Start()
        {
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
            var col = dbFactory.GetCollection<TaskRunner>("corp-hq", "runners");
            col.InsertOne(new TaskRunner { Name = this.controlQueueName });

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
                }

                // Clean up any unmanaged resources here.
                this.isDisposed = true;
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

            var col = dbFactory.GetCollection<TaskRunner>("corp-hq", "runners");
            col.FindOneAndDelete(r => r.Name == this.controlQueueName);
            this.Dispose();
        }

        private void HandleJobMessage(object sender, BasicDeliverEventArgs e)
        {
            var uuid = new Guid(e.Body);
            Console.WriteLine(" [x] Received {0}", uuid.ToString());

            /* TODO: Actual job processing logic here. */

            this.channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            Console.WriteLine(" [x] Completed {0}", uuid.ToString());
        }
    }
}