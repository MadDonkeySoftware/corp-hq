// Copyright (c) MadDonkeySoftware

namespace Api.Model.Response
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the main response for the GET job status endpoint.
    /// </summary>
    public class JobDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobDetails"/> class.
        /// </summary>
        public JobDetails()
        {
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Gets or sets unique id of the job
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets status of the job
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets type of the job
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the starting timestamp for the job
        /// </summary>
        [JsonProperty("startTimestamp")]
        public DateTime? StartTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the ending timestamp for the job
        /// </summary>
        [JsonProperty("endTimestamp")]
        public DateTime? EndTimestamp { get; set; }

        /// <summary>
        /// Gets the messages associated with the job.
        /// </summary>
        [JsonProperty("messages")]
        public List<string> Messages { get; private set; }
    }
}