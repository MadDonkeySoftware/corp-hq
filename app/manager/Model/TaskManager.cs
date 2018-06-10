// Copyright (c) MadDonkeySoftware

namespace Manager.Model
{
    using System;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a job runner within the system.
    /// </summary>
    public class TaskManager : MongoBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }
    }
}