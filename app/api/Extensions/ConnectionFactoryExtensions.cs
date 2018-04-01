// Copyright (c) MadDonkeySoftware

namespace Api.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using MongoDB.Driver;
    using RabbitMQ.Client;

    /// <summary>
    /// Extension methods for RabbitMQ.Client.ConnectionFactory
    /// </summary>
    public static class ConnectionFactoryExtensions
    {
        private static readonly object Padlock = new object();
        private static Common.Model.RabbitMQ.Root rabbitSettings;

        private static Common.Model.RabbitMQ.Root RabbitSettings
        {
            get
            {
                lock (Padlock)
                {
                    if (rabbitSettings == null)
                    {
                        var dbFactory = new DbFactory();
                        var settingsCol = dbFactory.GetCollection<Common.Model.Setting<Common.Model.RabbitMQ.Root>>("corp-hq", CollectionNames.Settings);
                        var settings = settingsCol.AsQueryable().Where(s => s.Key == "rabbitConnection").First().Value;
                        rabbitSettings = settings;
                    }

                    return rabbitSettings;
                }
            }
        }

        /// <summary>
        /// Create a connection to one of the endpoints provided by the IEndpointResolver
        /// returned by the EndpointResolverFactory. This uses the connection settings
        /// present in the mongo database.
        /// </summary>
        /// <param name="factory">The factory being extended</param>
        /// <returns>A new IConnection to the RabbitMQ broker.</returns>
        public static IConnection CreateConfiguredConnection(this ConnectionFactory factory)
        {
            factory.UserName = RabbitSettings.Username;
            factory.Password = RabbitSettings.Password;
            var hosts = RabbitSettings.Hosts.Select(h => h.Address).ToList();

            return factory.CreateConnection(hosts);
        }
    }
}