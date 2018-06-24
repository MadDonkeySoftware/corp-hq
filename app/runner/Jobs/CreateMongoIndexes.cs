// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;

    using Common.Data;
    using Common.Model;
    using MongoDB.Driver;
    using Runner.Data;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public class CreateMongoIndexes : Job
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMongoIndexes"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public CreateMongoIndexes(IJobRepository jobRepository, IDbFactory dbFactory)
            : base(jobRepository)
        {
            this.DbFactory = dbFactory;
        }

        private IDbFactory DbFactory { get; set; }

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
            this.CreateExpireAtIndex<dynamic>(col);
        }

        private void CreateJobsIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.Jobs);
            this.CreateExpireAtIndex<dynamic>(col);
        }

        private void CreateJobMessagesIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.JobMessages);
            this.CreateExpireAtIndex<dynamic>(col);
        }

        private void CreateMarketOrdersIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.MarketOrders);
            this.CreateExpireAtIndex<dynamic>(col);
        }

        private void CreateSessionIndexes()
        {
            var col = this.DbFactory.GetCollection<dynamic>(CollectionNames.Sessions);
            this.CreateExpireAtIndex<dynamic>(col);
        }

        private void CreateExpireAtIndex<T>(IMongoCollection<T> collection)
        {
            collection.Indexes.CreateOne(
                Builders<T>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
        }
    }
}