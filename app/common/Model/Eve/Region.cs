// Copyright (c) MadDonkeySoftware

namespace Common.Model.Eve
{
    using System.Collections.Generic;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A sample class
    /// </summary>
    public class Region : MongoBase
    {
        /// <summary>
        /// Gets or sets the region id.
        /// </summary>
        [BsonElement("regionId")]
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the associated constellations.
        /// </summary>
        [BsonElement("constellationIds")]
        public List<int> ConstellationIds { get; set; }
    }
}