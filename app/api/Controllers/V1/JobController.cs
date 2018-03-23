// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Api.Model;
    using Common;
    using Common.Data;
    using Common.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    /// <summary>
    /// Controller for Task actions.
    /// </summary>
    [V1Route]
    public class JobController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;
        private readonly ConnectionFactory connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        /// <param name="factory">The factory for RabbitMQ connections.</param>
        public JobController(ILogger<RegistrationController> logger, IDbFactory dbFactory, ConnectionFactory factory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
            this.connectionFactory = factory;
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
            Console.WriteLine(newUser);
            this.logger.LogDebug(1001, "Adding new task to the queue.");
            var newTaskUuid = Guid.NewGuid();
            var col = this.dbFactory.GetCollection<Job<object>>("corp-hq", CollectionNames.Jobs);

            col.InsertOne(new Job<object>
            {
                Uuid = newTaskUuid.ToString(),
                Type = JobTypes.ApplyDbIndexes,

                // Data = JsonConvert.SerializeObject(new Dictionary<string, string> { { "arg", "arg1" } })
                Data = null
            });

            using (var connection = this.connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // TODO: Move channel queue declare to startup.
                channel.QueueDeclare(
                    queue: "jobs",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "jobs",
                    basicProperties: null,
                    body: newTaskUuid.ToByteArray());
            }

            return this.Accepted(new { uuid = newTaskUuid });
        }
    }
}