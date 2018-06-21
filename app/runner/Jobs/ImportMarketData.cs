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
    using RedLockNet.SERedis;
    using Runner.Data;
    using StackExchange.Redis;

    /// <summary>
    /// Job for creating the mongo indexes
    /// </summary>
    public class ImportMarketData : EveDataJob
    {
        private static readonly SmartHttpClient Client = new SmartHttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMarketData"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository used to persist information relating to this job.</param>
        /// <param name="settingRepository">The setting repository used to acquire setting information.</param>
        /// <param name="distributedLockRepository">The redis lock factory.</param>
        /// <param name="marketOrderRepository">The market order repository used to acquire and persist market information.</param>
        /// <param name="distributedKeyValueRepository">The distributed key value store used so we don't hit the Eve API too hard.</param>
       public ImportMarketData(IJobRepository jobRepository, ISettingRepository settingRepository, IDistributedLockRepository distributedLockRepository, IMarketOrderRepository marketOrderRepository, IDistributedKeyValueRepository distributedKeyValueRepository)
            : base(jobRepository, settingRepository)
        {
            this.DistributedLockRepository = distributedLockRepository;
            this.MarketOrderRepository = marketOrderRepository;
            this.DistributedKeyValueRepository = distributedKeyValueRepository;
        }

        private IDistributedLockRepository DistributedLockRepository { get; set; }

        private IMarketOrderRepository MarketOrderRepository { get; set; }

        private IDistributedKeyValueRepository DistributedKeyValueRepository { get; set; }

        /// <summary>
        /// The main body for the job being run.
        /// </summary>
        protected override void Work()
        {
            this.AddMessage(JobMessageLevel.Trace, "Starting market data import.");

            var jobData = this.JobRepository.GetJobData<MarketDataImport>(this.JobSpec.Uuid);
            this.ImportEveMarketData(jobData);

            this.AddMessage(JobMessageLevel.Trace, "Finished importing market data.");
        }

        private void ImportEveMarketData(MarketDataImport jobData)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("Accept", "application/json");

            var expiry = TimeSpan.FromSeconds(60);
            var wait = TimeSpan.FromSeconds(60);
            var retry = TimeSpan.FromSeconds(1);
            foreach (var id in jobData.MarketTypeIds)
            {
                var key = $"ImportMarketData-{jobData.RegionId}-{id}";

                using (var distLock = this.DistributedLockRepository.AcquireLock(key, expiry, wait, retry))
                {
                    if (distLock.IsAcquired)
                    {
                        var cacheKey = $"Refresh:{key}";
                        if (this.DistributedKeyValueRepository.Retrieve(cacheKey) != null)
                        {
                            this.AddMessage($"Cache indicates data for region {jobData.RegionId} item {id} fetched recently; skipping.");
                        }
                        else
                        {
                            this.ImportMarketDataForEveType(jobData.RegionId, id);
                            this.DistributedKeyValueRepository.Persist(cacheKey, key, TimeSpan.FromSeconds(300));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("TEMP: Could not acquire redis lock.");
                    }
                }
            }
        }

        private void ImportMarketDataForEveType(int regionId, int id)
        {
            this.AddMessage("Fetching orders for region '{0}' and type '{1}'.", regionId, id);
            var page = 1;
            var orderCount = 0;

            do
            {
                var uri = this.CreateEndpoint(string.Format(
                    CultureInfo.InvariantCulture, "/markets/{0}/orders?type_id={1}&page={2}", regionId, id, page));
                var result = Client.GetWithReties(uri);
                var marketOrders = JsonConvert.DeserializeObject<List<EveMarketOrder>>(result);

                foreach (var order in marketOrders)
                {
                    var marketOrder = new MarketOrder
                    {
                        OrderId = order.OrderId,
                        TypeId = order.TypeId,
                        LocationId = order.LocationId,
                        VolumeTotal = order.VolumeTotal,
                        VolumeRemain = order.VolumeRemain,
                        MinVolume = order.MinVolume,
                        Price = order.Price,
                        IsBuyOrder = order.IsBuyOrder,
                        Duration = order.Duration,
                        Issued = order.Issued,
                        Range = order.Range,
                        ExpireAt = order.Issued.AddDays(order.Duration)
                    };
                    this.MarketOrderRepository.Save(marketOrder);
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