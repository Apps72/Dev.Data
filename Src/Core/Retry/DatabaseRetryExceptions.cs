using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.Retry
{
    /// <summary>
    /// Parameters to retry a query when a SqlException occured (ex. DeadLock exception).
    /// </summary>
    public class DatabaseRetryExceptions
    {
        /// <summary>
        /// Gets or sets the number of retries to execute before the process failed.
        /// </summary>
        public int NumberOfRetriesBeforeFailed { get; set; } = 3;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before a new command will be executed.
        /// </summary>
        public int MillisecondsBetweenTwoRetries { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the list of error codes to catch and to retry.
        /// </summary>
        public List<int> ErrorCodesToRetry { get; set; } = new List<int>();

        /// <summary>
        /// Sets the default ErrorCodesToRetry list with DeadLock codes (1205).
        /// </summary>
        public void SetDeadLockCodes()
        {
            this.ErrorCodesToRetry.Add(1205);
        }

        /// <summary>
        /// Gets or sets the number of retries already occured.
        /// </summary>
        internal int RetryCount { get; set; }
    }

}
