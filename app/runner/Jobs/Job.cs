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
    using Runner.Data;

    /// <summary>
    /// Base Job class
    /// </summary>
    public abstract class Job : IJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        public Job(IJobRepository jobRepository)
        {
            this.JobRepository = jobRepository;
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Gets the internal job specification for this job.
        /// </summary>
        /// <returns>The job UUID.</returns>
        public JobSpecLite JobSpec { get; internal set; }

        /// <summary>
        /// Gets the configured job repository.
        /// </summary>
        /// <returns>The job repository.</returns>
        protected IJobRepository JobRepository { get; private set; }

        /// <summary>
        /// Gets the list of all messages associated with this job.
        /// </summary>
        protected List<string> Messages { get; private set; }

        /// <summary>
        /// Starts the job processing code.
        /// </summary>
        public void Start()
        {
            if (this.JobSpec == null)
            {
                throw new NullReferenceException("JobSpec");
            }

            try
            {
                this.Initialize();
                this.Work();
                this.Conclude();
            }
            catch (Exception ex)
            {
                this.JobRepository.UpdateStatusAndEndTimestamp(this.JobSpec, JobStatuses.Failed, DateTime.Now);
                this.DumpException(ex);
            }
        }

        /// <summary>
        /// Responsible for job initialization
        /// </summary>
        protected internal virtual void Initialize()
        {
            this.JobRepository.UpdateStatusAndStartTimestamp(this.JobSpec, JobStatuses.Running, DateTime.Now);
        }

        /// <summary>
        /// Responsible for job finalization
        /// </summary>
        protected internal virtual void Conclude()
        {
            this.JobRepository.UpdateStatusAndEndTimestamp(this.JobSpec, JobStatuses.Successful, DateTime.Now);
        }

        /// <summary>
        /// Associates a message with this job.
        /// </summary>
        /// <param name="level">The log level with which to associate the message.</param>
        /// <param name="format">The message format to associate with the job.</param>
        /// <param name="args">The message args to associate with the job.</param>
        protected internal void AddMessage(JobMessageLevel level, string format, params object[] args)
        {
            // TODO: Make Job Message expiry configurable via the settings db.
            var message = string.Format(CultureInfo.CurrentCulture, format, args);
            Console.WriteLine(message, args);
            this.Messages.Add(message);
            this.JobRepository.AddMessageToJob(this.JobSpec, level, message);
        }

        /// <summary>
        /// Associates a message with this job.
        /// </summary>
        /// <param name="format">The message format to associate with the job.</param>
        /// <param name="args">The message args to associate with the job.</param>
        protected internal void AddMessage(string format, params object[] args)
        {
            this.AddMessage(JobMessageLevel.Info, format, args);
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected abstract void Work();

        private void DumpException(Exception ex, int level = 1)
        {
            const string divider = "==================";
            if (level == 1)
            {
                Console.WriteLine(divider);
            }
            else
            {
                Console.WriteLine();
            }

            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                this.DumpException(ex.InnerException, ++level);
            }

            if (level == 1)
            {
                Console.WriteLine(divider);
            }
        }
    }

    /// <summary>
    /// Base Job class
    /// </summary>
    /// <typeparam name="T">The type for the Data property.</typeparam>y
    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleType",
        Justification = "No way to resolve this issue as rules conflict with one another.")]
    public abstract class Job<T> : Job, IJob<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Job{T}"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        public Job(IJobRepository jobRepository)
            : base(jobRepository)
        {
        }

        /// <summary>
        /// Gets or sets the data associated with this job.
        /// </summary>
        public T Data { get; set; }
    }
}