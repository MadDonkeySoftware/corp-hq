// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;

    /// <summary>
    /// A class to hold information common to all objects stored in the mongo database.
    /// </summary>
    public abstract class MongoBase
    {
        /// <summary>
        /// Gets or sets underlying mongo _id.
        /// </summary>
        /// <remarks>
        /// It should never be explicitly set. This is a public get / set since I haven't spent the time
        /// to figure out the proper way to fix it.
        /// </remarks>
        [JsonIgnore]
        [BsonId]
        public ObjectId BaseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [BsonElement("expireAt")]
        public DateTime? ExpireAt { get; set; }
    }
}