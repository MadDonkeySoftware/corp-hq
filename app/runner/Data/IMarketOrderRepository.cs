// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    /// <summary>
    /// Repository responsible for map information retrieval and persistance
    /// </summary>
    public interface IMarketOrderRepository
    {
        /// <summary>
        /// Saves a market order to the datastore by either inserting or updating.
        /// </summary>
        /// <param name="order">The order to persist to the data store.</param>
        void Save(MarketOrder order);
    }
}