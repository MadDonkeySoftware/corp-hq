// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using Common.Data;
    using Common.Model.Eve;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    internal class ImportMapData : Job<object>
    {
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            var dbFactory = new DbFactory();
            this.ImportRegions(dbFactory);
        }

        private void ImportRegions(IDbFactory dbFactory)
        {
            this.AddMessage("Fetching list of regions.");

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");

            var uri = new Uri("https://esi.tech.ccp.is/latest/universe/regions");
            var regionsTask = Client.GetStringAsync(uri);
            var regions = JsonConvert.DeserializeObject<List<int>>(regionsTask.Result);

            var regionCol = dbFactory.GetCollection<Region>("corp-hq", CollectionNames.Regions);
            foreach (var regionId in regions)
            {
                this.AddMessage("Fetching data for region: {0}.", regionId);
                uri = new Uri(string.Concat("https://esi.tech.ccp.is/latest/universe/regions/", regionId));
                var regionDetailsTask = Client.GetStringAsync(uri);
                var regionDetails = JsonConvert.DeserializeObject<RegionDetailsData>(regionDetailsTask.Result);

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