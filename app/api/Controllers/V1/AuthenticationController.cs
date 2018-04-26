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
    public class AuthenticationController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        public AuthenticationController(ILogger<RegistrationController> logger, IDbFactory dbFactory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// The POST verb handler
        /// </summary>
        /// <param name="authDetails">The details submitted for a user.</param>
        /// <returns>Success or error.</returns>
        /// <remarks>
        /// POST api/values
        /// </remarks>
        [HttpPost]
        public ActionResult Post([FromBody]AuthenticationBody authDetails)
        {
            if (!this.ModelState.IsValid)
            {
                // get built in errors
                var errs = this.ModelState.Errors();
                if (errs.Any())
                {
                    return this.BadRequest(new { messages = errs });
                }
            }

            this.logger.LogDebug(1001, "Authenticating user.");
            var userCol = this.dbFactory.GetCollection<User>(CollectionNames.Users);
            var user = userCol.AsQueryable<User>().Where(u => u.Username == authDetails.Username).FirstOrDefault();

            if (user != null)
            {
                var salt = user.PasswordSalt;
                var hashed = user.Password;
                var submittedHash = SecurityHelpers.GenerateSaltedHash(authDetails.Password, salt);

                if (SecurityHelpers.CompareByteArrays(hashed, submittedHash))
                {
                    var sessionCol = this.dbFactory.GetCollection<UserSession>(CollectionNames.Sessions);
                    var token = SecurityHelpers.GetToken();
                    var remote = this.HttpContext.Connection.RemoteIpAddress.ToString();

                    var userSession = new UserSession
                    {
                        Endpoint = remote,
                        ExpireAt = DateTime.Now.AddMinutes(30),
                        Key = token,
                        Username = user.Username
                    };

                    sessionCol.InsertOne(userSession);

                    return this.Ok(new { token = token });
                }
            }

            return this.Unauthorized();
        }
    }
}