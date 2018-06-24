// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;

    /// <summary>
    /// Object for containing lock information
    /// </summary>
    public interface IDistributedLock : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether if the lock was successfully acquired.
        /// </summary>
        /// <returns>True if the lock was successfully acquired; False otherwise.</returns>
        bool IsAcquired { get; }
    }
}