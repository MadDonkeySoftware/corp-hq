// Copyright (c) MadDonkeySoftware

namespace Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Extensions;
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
        /// The GET verb handler that provides details on a specific job.
        /// </summary>
        /// <param name="jobUuid">The job to get info on.</param>
        /// <returns>The details for the specified job.</returns>
        [HttpGet("status/{jobUuid}")]
        public dynamic GetStatus(string jobUuid)
        {
            var jobCol = this.dbFactory.GetCollection<JobSpecLite>("corp-hq", CollectionNames.Jobs);
            var status = jobCol.AsQueryable().Where(j => j.Uuid == jobUuid).Select(j => j.Status).FirstOrDefault();

            return new
            {
                status = status,
            };
        }

        /// <summary>
        /// The GET verb handler that provides details on a specific job.
        /// </summary>
        /// <param name="jobUuid">The job to get info on.</param>
        /// <returns>The details for the specified job.</returns>
        [HttpGet("{jobUuid}")]
        public dynamic Get(string jobUuid)
        {
            var jobCol = this.dbFactory.GetCollection<JobSpec<dynamic>>("corp-hq", CollectionNames.Jobs);
            var jobSpec = jobCol.AsQueryable().Where(j => j.Uuid == jobUuid).Select(j => new { Status = j.Status, Type = j.Type, Start = j.StartTimestamp, End = j.EndTimestamp }).FirstOrDefault();

            if (jobSpec == null)
            {
                return this.NotFound(new { message = "Not Found" });
            }

            var messagesCol = this.dbFactory.GetCollection<JobMessage>("corp-hq", CollectionNames.JobMessages);
            var messages = messagesCol.AsQueryable().Where(m => m.JobUuid == jobUuid).Select(m => m.Message).ToList();

            return new
            {
                uuid = jobUuid,
                status = jobSpec.Status,
                type = jobSpec.Type,
                startTimestamp = jobSpec.Start,
                endTimestamp = jobSpec.End,
                messages = messages
            };
        }

        /// <summary>
        /// The POST verb handler
        /// </summary>
        /// <param name="jobDetails">The details submitted for the new job.</param>
        /// <returns>Success or error.</returns>
        /// <remarks>
        /// POST api/values
        /// </remarks>
        [HttpPost]
        public ActionResult Post([FromBody]EnqueueJob jobDetails)
        {
            this.logger.LogDebug(1001, "Adding new job to the queue.");
            var newJobUuid = Guid.NewGuid();
            var col = this.dbFactory.GetCollection<JobSpec<string>>("corp-hq", CollectionNames.Jobs);

            var messages = VerifyJobType(jobDetails.JobType);
            if (messages.Count() > 0)
            {
                return this.BadRequest(new { messages = messages });
            }

            // TODO: Make Job expirary configurable via the settings db.
            col.InsertOne(new JobSpec<string>
            {
                Uuid = newJobUuid.ToString(),
                Type = jobDetails.JobType,
                Data = JsonConvert.SerializeObject(jobDetails.Data),
                Status = JobStatuses.New,
                ExpireAt = DateTime.Now.AddDays(3)
            });

            using (var connection = this.connectionFactory.CreateConfiguredConnection())
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
                    body: newJobUuid.ToByteArray());
            }

            return this.Accepted(new { uuid = newJobUuid });
        }

        private static List<string> VerifyJobType(string jobType)
        {
            // TODO: Find a better way to do this. Reflection maybe? Also, some job types will be restricted to certain users.
            // Figure that out as well.
            var messages = new List<string>();
            Console.WriteLine("JobType" + jobType);
            var availableTypes = new[]
            {
                JobTypes.ApplyDbIndexes,
                JobTypes.ImportMapData,
                JobTypes.ImportMarketData
            };

            if (availableTypes.Contains(jobType))
            {
                Console.WriteLine("JobType found.");
                return messages;
            }

            Console.WriteLine("JobType not found.");
            messages.Add(string.Format(CultureInfo.InvariantCulture, "Job Type \"{0}\" unknown.", jobType));
            messages.Add(string.Format(CultureInfo.InvariantCulture, "Available Job Types: {0}", string.Join(", ", availableTypes)));

            return messages;
        }
    }
}