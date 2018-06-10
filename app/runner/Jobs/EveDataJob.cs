// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using Common.Data;
    using Common.Model;
    using Common.Model.Eve;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public abstract class EveDataJob : Job
    {
        private static Uri baseEveDataEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveDataJob"/> class.
        /// </summary>
        /// <param name="jobSpec">The job specification this is running for.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public EveDataJob(JobSpecLite jobSpec, IDbFactory dbFactory)
            : base(jobSpec, dbFactory)
        {
        }

        /// <summary>
        /// Creates a standardized URI for the configured EVE data endpoint.
        /// </summary>
        /// <param name="part">The part of the url that is specific to this call.</param>
        /// <returns>A uri object with the fully configured URI.</returns>
        protected Uri CreateEndpoint(string part)
        {
            if (baseEveDataEndpoint == null)
            {
                var setting = this.DbFactory.GetCollectionAsQueryable<Common.Model.Setting<string>>(
                    CollectionNames.Settings).First(x => x.Key == "eveDataUri");
                baseEveDataEndpoint = new Uri(setting.Value.EndsWith("/", StringComparison.InvariantCulture) ? setting.Value : setting.Value + "/");
            }

            if (part.StartsWith("/", System.StringComparison.InvariantCulture))
            {
                return new Uri(baseEveDataEndpoint, part.Substring(1));
            }

            return new Uri(baseEveDataEndpoint, part);
        }
    }
}