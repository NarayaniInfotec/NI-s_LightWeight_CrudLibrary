using System;
using System.Collections.Generic;
using System.Text;

namespace NarayaniInfotec.DataLite.Core
{
    /// <summary>
    /// A class to handle exceptions.
    /// </summary>
    public class DataLiteException : Exception
    {
        /// <summary>
        /// Gets details of exception.
        /// </summary>
        /// <param name="message">Details of exception occured</param>
        /// <param name="innerException"></param>
        public DataLiteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
