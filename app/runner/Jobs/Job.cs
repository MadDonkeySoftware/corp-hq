// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Common;
    using Common.Data;
    using Common.Model;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    /// <summary>
    /// Base Job class
    /// </summary>
    internal abstract class Job : IJob
    {
        protected static readonly DbFactory DbFactory = new DbFactory();

        private IMongoCollection<JobMessage> messageCollection;
        private IMongoCollection<JobSpec<dynamic>> jobSpecCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="jobUuid">The job uuid this is running for.</param>
        public Job(string jobUuid)
        {
            this.JobUuid = jobUuid;
            this.Messages = new List<string>();
        }

        protected internal string JobUuid { get; private set; }

        /// <summary>
        /// Gets the list of all messages associated with this job.
        /// </summary>
        protected List<string> Messages { get; private set; }

        /// <summary>
        /// Starts the job processing code.
        /// </summary>
        public void Start()
        {
            try
            {
                this.messageCollection = DbFactory.GetCollection<JobMessage>("corp-hq", CollectionNames.JobMessages);
                this.jobSpecCollection = DbFactory.GetCollection<JobSpec<dynamic>>("corp-hq", CollectionNames.Jobs);
                this.Initialize();
                this.Work();
                this.Conclude();
            }
            catch (Exception ex)
            {
                this.jobSpecCollection.UpdateOne(
                    Builders<JobSpec<dynamic>>.Filter.Eq(j => j.Uuid, this.JobUuid),
                    Builders<JobSpec<dynamic>>.Update.Set(j => j.Status, JobStatuses.Failed).Set(j => j.EndTimestamp, DateTime.Now));
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Responsible for job initialization
        /// </summary>
        protected internal virtual void Initialize()
        {
            this.jobSpecCollection.UpdateOne(
                Builders<JobSpec<dynamic>>.Filter.Eq(j => j.Uuid, this.JobUuid),
                Builders<JobSpec<dynamic>>.Update.Set(j => j.Status, JobStatuses.Running).Set(j => j.StartTimestamp, DateTime.Now));
        }

        /// <summary>
        /// Responsible for job finalization
        /// </summary>
        protected internal virtual void Conclude()
        {
            this.jobSpecCollection.UpdateOne(
                Builders<JobSpec<dynamic>>.Filter.Eq(j => j.Uuid, this.JobUuid),
                Builders<JobSpec<dynamic>>.Update.Set(j => j.Status, JobStatuses.Successful).Set(j => j.EndTimestamp, DateTime.Now));
        }

        /// <summary>
        /// Associates a message with this job.
        /// </summary>
        /// <param name="format">The message format to associate with the job.</param>
        /// <param name="args">The message args to associate with the job.</param>
        protected internal void AddMessage(string format, params object[] args)
        {
            // TODO: Make Job Message expirary configurable via the settings db.
            var message = string.Format(CultureInfo.CurrentCulture, format, args);
            Console.WriteLine(message, args);
            this.Messages.Add(message);
            this.messageCollection.InsertOneAsync(new JobMessage
            {
                JobUuid = this.JobUuid,
                ExpireAt = DateTime.Now.AddDays(3),
                Timestamp = DateTime.Now,
                Message = message
            });
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected abstract void Work();
    }

    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleType",
        Justification = "No way to resolve this issue as rules confict with one another.")]
    internal abstract class Job<T> : Job, IJob<T>
        where T : class
    {
        public Job(string jobUuid)
            : base(jobUuid)
        {
        }

        /// <summary>
        /// Gets or sets the data associated with this job.
        /// </summary>
        public T Data { get; set; }
    }
}