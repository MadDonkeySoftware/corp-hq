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
        private static IConnectionFactory rabbitConnectionFactory;
        private static DbFactory dbFactory;

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
            Program.dbFactory = new DbFactory();

            // TODO: Get rabbit connection details from the database.
            var col = dbFactory.GetCollection<TaskRunner>("corp-hq", "settings");
            rabbitConnectionFactory = new ConnectionFactory() { HostName = "localhost", UserName = "rabbitmq", Password = "rabbitmq" };
            Monitor.Initialize(rabbitConnectionFactory, dbFactory);
        }
    }
}