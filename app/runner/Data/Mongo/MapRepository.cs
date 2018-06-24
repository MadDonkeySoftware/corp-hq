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
    public class MapRepository : BaseRepository, IMapRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public MapRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        /// <summary>
        /// Saves a region to the datastore by either inserting or updating.
        /// </summary>
        /// <param name="region">The region to persist to the data store.</param>
        public void SaveRegion(Region region)
        {
            var regionCol = this.DbFactory.GetCollection<Region>(CollectionNames.Regions);
            var filterCondition = Builders<Region>.Filter.Eq(r => r.RegionId, region.RegionId);
            var updateCondition = Builders<Region>.Update.Set(r => r.RegionId, region.RegionId)
                                                            .Set(r => r.Name, region.Name)
                                                            .Set(r => r.ConstellationIds, region.ConstellationIds);

            regionCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = true });
        }
    }
}