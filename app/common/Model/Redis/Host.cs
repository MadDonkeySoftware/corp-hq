// Copyright (c) MadDonkeySoftware


namespace Common.Model.Redis
{
    using MongoDB.Bson.Serialization.Attributes;

    public class Host : MongoBase
    {
        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("port")]
        public int Port { get; set; }
    }
}