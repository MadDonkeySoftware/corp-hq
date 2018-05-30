// Copyright (c) MadDonkeySoftware

namespace Api.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Extension methods for Microsoft.AspNetCore.Mvc.Controller.ModelState
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Queries the ModelState for existing error messages
        /// </summary>
        /// <param name="modelState">The ModelStateDictionary object from Controller.ModelState</param>
        /// <returns>
        /// A list of error messages from ModelState
        /// </returns>
        public static Dictionary<string, List<string>> Errors(this ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var key in modelState.Keys)
            {
                var entry = modelState[key];
                var messages = new List<string>();
                foreach (var error in entry.Errors)
                {
                    messages.Add(error.ErrorMessage);
                }

                if (messages.Count > 0)
                {
                    errors[key] = messages;
                }
            }

            return errors;
        }
    }
}