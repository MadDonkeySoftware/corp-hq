// Copyright (c) MadDonkeySoftware

namespace Api.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Api.Model.Data;
    using Common.Data;
    using Microsoft.AspNetCore.Http;
    using MongoDB.Driver;

    /// <summary>
    /// Request Token Middleware
    /// </summary>
    public class RequestTokenMiddleware
    {
        // TODO: This is a terrible way to maintain the list of non-token-requiring endpoints. Figure out something better.
        private static readonly List<string> IgnorePathPatterns = new List<string>
        {
            @"/health$",
            @"/api/v1/authentication$",
            @"/api/v1/registration$",
            @"/api/v1/token/.*$"
        };

        private readonly RequestDelegate next;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTokenMiddleware"/> class.
        /// </summary>
        /// <param name="next">Middleware next thingie...</param>
        /// <param name="dbFactory">The db factory used to verify tokens</param>
        public RequestTokenMiddleware(RequestDelegate next, IDbFactory dbFactory)
        {
            this.next = next;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// Invokes this handler
        /// </summary>
        /// <param name="context">The request context</param>
        /// <returns>The text invocation</returns>
        public Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            var verb = context.Request.Method;

            if (!IsIgnoredPath(path))
            {
                if (!context.Request.Headers.Keys.Contains("auth-token"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return context.Response.WriteAsync("Missing auth-token header.");
                }
                else
                {
                    var token = context.Request.Headers["auth-token"].FirstOrDefault();
                    var remote = context.Connection.RemoteIpAddress.ToString();

                    var sessionCol = this.dbFactory.GetCollection<UserSession>(CollectionNames.Sessions);
                    var session = sessionCol.AsQueryable<UserSession>().Where(s => s.Key == token).FirstOrDefault();

                    Console.WriteLine("remote: " + remote);

                    if (session == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return context.Response.WriteAsync("Unauthorized.");
                    }
                    else
                    {
                        var filterCondition = Builders<UserSession>.Filter.Eq(s => s.Key, token);
                        var updateCondition = Builders<UserSession>.Update.Set(s => s.ExpireAt, DateTime.Now.AddMinutes(30));
                        sessionCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = true });
                    }
                }
            }

            return this.next(context);
        }

        private static bool IsIgnoredPath(string path)
        {
            foreach (var pattern in IgnorePathPatterns)
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                if (regex.Match(path).Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}