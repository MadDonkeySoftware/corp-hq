// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A sample class
    /// </summary>
    public class UserSession : MongoBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [BsonElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [BsonElement("endpoint")]
        public string Endpoint { get; set; }
    }
}