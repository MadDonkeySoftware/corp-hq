// Copyright (c) MadDonkeySoftware

namespace Common.Data
{
    using System;
    using System.Linq;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

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
        /// <param name="collectionName">The collection in the corresponding mongo database.</param>
        /// <param name="databaseName">An optional mongo database to connect to. Defaults to provided environment variable</param>
        /// <param name="client">An optional client with which to connect.</param>
        /// <typeparam name="T">The type of data that this collection houses.</typeparam>
        /// <remarks>
        /// Use the optional "databaseName" if the intent is to connect to a database other that what was provided during application startup.
        /// </remarks>
        /// <returns>A <see cref="IMongoCollection{T}"/>.</returns>
        IMongoCollection<T> GetCollection<T>(string collectionName, string databaseName = null, IMongoClient client = null);

        /// <summary>
        /// Gets the underlying mongo collection for the provided information in Queryable mode.
        /// </summary>
        /// <param name="collectionName">The collection in the corresponding mongo database.</param>
        /// <param name="databaseName">An optional mongo database to connect to.</param>
        /// <param name="client">An optional client with which to connect.</param>
        /// <param name="aggregateOptions">An optional set of aggretation options for the queryable call.</param>
        /// <typeparam name="T">The type of data that this collection houses.</typeparam>
        /// <remarks>
        /// Use the optional "databaseName" if the intent is to connect to a database other that what was provided during application startup.
        /// </remarks>
        /// <returns>A <see cref="IMongoQueryable{T}"/>.</returns>
        IQueryable<T> GetCollectionAsQueryable<T>(string collectionName, string databaseName = null, IMongoClient client = null, AggregateOptions aggregateOptions = null);
    }
}