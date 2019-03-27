using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Apps72.Dev.Data.Retry
{
    /// <summary>
    /// Parameters to retry a query when a SqlException occured (ex. DeadLock exception).
    /// </summary>
    public class DatabaseRetryExceptions
    {
        // Gets or sets the number of retries already occured.
        private int _retryCount = 0;

        /// <summary>
        /// Gets or sets the number of retries to execute before the process failed.
        /// </summary>
        public virtual int NumberOfRetriesBeforeFailed { get; set; } = 3;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before a new command will be executed.
        /// </summary>
        public virtual int MillisecondsBetweenTwoRetries { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the list of error codes to catch and to retry.
        /// </summary>
        public virtual List<int> ErrorCodesToRetry { get; set; } = new List<int>();

        /// <summary>
        /// Sets the default ErrorCodesToRetry list with DeadLock codes (1205).
        /// </summary>
        public virtual void AddSqlServerDeadLockCodes()
        {
            this.ErrorCodesToRetry.Add(1205);
        }

        /// <summary>
        /// Returns True if at least one error code to check is defined.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCodeDefined()
        {
            return this.ErrorCodesToRetry.Count > 0;
        }

        /// <summary>
        /// Returns True if the specified DbException is known by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual bool IsExceptionToRetry(DbException ex)
        {
            if (ex != null)
            {
                // Check if error occured is known
                if (this.ErrorCodesToRetry.Contains(ex.ErrorCode))
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
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual bool IsNotAnExceptionToRetry(DbException ex)
        {
            return !IsExceptionToRetry(ex);
        }

        /// <summary>
        /// Increment the Retry Counter,
        /// And wait some milliseconds before to continue.
        /// Returns False if the counter is greater than NumberOfRetriesBeforeFailed value.
        /// </summary>
        /// <returns></returns>
        protected virtual bool WaitBeforeRetry()
        {
            _retryCount++;

            if (_retryCount < this.NumberOfRetriesBeforeFailed)
            {
                System.Threading.Thread.Sleep(this.MillisecondsBetweenTwoRetries);
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
        protected virtual void ResetRetryCounter()
        {
            _retryCount = 0;
        }

        /// <summary>
        /// Execute the specified method (ExecuteTable, ExecuteNonQuery or ExecuteScalar)
        /// And retry x times if asked by RetryIfExceptionsOccured property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual T ExecuteCommandOrRetryIfErrorOccured<T>(Func<T> action)
        {
            if (this.IsCodeDefined())
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
                        if (this.IsNotAnExceptionToRetry(ex)) throw;

                        // Need to execute this command (action) again
                        toRetry = this.WaitBeforeRetry();

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
