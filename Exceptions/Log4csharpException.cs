using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper
{
    /// <summary>
    /// Exception produced by the logging class
    /// </summary>
    public class Log4csharpException : ApplicationException
    {
        /// <summary>
        /// Constructor of <see cref="Log4csharpException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="originalException">Original exception</param>
        public Log4csharpException(string message, Exception originalException)
            : base(message, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="Log4csharpException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        public Log4csharpException(string message)
            : base(message)
        {
        }
    }
}
