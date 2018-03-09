// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A sample class
    /// </summary>
    public class Sample : MongoBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [BsonElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [BsonElement("value")]
        public string Value { get; set; }
    }
}