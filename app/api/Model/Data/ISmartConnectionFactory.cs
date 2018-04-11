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
    /// Connection factory aware of how to retrieve connection settings.
    /// </summary>
    public interface ISmartConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Gets the root rabbit connection settings.
        /// </summary>
        /// <returns>The root settings element.</returns>
        Common.Model.RabbitMQ.Root RabbitSettings { get; }

        /// <summary>
        /// Create a connection to one of the endpoints provided by the IEndpointResolver
        /// returned by the EndpointResolverFactory. This uses the connection settings
        /// present in the mongo database.
        /// </summary>
        /// <returns>A new IConnection to the RabbitMQ broker.</returns>
        IConnection CreateConfiguredConnection();
    }
}