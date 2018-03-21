// Copyright (c) MadDonkeySoftware


namespace Common.Model.RabbitMQ
{
    using MongoDB.Bson.Serialization.Attributes;

    public class Host : MongoBase
    {
        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("address")]
        public string Address { get; set; }
    }
}