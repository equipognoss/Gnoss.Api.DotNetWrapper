using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Exceptions
{
    /// <summary>
    /// Exception produced if the resource categories not exists in the thesaurus of the community
    /// </summary>
    public class GnossAPICategoryException : ArgumentException
    {
        /// <summary>
        /// Constructor of <see cref="GnossAPICategoryException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="originalException">Original exception</param>
        public GnossAPICategoryException(string message, Exception originalException)
            : base(message, originalException)
        {
        }

        /// <summary>
        /// Constructor of <see cref="GnossAPICategoryException"/>
        /// </summary>
        /// <param name="message">Message of the error</param>
        public GnossAPICategoryException(string message)
            : base(message)
        {
        }
    }
}
