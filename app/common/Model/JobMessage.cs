// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    using System;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>
    public class JobMessage : MongoBase
    {
        /// <summary>
        /// Gets or sets the job uuid.
        /// </summary>
        [BsonElement("jobUuid")]
        public string JobUuid { get; set; }

        /// <summary>
        /// Gets or sets the type of job.
        /// </summary>
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [BsonElement("message")]
        public string Message { get; set; }
    }
}