// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>
    public class JobSpecLite : MongoBase
    {
        /// <summary>
        /// Gets or sets the uuid.
        /// </summary>
        [BsonElement("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the type of job.
        /// </summary>
        [BsonElement("type")]
        public string Type { get; set; }
    }
}