// Copyright (c) MadDonkeySoftware

namespace Runner.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;

    using Common.Data;
    using Common.Model;
    using Common.Model.Eve;
    using Common.Model.JobData;
    using MongoDB.Driver;
    using Newtonsoft.Json;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    internal class ImportMarketData : Job
    {
        private static readonly HttpClient Client = new HttpClient();
        private IMongoCollection<MarketOrder> marketOrderCol;

        public ImportMarketData(string jobUuid)
            : base(jobUuid)
        {
        }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            this.AddMessage("Starting market data import.");

            var jobCol = DbFactory.GetCollection<JobSpec<string>>("corp-hq", CollectionNames.Jobs);
            var jobData = jobCol.AsQueryable().Where(j => j.Uuid == this.JobUuid).Select(j => j.Data).FirstOrDefault();

            if (string.IsNullOrEmpty(jobData))
            {
                throw new NullReferenceException("No job data could be found for job.");
            }

            var d = JsonConvert.DeserializeObject<MarketDataImport>(jobData);
            this.ImportEveMarketData(d);

            this.AddMessage("Finished importing market data.");
        }

        private void ImportEveMarketData(MarketDataImport jobData)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            this.marketOrderCol = DbFactory.GetCollection<MarketOrder>("corp-hq", CollectionNames.MarketOrders);

            foreach (var id in jobData.MarketTypeIds)
            {
                this.ImportMarketDataForEveType(jobData.RegionId, id);
            }
        }

        private void ImportMarketDataForEveType(int regionId, int id)
        {
            this.AddMessage("Fetching orders for region '{0}' and type '{1}'.", regionId, id);
            var page = 1;
            var orderCount = 0;

            do
            {
                var uri = new Uri(string.Format(
                    CultureInfo.InvariantCulture,
                    "https://esi.tech.ccp.is/latest/markets/{0}/orders?type_id={1}&page={2}",
                    regionId,
                    id,
                    page));
                var marketDataTask = Client.GetStringAsync(uri);
                var marketOrders = JsonConvert.DeserializeObject<List<EveMarketOrder>>(marketDataTask.Result);

                foreach (var order in marketOrders)
                {
                    var filterCondition = Builders<MarketOrder>.Filter.Eq(r => r.OrderId, order.OrderId);
                    var updateCondition = Builders<MarketOrder>.Update.Set(r => r.TypeId, order.TypeId)
                                                                      .Set(r => r.LocationId, order.LocationId)
                                                                      .Set(r => r.VolumeTotal, order.VolumeTotal)
                                                                      .Set(r => r.VolumeRemain, order.VolumeRemain)
                                                                      .Set(r => r.MinVolume, order.MinVolume)
                                                                      .Set(r => r.Price, order.Price)
                                                                      .Set(r => r.IsBuyOrder, order.IsBuyOrder)
                                                                      .Set(r => r.Duration, order.Duration)
                                                                      .Set(r => r.Issued, order.Issued)
                                                                      .Set(r => r.Range, order.Range)
                                                                      .Set(r => r.ExpireAt, order.Issued.AddDays(order.Duration));

                    this.marketOrderCol.UpdateOne(filterCondition, updateCondition, new UpdateOptions { IsUpsert = true });
                }

                orderCount = marketOrders.Count();
                page++;
            }
            while (orderCount == 1000);
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification="Used by Newtonsoft.Json")]
        internal class EveMarketOrder
        {
            [JsonProperty("order_id")]
            internal long OrderId { get; set; }

            [JsonProperty("type_id")]
            internal int TypeId { get; set; }

            [JsonProperty("location_id")]
            internal long LocationId { get; set; }

            [JsonProperty("volume_total")]
            internal int VolumeTotal { get; set; }

            [JsonProperty("volume_remain")]
            internal int VolumeRemain { get; set; }

            [JsonProperty("min_volume")]
            internal int MinVolume { get; set; }

            [JsonProperty("price")]
            internal double Price { get; set; }

            [JsonProperty("is_buy_order")]
            internal bool IsBuyOrder { get; set; }

            [JsonProperty("duration")]
            internal int Duration { get; set; }

            [JsonProperty("issued")]
            internal DateTime Issued { get; set; }

            [JsonProperty("range")]
            internal string Range { get; set; }
        }
    }
}