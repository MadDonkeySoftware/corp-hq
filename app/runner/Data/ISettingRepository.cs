// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using MongoDB;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    /// <summary>
    /// Repository responsible for setting retrieval and persistance
    /// </summary>
    public interface ISettingRepository
    {
        /// <summary>
        /// Fetches a setting from the data store.
        /// </summary>
        /// <param name="key">The key that relates to the setting.</param>
        /// <typeparam name="T">The type to hydrate the setting into.</typeparam>
        /// <returns>A object representing the setting.</returns>
        T FetchSetting<T>(string key);
    }
}