
// Copyright (c) MadDonkeySoftware

namespace Common.Model.JobData
{
    using System;
    using System.Collections.Generic;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>
    public class MarketDataImport
    {
        /// <summary>
        /// Gets or sets the region id.
        /// </summary>
        [JsonProperty("regionId")]
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the market type ids to fetch data for.
        /// </summary>
        [JsonProperty("marketIds")]
        public List<int> MarketTypeIds { get; set; }
    }
}