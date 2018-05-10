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
    using Common.Model.Eve;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    /// <summary>
    /// Controller for Map actions.
    /// </summary>
    [V1Route]
    public class MapController : Controller
    {
        private readonly ILogger logger;
        private readonly IDbFactory dbFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapController"/> class.
        /// </summary>
        /// <param name="logger">The logger in which to use.</param>
        /// <param name="dbFactory">The factory for DB connections.</param>
        public MapController(ILogger<JobController> logger, IDbFactory dbFactory)
        {
            this.logger = logger;
            this.dbFactory = dbFactory;
        }

        /// <summary>
        /// The GET verb handler that provides details on a specific job.
        /// </summary>
        /// <returns>The details for the specified job.</returns>
        [HttpGet("regions")]
        public IActionResult GetStatus()
        {
            var jobCol = this.dbFactory.GetCollectionAsQueryable<Region>(CollectionNames.Regions);
            var regions = jobCol.Select(r => new RegionLite { Id = r.RegionId, Name = r.Name }).ToList();

            return this.Ok(new ApiResponse<List<RegionLite>> { Result = regions });
        }
    }
}