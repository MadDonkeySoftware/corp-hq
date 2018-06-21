// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Mongo
{
    using System;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using Common.Model.Eve;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository responsible for job retrieval and persistance
    /// </summary>
    public class MarketOrderRepository : BaseRepository, IMarketOrderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrderRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public MarketOrderRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        /// <summary>
        /// Saves a market order to the datastore by either inserting or updating.
        /// </summary>
        /// <param name="order">The order to persist to the data store.</param>
        public void Save(MarketOrder order)
        {
            var marketOrderCol = this.DbFactory.GetCollection<MarketOrder>(CollectionNames.MarketOrders);

            var filterCondition = Builders<MarketOrder>.Filter.Eq(r => r.OrderId, order.OrderId);
            var updateCondition = Builders<MarketOrder>.Update.Set(r => r.TypeId, order.TypeId)
                                                                .Set(r => r.LocationId, order.LocationId)
                                                                .Set(r => r.VolumeTotal, order.VolumeTotal)
                                                                .Set(r => r.VolumeRemain, order.VolumeRemain)
                                                                .Set(r => r.MinVolume, order.MinVolume)
                                                                .Set(r => r.Price, order.Price)
                                                                .Set(r => r.IsBuyOrder, order.IsBuyOrder)
                                                                .Set(r => r.Duration, order.Duration)
                                                                .Set(r => r.Issued, order.Issued)
                                                                .Set(r => r.Range, order.Range)
                                                                .Set(r => r.ExpireAt, order.Issued.AddDays(order.Duration));

            marketOrderCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = true });
        }
    }
}