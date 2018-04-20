// Copyright (c) MadDonkeySoftware

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// Struct to hold 
    /// </summary>
    public static class JobTypes
    {
        public const string ApplyDbIndexes = "ApplyDbIndexes";
        public const string ImportMapData = "ImportMapData";
        public const string ImportMarketData = "ImportMarketData";

        /// <summary>
        /// Converts the available job types to an enumerable of strings
        /// </summary>
        /// <returns>The enunerable of job types</returns>
        public static IEnumerable<string> ToEnumerable()
        {
            return typeof(JobTypes)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetRawConstantValue())
                .ToArray();
        }
    }
}