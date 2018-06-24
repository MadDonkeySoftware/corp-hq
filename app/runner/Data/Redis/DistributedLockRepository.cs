// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Redis
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;
    using RedLockNet.SERedis;

    /// <summary>
    /// Repository responsible for handling distributed locks.
    /// </summary>
    public class DistributedLockRepository : IDistributedLockRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLockRepository"/> class.
        /// </summary>
        /// <param name="redLockFactory">The redis lock factory to wrap.</param>
        public DistributedLockRepository(RedLockFactory redLockFactory)
        {
            this.RedLockFactory = redLockFactory;
        }

        private RedLockFactory RedLockFactory { get; set; }

        /// <summary>
        /// Acquire a distributed lock on a resource.
        /// </summary>
        /// <param name="key">The key to lock upon.</param>
        /// <param name="expiry">Maximum length of time to lock the key.</param>
        /// <returns>Returns a new lock object.</returns>
        public IDistributedLock AcquireLock(string key, TimeSpan expiry)
        {
            var redLock = this.RedLockFactory.CreateLock(key, expiry);
            return new DistributedLock(redLock);
        }

        /// <summary>
        /// Acquire a distributed lock on a resource.
        /// </summary>
        /// <param name="key">The key to lock upon.</param>
        /// <param name="expiry">Maximum length of time to lock the key.</param>
        /// <param name="wait">Maximum length of time to attempt to grab the lock.</param>
        /// <param name="retry">How long to wait between each attempt to grab the lock.</param>
        /// <returns>Returns a new lock object.</returns>
        public IDistributedLock AcquireLock(string key, TimeSpan expiry, TimeSpan wait, TimeSpan retry)
        {
            var redLock = this.RedLockFactory.CreateLock(key, expiry, wait, retry);
            return new DistributedLock(redLock);
        }
    }
}