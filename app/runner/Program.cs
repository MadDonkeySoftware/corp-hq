// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Data;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
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
            var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
            DbFactory.SetClient(new MongoClient(connString));

            Monitor.Initialize(new ConnectionFactory(), new DbFactory());
        }
    }
}