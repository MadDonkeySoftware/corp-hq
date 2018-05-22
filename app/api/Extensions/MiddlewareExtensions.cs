// Copyright (c) MadDonkeySoftware

namespace Api.Extensions
{
    using Api.Middleware;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Extension methods for dotnet core web api middleware
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Oh god the documentation I don't know what to write because it's late.
        /// </summary>
        /// <param name="builder">The builder we are extending</param>
        /// <returns>What it should return</returns>
        public static IApplicationBuilder UseRequestToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTokenMiddleware>();
        }
    }
}