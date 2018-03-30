// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the data submitted for user registration.
    /// </summary>
    public class AuthenticationBody
    {
        private string username;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [JsonProperty("username")]
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}