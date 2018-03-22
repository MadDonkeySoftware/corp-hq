// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using Common.Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// A class representing a user inside of the system.
    /// </summary>
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
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        [BsonElement("passwordSalt")]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; }
    }
}