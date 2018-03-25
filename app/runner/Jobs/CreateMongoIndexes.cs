// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;

    using Common.Data;
    using MongoDB.Driver;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    internal class CreateMongoIndexes : Job
    {
        public CreateMongoIndexes(string jobUuid)
            : base(jobUuid)
        {
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            var dbFactory = new DbFactory();
            this.AddMessage("Starting to apply indexes");
            this.CreateRunnersIndexes();
            this.CreateJobsIndexes();
            this.CreateJobMessagesIndexes();
            this.AddMessage("Finished applying indexes");
        }

        private void CreateRunnersIndexes()
        {
            var runnerCol = DbFactory.GetCollection<dynamic>("corp-hq", CollectionNames.Runners);
            runnerCol.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobsIndexes()
        {
            var jobsCol = DbFactory.GetCollection<dynamic>("corp-hq", CollectionNames.Jobs);
            jobsCol.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobMessagesIndexes()
        {
            var jobsCol = DbFactory.GetCollection<dynamic>("corp-hq", CollectionNames.JobMessages);
            jobsCol.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }
    }
}