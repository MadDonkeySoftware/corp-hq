// Copyright (c) MadDonkeySoftware

namespace Common.Data
{
    using System;

    using MongoDB;
    using MongoDB.Driver;

    /// <summary>
    /// Factory for providing mongo related objects.
    /// </summary>
    public class DbFactory : IDbFactory
    {
        private static IMongoClient client;

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
                    return (MongoClient)TestClient;
                }

                if (DbFactory.client == null)
                {
                    var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
                    DbFactory.client = new MongoClient(connString);
                }

                return DbFactory.client;
            }
        }

        internal static object TestClient { get; set; }

        /// <summary>
        /// Gets the underlying mongo collection for the provided information.
        /// </summary>
        /// <param name="databaseName">The mongo database to connect to.</param>
        /// <param name="collectionName">The collection in the corresponding mongo database.</param>
        /// <param name="client">An optional client with which to connect.</param>
        /// <typeparam name="T">The type of data that this collection houses.</typeparam>
        /// <returns>A <see cref="IMongoCollection{T}"/>.</returns>
        public IMongoCollection<T> GetCollection<T>(string databaseName, string collectionName, IMongoClient client = null)
        {
            if (client == null)
            {
                client = DbFactory.client;
            }

            var db = client.GetDatabase(databaseName);
            return db.GetCollection<T>(collectionName);
        }

        public static void SetClient(IMongoClient client)
        {
            DbFactory.client = client;
        }
    }
}