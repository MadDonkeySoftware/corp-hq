// Copyright (c) MadDonkeySoftware

namespace Api.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents the main envalope for responses from the API.
    /// </summary>
    /// <typeparam name="T">The type that the Result will be.</typeparam>
    public class ApiResponse<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
        /// </summary>
        public ApiResponse()
        {
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Gets the messages from the users request. This is typically error messages or extra information.
        /// </summary>
        /// <returns>The list of messages.</returns>
        /// <remarks>
        /// Use the AddMessage method to add messages.
        /// </remarks>
        [JsonProperty("messages")]
        public List<string> Messages { get; private set; }

        /// <summary>
        /// Gets or sets the result from the users request.
        /// </summary>
        [JsonProperty("data")]
        public T Result { get; set; }
    }
}