// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    /// <summary>
    /// Repository responsible for job retrieval and persistance
    /// </summary>
    public interface IJobMessageRepository
    {
        /// <summary>
        /// Adds a message to the associated job at the specified level.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="level">The message level.</param>
        /// <param name="message">The message.</param>
        void AddMessageToJob(JobSpecLite jobSpec, JobMessageLevel level, string message);
    }
}