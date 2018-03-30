// Copyright (c) MadDonkeySoftware

namespace Runner.Model
{
    using System;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a job runner within the system.
    /// </summary>
    public class TaskRunner : MongoBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [BsonElement("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}