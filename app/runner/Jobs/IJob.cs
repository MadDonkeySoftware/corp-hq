// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using Common.Model;

    /// <summary>
    /// Contract for a job in this system.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets the internal job specification for this job.
        /// </summary>
        /// <returns>The job UUID.</returns>
        JobSpecLite JobSpec { get; }

        /// <summary>
        /// Starts the job processing code.
        /// </summary>
        void Start();
    }

    /// <summary>
    /// Contract for a job in this system.
    /// </summary>
    /// <typeparam name="T">The type of data used by this job.</typeparam>
    public interface IJob<T> : IJob
        where T : class
    {
        /// <summary>
        /// Gets or sets the data that is associated with this job.
        /// </summary>
        T Data { get; set; }
    }
}