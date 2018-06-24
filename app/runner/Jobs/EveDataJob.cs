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
    using Runner.Data;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public abstract class EveDataJob : Job
    {
        private static Uri baseEveDataEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveDataJob"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        /// <param name="settingRepository">The setting repository used to acquire setting information.</param>
        public EveDataJob(IJobRepository jobRepository, ISettingRepository settingRepository)
            : base(jobRepository)
        {
            this.SettingRepository = settingRepository;
        }

        /// <summary>
        /// Gets the configured job repository.
        /// </summary>
        /// <returns>The job repository.</returns>
        protected ISettingRepository SettingRepository { get; private set; }

        /// <summary>
        /// Creates a standardized URI for the configured EVE data endpoint.
        /// </summary>
        /// <param name="part">The part of the url that is specific to this call.</param>
        /// <returns>A uri object with the fully configured URI.</returns>
        protected Uri CreateEndpoint(string part)
        {
            if (baseEveDataEndpoint == null)
            {
                var setting = this.SettingRepository.FetchSetting<string>("eveDataUri");
                baseEveDataEndpoint = new Uri(setting.EndsWith("/", StringComparison.InvariantCulture) ? setting : setting + "/");
            }

            if (part.StartsWith("/", System.StringComparison.InvariantCulture))
            {
                return new Uri(baseEveDataEndpoint, part.Substring(1));
            }

            return new Uri(baseEveDataEndpoint, part);
        }
    }
}