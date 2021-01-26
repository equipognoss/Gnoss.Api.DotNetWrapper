using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gnoss.ApiWrapper.Exceptions
{
    /// <summary>
    /// Exceptions produced during the execution of the api
    /// </summary>
    public class GnossAPIException : ArgumentException
    {
        /// <summary>
        /// Constructor of <see cref="GnossAPIException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="paramName">Parameter with some error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPIException(string message, string paramName, Exception originalException)
            : base(message, paramName, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="paramName">Parameter with some error</param>
        public GnossAPIException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPIException(string message, Exception originalException)
            : base(message, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        public GnossAPIException(string message)
            : base(message)
        {
        }
    }
}
