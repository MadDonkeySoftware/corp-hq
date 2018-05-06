// Copyright (c) MadDonkeySoftware

namespace Api.Model.Response
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the basic details of a Region
    /// </summary>
    public class RegionLite
    {
        /// <summary>
        /// Gets or sets the name of the region
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the region
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}