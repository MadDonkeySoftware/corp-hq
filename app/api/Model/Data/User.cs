// Copyright (c) MadDonkeySoftware

namespace Api.Model.Data
{
    using System.Diagnostics.CodeAnalysis;
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>:w
    public class User : MongoBase
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [BsonElement("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [BsonElement("password")]
        [SuppressMessage("Microsoft.Performance", "CA1819::PropertiesShouldNotReturnArrays", Justification="Must be done this way to store in Mongo")]
        public byte[] Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        [BsonElement("passwordSalt")]
        [SuppressMessage("Microsoft.Performance", "CA1819::PropertiesShouldNotReturnArrays", Justification="Must be done this way to store in Mongo")]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; }
    }
}