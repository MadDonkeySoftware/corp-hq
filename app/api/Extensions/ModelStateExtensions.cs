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
        public static List<string> Errors(this ModelStateDictionary modelState)
        {
            var errors = from x in modelState.Values
                from y in x.Errors
                where x.Errors.Count > 0
                select y.ErrorMessage;

            if (errors.Any())
            {
                return errors.ToList();
            }

            return new List<string>();
        }
    }
}