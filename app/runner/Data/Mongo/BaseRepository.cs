// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Mongo
{
    using System;
    using System.Linq;
    using Common.Data;
    using Common.Model;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Base repository responsible for common responsibilities.
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        protected BaseRepository(IDbFactory dbFactory)
        {
            this.DbFactory = dbFactory;
        }

        /// <summary>
        /// Gets the configured db factory.
        /// </summary>
        /// <returns>The db factory.</returns>
        protected IDbFactory DbFactory { get; private set; }
    }
}