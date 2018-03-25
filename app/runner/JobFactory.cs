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
        /// <summary>
        /// Creates a new IJob instance for the appropriate job type.
        /// </summary>
        /// <param name="key">The job type to initialize the job for.</param>
        /// <param name="data">And data needing to be supplied to the job.</param>
        /// <returns>An instance of a job ready to be started.</returns>
        public static IJob AcquireJob(string key, dynamic data)
        {
            IJob<dynamic> job;
            switch (key)
            {
                case JobTypes.ApplyDbIndexes:
                    job = new CreateMongoIndexes();
                    break;
                case JobTypes.ImportMapData:
                    job = new ImportMapData();
                    break;
                default:
                    job = null;
                    break;
            }

            if (job != null)
            {
                job.Data = data;
            }

            return (IJob)job;
        }
    }
}