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
    public abstract class Job : IJob
    {
        private IMongoCollection<JobMessage> messageCollection;
        private IMongoCollection<JobSpec<dynamic>> jobSpecCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="jobUuid">The job uuid this is running for.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public Job(string jobUuid, IDbFactory dbFactory)
        {
            this.JobUuid = jobUuid;
            this.DbFactory = dbFactory;
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Gets the internal job UUID for this job.
        /// </summary>
        /// <returns>The job UUID.</returns>
        protected internal string JobUuid { get; private set; }

        /// <summary>
        /// Gets the configured db factory.
        /// </summary>
        /// <returns>The db factory.</returns>
        protected IDbFactory DbFactory { get; private set; }

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
                this.messageCollection = this.DbFactory.GetCollection<JobMessage>(CollectionNames.JobMessages);
                this.jobSpecCollection = this.DbFactory.GetCollection<JobSpec<dynamic>>(CollectionNames.Jobs);
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

    /// <summary>
    /// Base Job class
    /// </summary>
    /// <typeparam name="T">The type for the Data property.</typeparam>y
    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleType",
        Justification = "No way to resolve this issue as rules confict with one another.")]
    public abstract class Job<T> : Job, IJob<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Job{T}"/> class.
        /// </summary>
        /// <param name="jobUuid">The job uuid this is running for.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public Job(string jobUuid, IDbFactory dbFactory)
            : base(jobUuid, dbFactory)
        {
        }

        /// <summary>
        /// Gets or sets the data associated with this job.
        /// </summary>
        public T Data { get; set; }
    }
}