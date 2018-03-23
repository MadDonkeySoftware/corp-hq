// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;

    using Common.Data;
    using MongoDB.Driver;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    internal class CreateMongoIndexes : Job<object>
    {
        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            var dbFactory = new DbFactory();
            this.CreateRunnersIndexes(dbFactory);
        }

        private void CreateRunnersIndexes(IDbFactory dbFactory)
        {
            Console.WriteLine("Starting to apply indexes");
            var runnerCol = dbFactory.GetCollection<dynamic>("corp-hq", CollectionNames.Runners);
            runnerCol.Indexes.CreateOne(
                Builders<dynamic>.IndexKeys.Ascending("expireAt"),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) });
            Console.WriteLine("Finished applying indexes");
        }
    }
}