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
        /// Gets or sets the authentication user name.
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the authentication password.
        /// </summary>
        [BsonElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the runner record expiration time in minutes.
        /// </summary>
        [BsonElement("recordTtl")]
        public int RecordTtl { get; set; }

        /// <summary>
        /// Gets or sets the runner record refresh interval in minutes.
        /// </summary>
        [BsonElement("recordHeartbeat")]
        public int RecordHeartbeatInterval { get; set; }
    }
}