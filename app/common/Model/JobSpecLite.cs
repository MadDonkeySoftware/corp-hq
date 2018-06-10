// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class JobSpecLite : MongoBase
    {
        /// <summary>
        /// Gets or sets the uuid.
        /// </summary>
        [BsonElement("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the parent job uuid.
        /// </summary>
        [BsonElement("parentUuid")]
        public string ParentUuid { get; set; }

        /// <summary>
        /// Gets or sets the top-most parent job uuid.
        /// </summary>
        [BsonElement("masterUuid")]
        public string MasterUuid { get; set; }

        /// <summary>
        /// Gets or sets the type of job.
        /// </summary>
        [BsonElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the status of job.
        /// </summary>
        [BsonElement("status")]
        public string Status { get; set; }
    }
}