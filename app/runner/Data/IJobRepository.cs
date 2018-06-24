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
    public interface IJobRepository
    {
        /// <summary>
        /// Gets the job data associated with the job.
        /// </summary>
        /// <param name="jobUuid">The unique identifier for the job.</param>
        /// <typeparam name="T">The type to deserialize the job data to.</typeparam>
        /// <returns>A object representing the job data.</returns>
        T GetJobData<T>(string jobUuid);

        /// <summary>
        /// Adds a message to the associated job at the specified level.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="level">The message level.</param>
        /// <param name="message">The message.</param>
        void AddMessageToJob(JobSpecLite jobSpec, JobMessageLevel level, string message);

        /// <summary>
        /// Updates the job status and starting timestamp.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="status">The new status for the job.</param>
        /// <param name="timestamp">The new timestamp for the job's start.</param>
        void UpdateStatusAndStartTimestamp(JobSpecLite jobSpec, string status, DateTime timestamp);

        /// <summary>
        /// Updates the job status and end timestamp.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="status">The new status for the job.</param>
        /// <param name="timestamp">The new timestamp for the job's start.</param>
        void UpdateStatusAndEndTimestamp(JobSpecLite jobSpec, string status, DateTime timestamp);
    }
}