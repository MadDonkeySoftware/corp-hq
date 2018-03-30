// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
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

        private readonly string jobUuid;

        private IMongoCollection<JobMessage> messageCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="jobUuid">The job uuid this is running for.</param>
        public Job(string jobUuid)
        {
            this.jobUuid = jobUuid;
            this.Messages = new List<string>();
        }

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
                this.Initialize();
                this.Work();
                this.Conclude();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Responsible for job initialization
        /// </summary>
        protected internal virtual void Initialize()
        {
        }

        /// <summary>
        /// Responsible for job finalization
        /// </summary>
        protected internal virtual void Conclude()
        {
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
                JobUuid = this.jobUuid,
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