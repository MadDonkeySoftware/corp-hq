// Copyright (c) MadDonkeySoftware


namespace Common.Model.RabbitMQ
{
    using System.Collections.Generic;
    using MongoDB.Bson.Serialization.Attributes;

    public class Root : MongoBase
    {
        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("hosts")]
        public List<Host> Hosts { get; set; }

        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [BsonElement("password")]
        public string Password { get; set; }
    }
}