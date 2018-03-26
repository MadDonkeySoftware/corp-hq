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
    public class JobSpec<T> : JobSpecLite where T : class
    {
        /// <summary>
        /// Gets or sets the start timestamp for the job.
        /// </summary>
        [BsonElement("startTimestamp")]
        public DateTime? StartTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the start timestamp for the job.
        /// </summary>
        [BsonElement("endTimestamp")]
        public DateTime? EndTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the data for the job.
        /// </summary>
        [BsonElement("arguments")]
        public T Data { get; set; }
    }
}