using System;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Argument of ExceptionOccured event.
    /// </summary>
    public class ExceptionOccuredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the exception occured
        /// </summary>
        public Exception Exception { get; set; }
    }
}
