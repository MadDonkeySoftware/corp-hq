// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;
    using Common.Model;

    /// <summary>
    /// The class for monitoring RabbitMQ queues
    /// </summary>
    public interface IAdjudicator
    {
        /// <summary>
        /// Checks to see if the job should skip processing.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if processing should be skipped, false otherwise.</returns>
        bool SkipProcessing(JobSpec<string> job);

        /// <summary>
        /// Checks to see if the job should be split up.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if the job should be split up, false otherwise.</returns>
        bool RequiresSplit(JobSpec<string> job);

        /// <summary>
        /// Splits the provided job up into smaller child jobs
        /// </summary>
        /// <param name="job">The job to split.</param>
        /// <returns>The results of the split operation.</returns>
        SplitResult Split(JobSpec<string> job);
    }
}