// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;

    using Common.Data;
    using Common.Model;
    using MongoDB.Driver;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public class CreateMongoIndexes : Job
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMongoIndexes"/> class.
        /// </summary>
        /// <param name="jobSpec">The job specification this is running for.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public CreateMongoIndexes(JobSpecLite jobSpec, IDbFactory dbFactory)
            : base(jobSpec, dbFactory)
        {
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            this.AddMessage("Starting to apply indexes");
            this.CreateRunnersIndexes();
            this.CreateJobsIndexes();
            this.CreateJobMessagesIndexes();
            this.CreateSessionIndexes();
            this.AddMessage("Finished applying indexes");
        }

        private void CreateRunnersIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.Runners);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobsIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.Jobs);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobMessagesIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.JobMessages);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateMarketOrdersIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.MarketOrders);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateSessionIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.Sessions);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }
    }
}