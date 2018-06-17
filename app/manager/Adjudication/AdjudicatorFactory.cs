// Copyright (c) MadDonkeySoftware

namespace Manager.Adjudication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The class for monitoring RabbitMQ queues
    /// </summary>
    public static class AdjudicatorFactory
    {
        private static List<Type> types;

        private static List<Type> Types
        {
            get
            {
                if (types == null)
                {
                    types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "Manager.Adjudication").ToList();
                }

                return types;
            }
        }

        /// <summary>
        /// Creates and returns a adjudicator instance for provided job type.
        /// </summary>
        /// <param name="jobType">The job type.</param>
        /// <returns>A instance of an adjudicator.</returns>
        public static IAdjudicator Get(string jobType)
        {
            foreach (var adjudicatorType in Types)
            {
                var attr = adjudicatorType.GetCustomAttributes(typeof(AdjudicatorAttribute), true).FirstOrDefault() as AdjudicatorAttribute;
                if (attr != null && attr.JobType == jobType)
                {
                    return Activator.CreateInstance(adjudicatorType) as IAdjudicator;
                }
            }

            return new DefaultAdjudicator();
        }
    }
}
