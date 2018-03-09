// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Api.Data;
    using Api.Model;
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
            this.logger.LogDebug(1001, "Adding to list of values");
            var col = this.dbFactory.GetCollection<User>("corp-hq", "users");

            var error = this.ValidateUserRegistrationBody(newUser, col);
            if (error != null)
            {
                return error;
            }

            var salt = SecurityHelpers.GetSalt();
            var hashed = SecurityHelpers.GenerateSaltedHash(newUser.Password, salt);

            col.InsertOne(new User
            {
                Username = newUser.Username,
                Password = SecurityHelpers.BytesToString(hashed),
                PasswordSalt = SecurityHelpers.BytesToString(salt),
                Email = newUser.Email
            });

            return this.Accepted();
        }

        private BadRequestObjectResult ValidateUserRegistrationBody(RegistrationUser user, IMongoCollection<User> col)
        {
            // Do "cheap" validations first.
            // TODO: Figure out how to do validation the .NET way
            if (user == null)
            {
                return this.BadRequest(new { message = "Post body cannot be null." });
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return this.BadRequest(new { message = "Username is a required field." });
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return this.BadRequest(new { message = "Email is a required field." });
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return this.BadRequest(new { message = "Password is a required field." });
            }

            if (string.IsNullOrWhiteSpace(user.PasswordConfirm))
            {
                return this.BadRequest(new { message = "Password is a required field." });
            }

            if (user.Password != user.PasswordConfirm)
            {
                return this.BadRequest(new { message = "Passwords do not match." });
            }

            // Perform more expensive validations.
            var usernameFree = col.Count(new BsonDocument { { "username", user.Username } }) == 0;
            var emailFree = col.Count(new BsonDocument { { "email", user.Email } }) == 0;

            if (!usernameFree || !emailFree)
            {
                return this.BadRequest(new { message = "Invalid username or email." });
            }

            return null;
        }
    }
}