using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Exceptions
{
    /// <summary>
    /// Exception produced if there is invalid arguments
    /// </summary>
    [Serializable]
    public class GnossAPIArgumentException : GnossAPIException
    {
        /// <summary>
        /// Constructor of <see cref="GnossAPIArgumentException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="paramName">Parameter with some error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPIArgumentException(string message, string paramName, Exception originalException)
            :base(message, paramName, originalException)
        {

        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIArgumentException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="paramName">Parameter with some error</param>
        public GnossAPIArgumentException(string message, string paramName)
            : base(message, paramName)
        {

        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIArgumentException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPIArgumentException(string message, Exception originalException)
            : base(message, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIArgumentException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        public GnossAPIArgumentException(string message)
            : base(message)
        {
        }
    }
}
