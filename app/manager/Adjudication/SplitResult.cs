// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;
    using System.Collections.Generic;
    using Common.Model;
    using Common.Model.JobData;

    /// <summary>
    /// The class containing relivant information for job splitting.
    /// </summary>
    public class SplitResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitResult"/> class.
        /// </summary>
        public SplitResult()
        {
            this.ChildJobs = new List<JobSpec<string>>();
        }

        /// <summary>
        /// Gets the list of new child jobs.
        /// </summary>
        /// <returns>The list of new child jobs.</returns>
        public List<JobSpec<string>> ChildJobs { get; private set; }
    }
}