// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;

    /// <summary>
    /// Attribute to specify what job type a class adjudicates for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AdjudicatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjudicatorAttribute"/> class.
        /// </summary>
        /// <param name="jobType">The job type this class will adjudicate for.</param>
        public AdjudicatorAttribute(string jobType)
        {
            this.JobType = jobType;
        }

        /// <summary>
        /// Gets the job type this class adjudicates for.
        /// </summary>
        /// <returns>A string representing the job type adjudication is for.</returns>
        public string JobType { get; private set; }
    }
}