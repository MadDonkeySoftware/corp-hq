// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Redis
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;
    using StackExchange.Redis;

    /// <summary>
    /// Repository responsible for handling distributed key value storage.
    /// </summary>
    public class DistributedKeyValueRepository : IDistributedKeyValueRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedKeyValueRepository"/> class.
        /// </summary>
        /// <param name="connectionMultiplexer">The redis connection multiplexer.</param>
        public DistributedKeyValueRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            this.ConnectionMultiplexer = connectionMultiplexer;
        }

        private IConnectionMultiplexer ConnectionMultiplexer { get; set; }

        /// <summary>
        /// Sets a string value in the distributed key value store.
        /// </summary>
        /// <param name="key">The key for the data to store.</param>
        /// <param name="value">The data to store.</param>
        /// <param name="expiry">The TTL for the record. If no value is provided a default is applied.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool Persist(string key, string value, TimeSpan? expiry = null)
        {
            if (!expiry.HasValue)
            {
                expiry = TimeSpan.FromHours(1);
            }

            var db = this.ConnectionMultiplexer.GetDatabase();
            return db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// Gets a string value from the distributed key value store.
        /// </summary>
        /// <param name="key">The key for the data requested.</param>
        /// <returns>The data requested if available, null otherwise.</returns>
        public string Retrieve(string key)
        {
            var db = this.ConnectionMultiplexer.GetDatabase();
            var value = db.StringGet(key);
            return value.HasValue ? value.ToString() : null;
        }
    }
}