// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Api.Extensions;
    using Api.Model;
    using Api.Model.Data;
    using Common.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// Controller for Registration actions.
    /// </summary>
    [V1Route]
    public class TokenController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        public TokenController(ILogger<TokenController> logger, IDbFactory dbFactory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// The PUT verb handler
        /// </summary>
        /// <param name="token">The token being refreshed.</param>
        /// <returns>No Content</returns>
        /// <remarks>
        /// PUT api/v1/token/{id}
        /// </remarks>
        [HttpPut("{token}")]
        public ActionResult Post(string token)
        {
            this.logger.LogDebug(1001, "Refreshing token.");
            var sessionCol = this.dbFactory.GetCollection<UserSession>(CollectionNames.Sessions);

            var filterCondition = Builders<UserSession>.Filter.Eq(s => s.Key, token);
            var updateCondition = Builders<UserSession>.Update.Set(s => s.ExpireAt, DateTime.Now.AddMinutes(30));
            sessionCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = false });

            return this.NoContent();
        }

        /// <summary>
        /// The DELETE verb handler
        /// </summary>
        /// <param name="token">The token being removed.</param>
        /// <returns>No Content</returns>
        /// <remarks>
        /// DELETE api/v1/token/{id}
        /// </remarks>
        [HttpDelete("{token}")]
        public ActionResult Delete(string token)
        {
            this.logger.LogDebug(1001, "Removing token.");
            var sessionCol = this.dbFactory.GetCollection<UserSession>(CollectionNames.Sessions);

            var filterCondition = Builders<UserSession>.Filter.Eq(s => s.Key, token);
            sessionCol.DeleteOne(filterCondition);

            return this.NoContent();
        }
    }
}