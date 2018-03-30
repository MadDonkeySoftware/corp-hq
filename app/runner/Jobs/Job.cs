// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Base Job class
    /// </summary>
    /// <typeparam name="T">The type of data used by this job.</typeparam>
    internal abstract class Job<T> : IJob<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Job{T}"/> class.
        /// </summary>
        public Job()
        {
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Sets the data associated with this job
        /// </summary>
        public T Data { private get; set; }

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
        /// <param name="message">The message to associate with the job.</param>
        protected internal void AddMessage(string message)
        {
            // TODO: Eventually these messages should be stored in our data store.
            this.Messages.Add(message);
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected abstract void Work();
    }
}