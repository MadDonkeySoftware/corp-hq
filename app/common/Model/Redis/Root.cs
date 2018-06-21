// Copyright (c) MadDonkeySoftware

namespace Common.Model.Redis
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
    }
}