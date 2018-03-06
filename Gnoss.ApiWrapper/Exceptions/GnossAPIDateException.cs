using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Exceptions
{
    /// <summary>
    /// Exception produced when there is problems converting dates. 
    /// </summary>
    public class GnossAPIDateException : GnossAPIException
    {
        /// <summary>
        /// Constructor of <see cref="GnossAPIDateException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPIDateException(string message, Exception originalException)
            : base(message, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPIDateException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        public GnossAPIDateException(string message)
            : base(message)
        {
        }
    }
}
