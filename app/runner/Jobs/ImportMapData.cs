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

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public class ImportMapData : EveDataJob
    {
        private static readonly SmartHttpClient Client = new SmartHttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMapData"/> class.
        /// </summary>
        /// <param name="jobSpec">The job specification this is running for.</param>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public ImportMapData(JobSpecLite jobSpec, IDbFactory dbFactory)
            : base(jobSpec, dbFactory)
        {
        }

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

            var regionCol = this.DbFactory.GetCollection<Region>(CollectionNames.Regions);
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

                var filterCondition = Builders<Region>.Filter.Eq(r => r.RegionId, regionId);
                var updateCondition = Builders<Region>.Update.Set(r => r.RegionId, regionId)
                                                             .Set(r => r.Name, regionDetails.Name)
                                                             .Set(r => r.ConstellationIds, regionDetails.ConstellationIds);

                regionCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = true });
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