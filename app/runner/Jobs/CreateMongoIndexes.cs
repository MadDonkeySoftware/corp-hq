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
            this.AddMessage("Starting to apply indexes");
            this.CreateRunnersIndexes();
            this.CreateJobsIndexes();
            this.CreateJobMessagesIndexes();
            this.CreateSessionIndexes();
            this.AddMessage("Finished applying indexes");
        }

        private void CreateRunnersIndexes()
        {
            var col = DbFactory.GetCollection<dynamic>(CollectionNames.Runners);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobsIndexes()
        {
            var col = DbFactory.GetCollection<dynamic>(CollectionNames.Jobs);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateJobMessagesIndexes()
        {
            var col = DbFactory.GetCollection<dynamic>(CollectionNames.JobMessages);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateMarketOrdersIndexes()
        {
            var col = DbFactory.GetCollection<dynamic>(CollectionNames.MarketOrders);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }

        private void CreateSessionIndexes()
        {
            var col = DbFactory.GetCollection<dynamic>(CollectionNames.Sessions);
            col.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }
    }
}