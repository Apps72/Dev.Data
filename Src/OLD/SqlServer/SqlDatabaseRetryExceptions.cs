using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Parameters to retry a query when a SqlException occured (ex. DeadLock exception).
    /// </summary>
    public class SqlDatabaseRetryExceptions
    {
        /// <summary>
        /// Initializes a new instance of SqlDatabaseRetryExceptions.
        /// </summary>
        public SqlDatabaseRetryExceptions()
        {
            this.NumberOfRetriesBeforeFailed = 3;
            this.MillisecondsBetweenTwoRetries = 1000;
            this.ErrorCodesToRetry = new List<int>();
        }

        /// <summary>
        /// Gets or sets the number of retries to execute before the process failed.
        /// </summary>
        public int NumberOfRetriesBeforeFailed { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before a new command will be executed.
        /// </summary>
        public int MillisecondsBetweenTwoRetries { get; set; }

        /// <summary>
        /// Gets or sets the list of error codes to catch and to retry.
        /// </summary>
        public List<int> ErrorCodesToRetry { get; set; }

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

    /// <summary>
    /// Extensions methods to manipulate SqlDatabaseRetryExceptions
    /// </summary>
    internal static class SqlDatabaseRetryExceptionsExtension
    {

        /// <summary>
        /// Returns True if the retryExceptions object is defined with at least one error code to check
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <returns></returns>
        public static bool IsDefined(this SqlDatabaseRetryExceptions retryExceptions)
        {
            return retryExceptions != null && retryExceptions.ErrorCodesToRetry.Count > 0;
        }

        /// <summary>
        /// Returns True if the specified SqlException is known by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsAnExceptionToRetry(this SqlDatabaseRetryExceptions retryExceptions, System.Data.SqlClient.SqlException ex)
        {
            if (ex != null)
            {
                // Check if error occured is known
                if (retryExceptions.ErrorCodesToRetry.Contains(ex.Number))
                {
                    return true;
                }
                else
                {
                    // Check also all sub errors
                    foreach (System.Data.SqlClient.SqlError exToCheck in ex.Errors)
                    {
                        if (retryExceptions.ErrorCodesToRetry.Contains(exToCheck.Number))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns True if the specified SqlException is unknown by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsNotAnExceptionToRetry(this SqlDatabaseRetryExceptions retryExceptions, System.Data.SqlClient.SqlException ex)
        {
            return !IsAnExceptionToRetry(retryExceptions, ex);
        }

        /// <summary>
        /// Increment the Retry Counter of SqlDatabaseRetryExceptions,
        /// And wait some milliseconds before to continue.
        /// Returns False if the counter is greater than NumberOfRetriesBeforeFailed value.
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <returns></returns>
        public static bool IsMustRetryAndWait(this SqlDatabaseRetryExceptions retryExceptions)
        {
            retryExceptions.RetryCount++;

            if (retryExceptions.RetryCount < retryExceptions.NumberOfRetriesBeforeFailed)
            {
                System.Threading.Thread.Sleep(retryExceptions.MillisecondsBetweenTwoRetries);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reset the Retry Counter to 0
        /// </summary>
        /// <param name="retryExceptions"></param>
        public static void ResetRetryCounter(this SqlDatabaseRetryExceptions retryExceptions)
        {
            retryExceptions.RetryCount = 0;
        }
    }
}
