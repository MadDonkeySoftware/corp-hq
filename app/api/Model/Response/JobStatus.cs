// Copyright (c) MadDonkeySoftware

namespace Api.Model.Response
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the main response for the GET job status endpoint.
    /// </summary>
    public class JobStatus
    {
        /// <summary>
        /// Gets or sets status of the job
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}