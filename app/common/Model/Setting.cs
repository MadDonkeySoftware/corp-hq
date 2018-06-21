// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A sample class
    /// </summary>
    public class Setting<T> : MongoBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [BsonElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [BsonElement("environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [BsonElement("value")]
        public T Value { get; set; }
    }
}