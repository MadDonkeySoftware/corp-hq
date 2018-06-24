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
    /// Repository responsible for job retrieval and persistance
    /// </summary>
    public class SettingRepository : BaseRepository, ISettingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingRepository"/> class.
        /// </summary>
        /// <param name="dbFactory">The dbFactory for this job to use.</param>
        public SettingRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        /// <summary>
        /// Fetches a setting from the data store.
        /// </summary>
        /// <param name="key">The key that relates to the setting.</param>
        /// <typeparam name="T">The type to hydrate the setting into.</typeparam>
        /// <returns>A object representing the setting.</returns>
        public T FetchSetting<T>(string key)
        {
            var environment = Environment.GetEnvironmentVariable("CORP_HQ_ENVIRONMENT");
            var settings = this.DbFactory.GetCollectionAsQueryable<Common.Model.Setting<T>>(CollectionNames.Settings).Where(
                x => x.Key == key && x.Environment == environment).ToList();

            var settingsCount = settings.Count();
            if (settingsCount == 0)
            {
                // TODO: Log warning.
                // Next try without the environment filter
                settings = this.DbFactory.GetCollectionAsQueryable<Common.Model.Setting<T>>(CollectionNames.Settings).Where(
                    x => x.Key == key).ToList();
            }

            T setting;
            if (settingsCount > 1)
            {
                throw new Exception($"Multiple settings found for key: {key}, environment: {environment}.");
            }
            else
            {
                setting = settings.First().Value;
            }

            return setting;
        }
    }
}