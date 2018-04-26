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
    public class DbFactory : IDbFactory
    {
        private static IMongoClient client;
        private static string database;

        /// <summary>
        /// Gets the mongo client.
        /// </summary>
        /// <returns>A <see cref="IMongoClient"/> instance.</returns>
        public IMongoClient Client
        {
            get
            {
                if (TestClient != null)
                {
                    return TestClient;
                }

                if (DbFactory.client == null)
                {
                    var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
                    DbFactory.client = new MongoClient(connString);
                }

                return DbFactory.client;
            }
        }

        public string Database
        {
            get
            {
                if (TestDatabase != null)
                {
                    return TestDatabase;
                }

                if (string.IsNullOrEmpty(DbFactory.database))
                {
                    DbFactory.database = Environment.GetEnvironmentVariable("MONGO_DATABASE") ?? "corp-hq";
                }

                return DbFactory.database;
            }
        }

        internal static IMongoClient TestClient { get; set; }
        internal static string TestDatabase { get; set; }

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
        public IMongoCollection<T> GetCollection<T>(string collectionName, string databaseName = null, IMongoClient client = null)
        {
            if (client == null)
            {
                client = this.Client;
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = this.Database;
            }

            return client.GetDatabase(databaseName).GetCollection<T>(collectionName);
        }

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
        public IQueryable<T> GetCollectionAsQueryable<T>(string collectionName, string databaseName = null, IMongoClient client = null, AggregateOptions aggregateOptions = null)
        {
            var col = this.GetCollection<T>(collectionName, databaseName, client);
            return col.AsQueryable(aggregateOptions);
        }

        public static void SetClient(IMongoClient client)
        {
            DbFactory.client = client;
        }
    }
}