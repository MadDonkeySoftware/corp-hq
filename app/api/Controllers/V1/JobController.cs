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
    using Api.Model.Data;
    using Api.Model.Response;
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
        private readonly ISmartConnectionFactory connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        /// <param name="factory">The factory for RabbitMQ connections.</param>
        public JobController(ILogger<JobController> logger, IDbFactory dbFactory, ISmartConnectionFactory factory)
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
        public IActionResult GetStatus(string jobUuid)
        {
            var jobCol = this.dbFactory.GetCollectionAsQueryable<JobSpecLite>(CollectionNames.Jobs);
            var status = jobCol.Where(j => j.Uuid == jobUuid).Select(j => j.Status).FirstOrDefault();

            if (string.IsNullOrEmpty(status))
            {
                var response = new ApiResponse<JobStatus>();
                response.Messages.Add("job Uuid not found");
                return this.NotFound(response);
            }

            return this.Ok(new ApiResponse<JobStatus> { Result = new JobStatus { Status = status } });
        }

        /// <summary>
        /// The GET verb handler that provides details on a specific job.
        /// </summary>
        /// <param name="jobUuid">The job to get info on.</param>
        /// <returns>The details for the specified job.</returns>
        [HttpGet("{jobUuid}")]
        public IActionResult Get(string jobUuid)
        {
            var jobCol = this.dbFactory.GetCollectionAsQueryable<JobSpec<dynamic>>(CollectionNames.Jobs);
            var jobSpec = jobCol.Where(j => j.Uuid == jobUuid).Select(j => new { Status = j.Status, Type = j.Type, Start = j.StartTimestamp, End = j.EndTimestamp, Children = j.Children }).FirstOrDefault();

            if (jobSpec == null)
            {
                var respData = new ApiResponse<JobDetails>();
                respData.Messages.Add("Not Found");
                return this.NotFound(respData);
            }

            var messagesCol = this.dbFactory.GetCollectionAsQueryable<JobMessage>(CollectionNames.JobMessages);
            var messages = messagesCol.Where(m => (m.JobUuid == jobUuid || m.MasterJobUuid == jobUuid) && m.Level > (ushort)JobMessageLevel.Debug)
                                      .OrderBy(x => x.Timestamp)
                                      .Select(m => m.Message)
                                      .ToList();

            var details = new JobDetails
            {
                Uuid = jobUuid,
                Status = jobSpec.Status,
                Type = jobSpec.Type,
                StartTimestamp = jobSpec.Start,
                EndTimestamp = jobSpec.End,
            };
            details.Messages.AddRange(messages);

            return this.Ok(new ApiResponse<JobDetails>
            {
                Result = details
            });
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
        public IActionResult Post([FromBody]EnqueueJob jobDetails)
        {
            this.logger.LogDebug(1001, "Adding new job to the queue.");
            var newJobUuid = Guid.NewGuid();
            var col = this.dbFactory.GetCollection<JobSpec<string>>(CollectionNames.Jobs);

            var messages = VerifyJobType(jobDetails.JobType);
            if (messages.Count() > 0)
            {
                var details = new ApiResponse<JobCreated>();
                details.Messages.AddRange(messages);
                return this.BadRequest(details);
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

            // TODO: Make base URL configurable. -- CORPHQ_API_URL
            var baseUrl = new Uri("/api/v1/job/");
            var jobUrl = new Uri(baseUrl, newJobUuid.ToString());
            return this.Created(jobUrl, new ApiResponse<JobCreated> { Result = new JobCreated { Uuid = newJobUuid } });
        }

        private static List<string> VerifyJobType(string jobType)
        {
            var messages = new List<string>();
            Console.WriteLine("JobType" + jobType);
            var availableTypes = JobTypes.ToEnumerable();

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