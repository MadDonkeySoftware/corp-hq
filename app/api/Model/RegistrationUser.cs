// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the data submitted for user registration.
    /// </summary>
    public class RegistrationUser
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
                return this.username.ToUpperInvariant();
            }

            set
            {
                this.username = value;
                this.DisplayName = value;
            }
        }

        /// <summary>
        /// Gets the display name for this user based on the user name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password confirmation.
        /// </summary>
        [Required]
        [JsonProperty("passwordConfirm")]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}