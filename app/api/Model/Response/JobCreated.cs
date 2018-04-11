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
    public class JobCreated
    {
        /// <summary>
        /// Gets or sets status of the job
        /// </summary>
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }
    }
}