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
    public class RegistrationController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        public RegistrationController(ILogger<RegistrationController> logger, IDbFactory dbFactory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// The POST verb handler
        /// </summary>
        /// <param name="newUser">The details submitted for a new user.</param>
        /// <returns>Success or error.</returns>
        /// <remarks>
        /// POST api/values
        /// </remarks>
        [HttpPost]
        public ActionResult Post([FromBody]RegistrationUser newUser)
        {
            Dictionary<string, List<string>> errors;
            if (!this.ModelState.IsValid)
            {
                // get built in errors
                errors = this.ModelState.Errors();
                if (errors.Any())
                {
                    return this.BadRequest(new { messages = errors });
                }

                return this.BadRequest("This is awkward, you missed some fields, but I'm not sure which ones...");
            }

            // get Action specific errors
            var col = this.dbFactory.GetCollection<User>(CollectionNames.Users);
            errors = ValidateUserRegistrationBody(newUser, col);
            if (errors.Any())
            {
                return this.BadRequest(new { messages = errors });
            }

            this.logger.LogDebug(1001, "Adding to list of values");

            var salt = SecurityHelpers.GetSalt();
            var hashed = SecurityHelpers.GenerateSaltedHash(newUser.Password, salt);

            col.InsertOne(new User
            {
                Username = newUser.Username,
                DisplayName = newUser.DisplayName,
                Password = hashed,
                PasswordSalt = salt,
                Email = newUser.Email
            });

            return this.Accepted();
        }

        private static Dictionary<string, List<string>> ValidateUserRegistrationBody(RegistrationUser user, IMongoCollection<User> col)
        {
            // Perform expensive validations.
            var usernameFree = col.Count(new BsonDocument { { "username", user.Username } }) == 0;
            var emailFree = col.Count(new BsonDocument { { "email", user.Email } }) == 0;

            if (!usernameFree || !emailFree)
            {
                return new Dictionary<string, List<string>>
                {
                   { "General", new List<string> { "Invalid username or email." } }
                };
            }

            return new Dictionary<string, List<string>>();
        }
    }
}