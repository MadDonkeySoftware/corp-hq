// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
    public class ImportMapData : EveDataJob
    {
        private static readonly SmartHttpClient Client = new SmartHttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMapData"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        /// <param name="settingRepository">The setting repository used to acquire setting information.</param>
        /// <param name="mapRepository">The map repository used to acquire and persist eve map information.</param>
        public ImportMapData(IJobRepository jobRepository, ISettingRepository settingRepository, IMapRepository mapRepository)
            : base(jobRepository, settingRepository)
        {
            this.MapRepository = mapRepository;
        }

        private IMapRepository MapRepository { get; set; }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            this.ImportRegions();
        }

        private void ImportRegions()
        {
            this.AddMessage("Fetching list of regions.");

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");

            var uri = this.CreateEndpoint("/universe/regions");
            var result = Client.GetWithReties(uri);
            var regions = JsonConvert.DeserializeObject<List<int>>(result);

            foreach (var regionId in regions)
            {
                this.AddMessage("Fetching data for region: {0}.", regionId);
                uri = this.CreateEndpoint(string.Concat("/universe/regions/", regionId));
                result = Client.GetWithReties(uri);
                var regionDetails = JsonConvert.DeserializeObject<RegionDetailsData>(result);

                var regionData = new Region
                {
                    BaseId = ObjectId.GenerateNewId(),
                    RegionId = regionId,
                    Name = regionDetails.Name,
                    ConstellationIds = regionDetails.ConstellationIds
                };

                this.MapRepository.SaveRegion(regionData);
            }

            this.AddMessage("Finished importing region data.");
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification="Used by Newtonsoft.Json")]
        internal class RegionDetailsData
        {
            [JsonProperty("name")]
            internal string Name { get; set; }

            [JsonProperty("region_id")]
            internal int RegionId { get; set; }

            [JsonProperty("constellations")]
            internal List<int> ConstellationIds { get; set; }
        }
    }
}