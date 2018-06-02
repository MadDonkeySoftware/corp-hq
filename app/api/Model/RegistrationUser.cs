// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the data submitted for user registration.
    /// </summary>
    public class RegistrationUser : IValidatableObject
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
        [StringLength(255, ErrorMessage = "Must be between 8 and 255 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password confirmation.
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        [DataType(DataType.Password)]
        [JsonProperty("passwordConfirm")]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terms and conditions have been accepted.
        /// </summary>
        /// <returns>true if terms have been accepted, false otherwise.</returns>
        [JsonProperty("terms")]
        public bool TermsAccepted { get; set; }

        /// <summary>
        /// Performs model validations that cannot be done through attributes.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>An enumerable set of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.TermsAccepted)
            {
                yield return new ValidationResult("You must accept the terms and conditions.", new List<string> { "TermsAccepted" });
            }
        }
    }
}