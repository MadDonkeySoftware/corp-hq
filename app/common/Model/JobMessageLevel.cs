// Copyright (c) MadDonkeySoftware

namespace Common.Model
{
    /// <summary>
    /// Message levels to assist filtering noise when querying job messages.
    /// </summary>
    public enum JobMessageLevel
    {
        /// <summary>
        /// Indicates a message of the lowest level. This is the most verbose named level.
        /// </summary>
        Trace = 10,

        /// <summary>
        /// Indicates a message of the second lowest level.
        /// </summary>
        Debug = 20,

        /// <summary>
        /// Indicates a message of the middle level.
        /// </summary>
        Info = 30,

        /// <summary>
        /// Indicates a message of the second from highest level.
        /// </summary>
        Warn = 40,

        /// <summary>
        /// Indicates a message of the highest level. This is the least verbose named level.
        /// </summary>
        Error = 50,
    }
}