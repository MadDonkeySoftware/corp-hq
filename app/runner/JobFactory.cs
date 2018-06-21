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
    using RedLockNet.SERedis;
    using Runner.Jobs;
    using Runner.Model;

    /// <summary>
    /// The class responsible for finding the correct job and returning it
    /// </summary>
    public static class JobFactory
    {
        /// <summary>
        /// Creates a new IJob instance for the appropriate job type.
        /// </summary>
        /// <param name="jobSpec">The job specification to initialize the job for.</param>
        /// <returns>An instance of a job ready to be started.</returns>
        public static IJob AcquireJob(JobSpecLite jobSpec)
        {
            Job job;
            switch (jobSpec.Type)
            {
                case JobTypes.ApplyDbIndexes:
                    job = (Job)Bootstrap.ServiceProvider.GetService(typeof(CreateMongoIndexes));
                    break;
                case JobTypes.ImportMapData:
                    job = (Job)Bootstrap.ServiceProvider.GetService(typeof(ImportMapData));
                    break;
                case JobTypes.ImportMarketData:
                    job = (Job)Bootstrap.ServiceProvider.GetService(typeof(ImportMarketData));
                    break;
                default:
                    job = null;
                    break;
            }

            job.JobSpec = jobSpec;
            return job;
        }
    }
}