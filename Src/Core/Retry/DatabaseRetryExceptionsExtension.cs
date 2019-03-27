using System;
using System.Data.Common;

namespace Apps72.Dev.Data.Retry
{
    /// <summary>
    /// Extensions methods to manipulate DatabaseRetryExceptions
    /// </summary>
    internal static class DatabaseRetryExceptionsExtension
    {

        /// <summary>
        /// Returns True if the retryExceptions object is defined with at least one error code to check
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <returns></returns>
        public static bool IsDefined(this DatabaseRetryExceptions retryExceptions)
        {
            return retryExceptions != null && retryExceptions.ErrorCodesToRetry.Count > 0;
        }

        /// <summary>
        /// Returns True if the specified DbException is known by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsExceptionToRetry(this DatabaseRetryExceptions retryExceptions, DbException ex)
        {
            if (ex != null)
            {
                // Check if error occured is known
                if (retryExceptions.ErrorCodesToRetry.Contains(ex.ErrorCode))
                {
                    return true;
                }
                //else
                //{
                //    // Check also all sub errors
                //    foreach (var exToCheck in ex.InnerException.Errors)
                //    {
                //        if (retryExceptions.ErrorCodesToRetry.Contains(exToCheck.Number))
                //        {
                //            return true;
                //        }
                //    }
                //    return false;
                //}
            }
            return false;
        }

        /// <summary>
        /// Returns True if the specified DbException is unknown by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsNotAnExceptionToRetry(this DatabaseRetryExceptions retryExceptions, DbException ex)
        {
            return !IsExceptionToRetry(retryExceptions, ex);
        }

        /// <summary>
        /// Increment the Retry Counter of DatabaseRetryExceptions,
        /// And wait some milliseconds before to continue.
        /// Returns False if the counter is greater than NumberOfRetriesBeforeFailed value.
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <returns></returns>
        public static bool WaitBeforeRetry(this DatabaseRetryExceptions retryExceptions)
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
        /// Reset the Retry Counter to zero.
        /// </summary>
        /// <param name="retryExceptions"></param>
        public static void ResetRetryCounter(this DatabaseRetryExceptions retryExceptions)
        {
            retryExceptions.RetryCount = 0;
        }

        /// <summary>
        /// Execute the specified method (ExecuteTable, ExecuteNonQuery or ExecuteScalar)
        /// And retry x times if asked by RetryIfExceptionsOccured property.
        /// </summary>
        /// <param name="retryExceptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T ExecuteCommandOrRetryIfErrorOccured<T>(this DatabaseRetryExceptions retryExceptions, Func<T> action)
        {
            if (retryExceptions != null && retryExceptions.IsDefined())
            {
                bool toRetry = false;
                DbException lastException = null;

                do
                {
                    try
                    {
                        // Execute the query
                        return action.Invoke();
                    }
                    catch (DbException ex)
                    {
                        lastException = ex;

                        // Check if a unknown Exception
                        if (retryExceptions.IsNotAnExceptionToRetry(ex)) throw;

                        // Need to execute this command (action) again
                        toRetry = retryExceptions.WaitBeforeRetry();

                        // If exeed the number of retries... So, throw this last exception
                        if (!toRetry) throw;

                        // Trace the error occured
                        //if (toRetry && this.Log != null)
                        //{
                        //    this.Log.Invoke(String.Format("Retry activated. SqlException #{1} was: \"{0}\".", this.Exception.Message, this.RetryIfExceptionsOccured.RetryCount - 1));
                        //}
                    }
                } while (toRetry);

                throw lastException;
            }
            else
            {
                return action.Invoke();
            }
        }
    }
}
