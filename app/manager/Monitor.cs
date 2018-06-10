// Copyright (c) MadDonkeySoftware

namespace Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Data;
    using Common.Model;
    using Common.Model.JobData;
    using Manager.Adjudication;
    using Manager.Model;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;

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
        private string controlQueueName;

        internal Monitor()
        {
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
            var settingsCollection = dbFactory.GetCollection<Common.Model.Setting<Common.Model.RabbitMQ.Root>>(CollectionNames.Settings);
            var settings = settingsCollection.AsQueryable().Where(s => s.Key == "rabbitConnection").First().Value;

            this.ConnectToMessageQueue(settings);

            this.channel = this.connection.CreateModel();
            this.channel.BasicQos(0, 1, false);

            // Get queue name for runner-specific purposes.
            this.controlQueueName = this.channel.QueueDeclare(
                queue: $"manager-{Guid.NewGuid()}").QueueName;

            // Create job queue if it doesn't exist
            this.channel.QueueDeclare(
                queue: "jobs",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Setup heartbeat
            var t = Task.Run(() =>
            {
                while (!this.isDisposed)
                {
                    // Since we want to do a partial update we have to use the builder
                    this.ProcessNewJobs();
                    ProcessQueuedJobs();
                    ProcessRunningJobs();
                    Thread.Sleep(1000 * 5);
                }
            });

            // create consumers
            this.controlConsumer = new EventingBasicConsumer(this.channel);

            // Set up the control queue
            this.controlConsumer.Received += this.HandleControlMessage;
            this.channel.BasicConsume(
                queue: this.controlQueueName,
                consumer: this.controlConsumer);

            Console.WriteLine("{0} is now processing jobs.", this.controlQueueName);
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

        private static void ProcessQueuedJobs()
        {
            try
            {
                var jobsCollection = dbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
                var newJobs = jobsCollection.AsQueryable().Where(j => j.Status == JobStatuses.Queued).ToList();

                foreach (var job in newJobs)
                {
                    var newStatus = JobStatuses.Queued;
                    var exitLoop = false;
                    foreach (var childId in job.Children)
                    {
                        var status = jobsCollection.AsQueryable().First(j => j.Uuid == childId).Status;
                        switch (status)
                        {
                            case JobStatuses.Successful:
                            case JobStatuses.Running:
                            case JobStatuses.Failed:
                                // Let the running logic update time stamps appropriately.
                                newStatus = JobStatuses.Running;
                                exitLoop = true;
                                break;
                        }

                        if (exitLoop)
                        {
                            break;
                        }
                    }

                    if (job.Children.Count > 0 && newStatus != JobStatuses.Queued)
                    {
                        var filterCondition = Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, job.Uuid);
                        var updateCondition = Builders<JobSpec<string>>.Update.Set(j => j.Status, newStatus);

                        switch (newStatus)
                        {
                            case JobStatuses.Failed:
                                updateCondition.Set(j => j.EndTimestamp, DateTime.Now);
                                break;
                            case JobStatuses.Running:
                                updateCondition.Set(j => j.StartTimestamp, DateTime.Now);
                                break;
                        }

                        jobsCollection.UpdateOne(filterCondition, updateCondition);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine("{0}", ex.StackTrace);
            }
        }

        private static void ProcessRunningJobs()
        {
            try
            {
                var jobsCollection = dbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
                var newJobs = jobsCollection.AsQueryable().Where(j => j.Status == JobStatuses.Running).ToList();

                foreach (var job in newJobs)
                {
                    var newStatus = JobStatuses.Successful;
                    var exitLoop = false;
                    foreach (var childId in job.Children)
                    {
                        var status = jobsCollection.AsQueryable().First(j => j.Uuid == childId).Status;
                        switch (status)
                        {
                            case JobStatuses.New:
                            case JobStatuses.Queued:
                            case JobStatuses.Running:
                                newStatus = JobStatuses.Running;
                                exitLoop = true;
                                break;
                            case JobStatuses.Failed:
                                newStatus = JobStatuses.Failed;
                                exitLoop = true;
                                break;
                        }

                        if (exitLoop)
                        {
                            break;
                        }
                    }

                    if (job.Children.Count > 0 && newStatus != JobStatuses.Running)
                    {
                        // Find the start and end time stamps based on the sub jobs.
                        var timestamps = jobsCollection.AsQueryable().Where(j => j.ParentUuid == job.Uuid)
                            .Select(j => new { j.StartTimestamp, j.EndTimestamp }).ToList();
                        var start = timestamps.Where(j => j.StartTimestamp.HasValue).OrderBy(j => j.StartTimestamp).First().StartTimestamp;
                        var end = timestamps.Where(j => j.EndTimestamp.HasValue).OrderByDescending(j => j.EndTimestamp).First().EndTimestamp;

                        var filterCondition = Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, job.Uuid);
                        var updateCondition = Builders<JobSpec<string>>.Update.Set(j => j.Status, newStatus)
                                                                              .Set(j => j.StartTimestamp, start)
                                                                              .Set(j => j.EndTimestamp, end);

                        jobsCollection.UpdateOne(filterCondition, updateCondition);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine("{0}", ex.StackTrace);
            }
        }

        private void QueueJob(IMongoCollection<JobSpec<string>> jobsCollection, JobSpec<string> job)
        {
            var filterCondition = Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, job.Uuid);
            var updateCondition = Builders<JobSpec<string>>.Update.Set(j => j.Status, JobStatuses.Queued);
            jobsCollection.UpdateOne(filterCondition, updateCondition);

            try
            {
                this.channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "jobs",
                    basicProperties: null,
                    body: Guid.Parse(job.Uuid).ToByteArray());
            }
            catch (Exception)
            {
                // TODO: Figure out what exception to use here.
                updateCondition = Builders<JobSpec<string>>.Update.Set(j => j.Status, JobStatuses.New);
                jobsCollection.UpdateOne(filterCondition, updateCondition);
            }
        }

        private void HandleControlMessage(object sender, BasicDeliverEventArgs e)
        {
            // TODO: Handle actual control messages here.
            Console.WriteLine(" Exit Received!");

            // Give time for any in-flight jobs to finish if they are near completion.
            for (var i = 5; i > 0; i--)
            {
                Console.WriteLine(" Exiting in {0}...", i);
                Thread.Sleep(1000);
            }

            this.Dispose();
        }

        private void ProcessNewJobs()
        {
            try
            {
                var jobsCollection = dbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);
                var newJobs = jobsCollection.AsQueryable().Where(j => j.Status == JobStatuses.New).ToList();

                foreach (var job in newJobs)
                {
                    var adjudicator = AdjudicatorFactory.Get(job.Type);

                    if (adjudicator.SkipProcessing(job))
                    {
                        continue;
                    }

                    if (adjudicator.RequiresSplit(job))
                    {
                        var result = adjudicator.Split(job);

                        var childJobIds = result.ChildJobs.Select(j => j.Uuid).ToList();
                        job.Children.AddRange(childJobIds);

                        jobsCollection.InsertMany(result.ChildJobs);
                        jobsCollection.UpdateOne(
                            Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, job.Uuid),
                            Builders<JobSpec<string>>.Update.Set(j => j.Children, childJobIds));
                    }
                    else
                    {
                        this.QueueJob(jobsCollection, job);
                        if (!string.IsNullOrEmpty(job.ParentUuid) && newJobs.Count(j => j.Uuid == job.ParentUuid) > 0)
                        {
                            jobsCollection.UpdateOne(
                                Builders<JobSpec<string>>.Filter.Eq(j => j.Uuid, job.ParentUuid),
                                Builders<JobSpec<string>>.Update.Set(j => j.Status, JobStatuses.Queued));

                            newJobs.First(j => j.Uuid == job.ParentUuid).Status = JobStatuses.Queued;
                        }
                    }
                }
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