// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Common.Data;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Connections;
    using RabbitMQ.Client;
    using RedLockNet.SERedis;
    using RedLockNet.SERedis.Configuration;
    using Runner.Data;
    using Runner.Data.Mongo;
    using Runner.Data.Redis;
    using Runner.Jobs;
    using StackExchange.Redis;

    /// <summary>
    /// Class to handle application initialization.
    /// </summary>
    internal static class Bootstrap
    {
        private static readonly Lazy<IServiceProvider> Instance = new Lazy<IServiceProvider>(() => services.BuildServiceProvider());
        private static IServiceCollection services = new ServiceCollection();

        /// <summary>
        /// Gets a service provider instance.
        /// </summary>
        /// <returns>A service provider instance.</returns>
        internal static IServiceProvider ServiceProvider
        {
            get
            {
                return Instance.Value;
            }
        }

        /// <summary>
        /// Main work method to invoke the bootstrapping process.
        /// </summary>
        internal static void Initialize()
        {
            services.AddLogging();

            // Repositories
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<IJobMessageRepository, JobMessageRepository>();
            services.AddTransient<IMapRepository, MapRepository>();
            services.AddTransient<IMarketOrderRepository, MarketOrderRepository>();
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<IDistributedLockRepository, DistributedLockRepository>();
            services.AddTransient<IDistributedKeyValueRepository, DistributedKeyValueRepository>();

            // Jobs
            services.AddTransient<CreateMongoIndexes>();
            services.AddTransient<ImportMapData>();
            services.AddTransient<ImportMarketData>();

            // Other
            services.AddTransient<Monitor>();
            services.AddTransient<IDbFactory, DbFactory>();
            services.AddSingleton<RabbitMQ.Client.IConnectionFactory, ConnectionFactory>();
            AddDbs();
        }

        private static void AddDbs()
        {
            // TODO: Clean this up.
            var environment = Environment.GetEnvironmentVariable("CORP_HQ_ENVIRONMENT");
            var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
            DbFactory.SetClient(new MongoClient(connString));

            var dbFactory = new DbFactory();
            var settingsCollection = dbFactory.GetCollection<Common.Model.Setting<Common.Model.Redis.Root>>(CollectionNames.Settings);

            var redisSettings = settingsCollection.AsQueryable().Where(s => s.Key == "redisConnection" && s.Environment == environment).First().Value;
            var connStr = string.Join(",", redisSettings.Hosts.Select(h => $"{h.Address}:{h.Port}"));
            var redisConnection = ConnectionMultiplexer.Connect(connStr);
            var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { redisConnection });

            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddSingleton<RedLockFactory>(redLockFactory);

            // HACK to clean up the redis stuff in the mean time.
            Monitor.Instance.Disposed += (object sender, EventArgs args) =>
            {
                redLockFactory.Dispose();
                redisConnection.Dispose();
            };
        }
    }
}