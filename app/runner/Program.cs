// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Data;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RedLockNet.SERedis;
    using RedLockNet.SERedis.Configuration;
    using Runner.Model;

    /// <summary>
    /// The class containing the main entry point for the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        public static void Main()
        {
            ConfigureDependencies();

            // RabbitListener();
            Monitor.Instance.Start();
        }

        private static void ConfigureDependencies()
        {
            Bootstrap.Initialize();

            var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
            DbFactory.SetClient(new MongoClient(connString));

            var dbFactory = new DbFactory();
            var settingsCollection = dbFactory.GetCollection<Common.Model.Setting<Common.Model.Redis.Root>>(CollectionNames.Settings);

            var redisSettings = settingsCollection.AsQueryable().Where(s => s.Key == "redisConnection").First().Value;
            var endPoints = new List<RedLockEndPoint>();
            foreach (var endPoint in redisSettings.Hosts.Select(h => new DnsEndPoint(h.Address, h.Port)).ToList())
            {
                endPoints.Add(endPoint);
            }

            var redLockFactory = RedLockFactory.Create(endPoints);

            Monitor.Initialize(new ConnectionFactory(), new DbFactory(), redLockFactory);
        }
    }
}