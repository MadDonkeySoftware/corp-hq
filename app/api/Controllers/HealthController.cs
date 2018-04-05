// Copyright (c) MadDonkeySoftware

namespace Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for health check endpoint.
    /// </summary>
    [Route("[controller]")]
    public class HealthController : Controller
    {
        /// <summary>
        /// The GET verb handler that provides the load balancer a place to verify this APIs health.
        /// </summary>
        /// <returns>The health check response.</returns>
        [HttpGet]
        public ActionResult Get()
        {
            return this.Ok();
        }
    }
}