// Copyright (c) MadDonkeySoftware

namespace Api.Model.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using MongoDB.Driver;
    using RabbitMQ.Client;

    /// <summary>
    /// A connection factory that knows how to get its connection settings from Mongo.
    /// </summary>
    public class SmartConnectionFactory : ConnectionFactory, ISmartConnectionFactory
    {
        private static readonly object Padlock = new object();
        private static Common.Model.RabbitMQ.Root rabbitSettings;

        /// <summary>
        /// Gets the root rabbit connection settings.
        /// </summary>
        /// <returns>The root settings element.</returns>
        public Common.Model.RabbitMQ.Root RabbitSettings
        {
            get
            {
                lock (Padlock)
                {
                    if (rabbitSettings == null)
                    {
                        var dbFactory = new DbFactory();
                        var settingsCol = dbFactory.GetCollection<Common.Model.Setting<Common.Model.RabbitMQ.Root>>(CollectionNames.Settings);
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
        /// <returns>A new IConnection to the RabbitMQ broker.</returns>
        public IConnection CreateConfiguredConnection()
        {
            this.UserName = this.RabbitSettings.Username;
            this.Password = this.RabbitSettings.Password;
            var hosts = this.RabbitSettings.Hosts.Select(h => h.Address).ToList();

            return this.CreateConnection(hosts);
        }
    }
}