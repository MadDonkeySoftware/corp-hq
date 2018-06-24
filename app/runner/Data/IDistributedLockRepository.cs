// Copyright (c) MadDonkeySoftware

namespace Runner.Data
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;

    /// <summary>
    /// Repository responsible for handling distributed locks.
    /// </summary>
    public interface IDistributedLockRepository
    {
        /// <summary>
        /// Acquire a distributed lock on a resource.
        /// </summary>
        /// <param name="key">The key to lock upon.</param>
        /// <param name="expiry">Maximum length of time to lock the key.</param>
        /// <returns>Returns a new lock object.</returns>
        IDistributedLock AcquireLock(string key, TimeSpan expiry);

        /// <summary>
        /// Acquire a distributed lock on a resource.
        /// </summary>
        /// <param name="key">The key to lock upon.</param>
        /// <param name="expiry">Maximum length of time to lock the key.</param>
        /// <param name="wait">Maximum length of time to attempt to grab the lock.</param>
        /// <param name="retry">How long to wait between each attempt to grab the lock.</param>
        /// <returns>Returns a new lock object.</returns>
        IDistributedLock AcquireLock(string key, TimeSpan expiry, TimeSpan wait, TimeSpan retry);
    }
}