// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the data submitted for user registration.
    /// </summary>
    public class EnqueueJob
    {
        /// <summary>
        /// Gets or sets the job type.
        /// </summary>
        [Required]
        [JsonProperty("jobType")]
        public string JobType { get; set; }
    }
}