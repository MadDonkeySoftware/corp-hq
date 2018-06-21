// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Mongo
{
    using System;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository responsible for job retrieval and persistance
    /// </summary>
    public class JobMessageRepository : BaseRepository, IJobMessageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobMessageRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public JobMessageRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        /// <summary>
        /// Adds a message to the associated job at the specified level.
        /// </summary>
        /// <param name="jobSpec">The job spec.</param>
        /// <param name="level">The message level.</param>
        /// <param name="message">The message.</param>
        public void AddMessageToJob(JobSpecLite jobSpec, JobMessageLevel level, string message)
        {
            InternalAddMessageToJob(this.DbFactory, jobSpec, level, message);
        }

        internal static void InternalAddMessageToJob(IDbFactory dbFactory, JobSpecLite jobSpec, JobMessageLevel level, string message)
        {
            // TODO: Make Job Message expiry configurable via the settings db.
            var expiry = DateTime.Now.AddDays(3);
            var messageCollection = dbFactory.GetCollection<JobMessage>(CollectionNames.JobMessages);
            messageCollection.InsertOneAsync(new JobMessage
            {
                JobUuid = jobSpec.Uuid,
                MasterJobUuid = jobSpec.MasterUuid,
                ExpireAt = expiry,
                Timestamp = DateTime.Now,
                Level = (ushort)level,
                Message = message
            });
        }
    }
}