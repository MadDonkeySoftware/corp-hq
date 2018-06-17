// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;
    using Common.Model;

    /// <summary>
    /// The class for monitoring RabbitMQ queues
    /// </summary>
    public class DefaultAdjudicator : IAdjudicator
    {
        /// <summary>
        /// Checks to see if the job should skip processing.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if processing should be skipped, false otherwise.</returns>
        public bool SkipProcessing(JobSpec<string> job)
        {
            return false;
        }

        /// <summary>
        /// Checks to see if the job should be split up.
        /// </summary>
        /// <param name="job">The job to verify</param>
        /// <returns>True if the job should be split up, false otherwise.</returns>
        public bool RequiresSplit(JobSpec<string> job)
        {
            return false;
        }

        /// <summary>
        /// Splits the provided job up into smaller child jobs
        /// </summary>
        /// <param name="job">The job to split.</param>
        /// <returns>Null as this should never be called.</returns>
        public SplitResult Split(JobSpec<string> job)
        {
            return null;
        }
    }
}