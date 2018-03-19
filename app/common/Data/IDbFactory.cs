// Copyright (c) MadDonkeySoftware

namespace Common.Data
{
    using System;

    using MongoDB;
    using MongoDB.Driver;

    /// <summary>
    /// Factory for providing mongo related objects.
    /// </summary>
    public interface IDbFactory
    {
        /// <summary>
        /// Gets the mongo client.
        /// </summary>
        /// <returns>A <see cref="IMongoClient"/> instance.</returns>
        IMongoClient Client { get; }

        /// <summary>
        /// Gets the underlying mongo collection for the provided information.
        /// </summary>
        /// <param name="databaseName">The mongo database to connect to.</param>
        /// <param name="collectionName">The collection in the corresponding mongo database.</param>
        /// <param name="client">An optional client with which to connect.</param>
        /// <typeparam name="T">The type of data that this collection houses.</typeparam>
        /// <returns>A <see cref="IMongoCollection{T}"/>.</returns>
        IMongoCollection<T> GetCollection<T>(string databaseName, string collectionName, IMongoClient client = null);
    }
}