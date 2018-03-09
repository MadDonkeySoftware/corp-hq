// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using Microsoft.AspNetCore.Mvc.Routing;

    /// <summary>
    /// An attribute to make V1 controller routes consistent
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class V1RouteAttribute : Attribute, IRouteTemplateProvider
    {
        /// <summary>
        /// Gets the template for V1 routes.
        /// </summary>
        public string Template => "api/v1/[controller]";

        /// <summary>
        /// Gets or sets the order in which to execute this route.
        /// </summary>
        /// <returns>The order in which to execute this route</returns>
        public int? Order { get; set; }

        /// <summary>
        /// Gets or sets the name for this route.
        /// </summary>
        /// <returns>The name for this route.</returns>
        public string Name { get; set; }
    }
}