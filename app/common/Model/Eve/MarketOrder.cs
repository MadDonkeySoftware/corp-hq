// Copyright (c) MadDonkeySoftware

namespace Common.Model.Eve
{
    using System;
    using System.Collections.Generic;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A sample class
    /// </summary>
    public class MarketOrder : MongoBase
    {
        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        [BsonElement("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// Gets or sets the type id the order is for.
        /// </summary>
        [BsonElement("typeId")]
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the location id of where the order originated.
        /// </summary>
        [BsonElement("locationId")]
        public long LocationId { get; set; }

        /// <summary>
        /// Gets or sets the total volume for the buy / sell order.
        /// </summary>
        [BsonElement("volumeTotal")]
        public int VolumeTotal { get; set; }

        /// <summary>
        /// Gets or sets the total volume remaining for the buy / sell order.
        /// </summary>
        [BsonElement("volumeRemain")]
        public int VolumeRemain { get; set; }

        /// <summary>
        /// Gets or sets the minimum volume allowed per transaction.
        /// </summary>
        [BsonElement("minVolume")]
        public int MinVolume { get; set; }

        /// <summary>
        /// Gets or sets the price per unit for the buy / sell order.
        /// </summary>
        /// <returns></returns>
        [BsonElement("price")]
        public double Price { get; set; }

        /// <summary>
        /// <c>True</c> if this is a buy order; false otherwise.
        /// </summary>
        /// <returns></returns>
        [BsonElement("isBuyOrder")]
        public bool IsBuyOrder { get; set; }

        /// <summary>
        /// Gets or sets the duration set for the buy order. Measured in days.
        /// </summary>
        /// <returns></returns>
        [BsonElement("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the date the order was created.
        /// </summary>
        [BsonElement("issued")]
        public DateTime Issued { get; set; }

        /// <summary>
        /// The range this order is allowed to be acted upon.
        /// </summary>
        /// <returns></returns>
        [BsonElement("range")]
        public string Range { get; set; }
    }
}