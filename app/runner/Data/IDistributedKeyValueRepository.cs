// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;

    /// <summary>
    /// Repository responsible for handling distributed key value storage.
    /// </summary>
    public interface IDistributedKeyValueRepository
    {
        /// <summary>
        /// Sets a string value in the distributed key value store.
        /// </summary>
        /// <param name="key">The key for the data to store.</param>
        /// <param name="value">The data to store.</param>
        /// <param name="expiry">The TTL for the record. If no value is provided a default is applied.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        bool Persist(string key, string value, TimeSpan? expiry = null);

        /// <summary>
        /// Gets a string value from the distributed key value store.
        /// </summary>
        /// <param name="key">The key for the data requested.</param>
        /// <returns>The data requested if available, null otherwise.</returns>
        string Retrieve(string key);
    }
}