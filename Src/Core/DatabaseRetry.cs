using System;
using System.Data.Common;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Retry a query when a SqlException occured (ex. DeadLock exception).
    /// </summary>
    public class DatabaseRetry
    {
        // Gets or sets the number of retries already occured.
        private int _retryCount = 0;
        private IDatabaseCommand _command;

        /// <summary>
        /// Initializes a new instance of DatabaseRetryExceptions
        /// </summary>
        /// <param name="command"></param>
        internal DatabaseRetry(IDatabaseCommand command)
        {
            _command = command;
        }

        /// <summary>
        /// Gets or sets the number of retries to execute before the process failed.
        /// </summary>
        public virtual int NumberOfRetriesBeforeFailed { get; set; } = 3;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before a new command will be executed.
        /// </summary>
        public virtual int MillisecondsBetweenTwoRetries { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the list of methods executed to check the error codes and to retry.
        /// </summary>
        public virtual Func<DbException, bool> CriteriaToRetry { get; set; }

        /// <summary>
        /// Sets the default ErrorCodesToRetry list with DeadLock codes (1205).
        /// </summary>
        public virtual void SetDefaultCriteriaToRetry(RetryDefaultCriteria value)
        {
            switch (value)
            {
                case RetryDefaultCriteria.SqlServer_DeadLock:
                    this.CriteriaToRetry = (ex) =>
                    {
                        return ex.Message.Contains("deadlock") ||
                               ex.InnerException?.Message.Contains("deadlock") == true;
                    };
                    break;

                case RetryDefaultCriteria.OracleServer_DeadLock:
                    this.CriteriaToRetry = (ex) =>
                    {
                        return ex.Message.Contains("ORA-04061") ||
                               ex.Message.Contains("ORA-04068") ||
                               ex.InnerException?.Message.Contains("ORA-04061") == true ||
                               ex.InnerException?.Message.Contains("ORA-04068") == true;
                    };
                    break;

            }
        }

        /// <summary>
        /// Activate the retry process, defining ExceptionsToRetry criterias.
        /// Ex. ExceptionsToRetry = (ex) => { return ex.Message.Contains("deadlock"); };
        /// </summary>
        /// <param name="options"></param>
        public virtual void Activate(Action<DatabaseRetry> options)
        {
            this.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);
            options.Invoke(this);
        }

        /// <summary>
        /// Returns True if the specified DbException is known by the ErrorCodesToRetry list
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual bool IsExceptionToRetry(DbException ex)
        {
            return this.CriteriaToRetry?.Invoke(ex) == true;
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
        internal virtual T ExecuteCommandOrRetryIfErrorOccured<T>(Func<T> action)
        {
            if (this.IsActivated())
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
                        if (this.IsNotAnExceptionToRetry(ex))
                            throw;

                        // Need to execute this command (action) again
                        toRetry = this.WaitBeforeRetry();

                        // If exeed the number of retries... So, throw this last exception
                        if (!toRetry)
                            throw;

                        // Trace the error occured
                        _command.Log?.Invoke($"Retry activated. SqlException #{_retryCount} was: \"{ex.Message}\".");
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

    /// <summary>
    /// List of default definitions for <see cref="CriteriaToRetry"/>
    /// </summary>
    public enum RetryDefaultCriteria
    {
        /// <summary>
        /// Deadlock in SQL Server (Message contains "deadlock").
        /// </summary>
        SqlServer_DeadLock,
        /// <summary>
        /// Existing state of package in Oracle Server (Message contains "ORA-04068" or "ORA-04061").
        /// </summary>
        OracleServer_DeadLock
    }

    internal static class DatabaseRetryExtensions
    {
        /// <summary>
        /// Returns True if a check criteria is defined.
        /// </summary>
        /// <returns></returns>
        public static bool IsActivated(this DatabaseRetry retry)
        {
            return retry != null && retry.CriteriaToRetry != null;
        }
    }
}
