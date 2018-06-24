// Copyright (c) MadDonkeySoftware

namespace Runner.Data.Redis
{
    using System;
    using System.Linq;
    using Common.Model;
    using Common.Model.Eve;
    using RedLockNet;

    /// <summary>
    /// Object for containing lock information
    /// </summary>
    public class DistributedLock : IDistributedLock, IDisposable
    {
        private bool isDisposed = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedLock"/> class.
        /// </summary>
        /// <param name="redLock">The redis lock object to wrap.</param>
        public DistributedLock(IRedLock redLock)
        {
            this.RedLock = redLock;
        }

        /// <summary>
        /// Gets a value indicating whether if the lock was successfully acquired.
        /// </summary>
        /// <returns>True if the lock was successfully acquired; False otherwise.</returns>
        public bool IsAcquired
        {
            get
            {
                return this.RedLock.IsAcquired;
            }
        }

        private IRedLock RedLock { get; set; }

        /// <summary>
        /// Disposes of managed and unmanaged resources referenced by this class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of managed and unmanaged resources referenced by this class.
        /// </summary>
        /// <param name="disposing">True to disposing managed and unmanaged resources. False to only cleanup unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Clean up managed resources here
                    this.RedLock.Dispose();
                }

                // Clean up any unmanaged resources here.
            }
        }
    }
}