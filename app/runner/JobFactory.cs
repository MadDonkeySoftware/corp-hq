// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Data;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Runner.Jobs;
    using Runner.Model;

    /// <summary>
    /// The class responsible for finding the correct job and returning it
    /// </summary>
    public static class JobFactory
    {
        private static readonly IDbFactory DbFactory = new DbFactory();

        /// <summary>
        /// Creates a new IJob instance for the appropriate job type.
        /// </summary>
        /// <param name="jobSpec">The job specification to initialize the job for.</param>
        /// <param name="dbFactory">An option DbFactory to use with the new job.</param>
        /// <returns>An instance of a job ready to be started.</returns>
        public static IJob AcquireJob(JobSpecLite jobSpec, IDbFactory dbFactory = null)
        {
            IJob job;
            switch (jobSpec.Type)
            {
                case JobTypes.ApplyDbIndexes:
                    job = new CreateMongoIndexes(jobSpec.Uuid, dbFactory ?? DbFactory);
                    break;
                case JobTypes.ImportMapData:
                    job = new ImportMapData(jobSpec.Uuid, dbFactory ?? DbFactory);
                    break;
                case JobTypes.ImportMarketData:
                    job = new ImportMarketData(jobSpec.Uuid, dbFactory ?? DbFactory);
                    break;
                default:
                    job = null;
                    break;
            }

            return (IJob)job;
        }
    }
}