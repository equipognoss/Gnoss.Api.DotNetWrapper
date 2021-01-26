using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Gnoss.ApiWrapper.Web
{

    /// <summary>
    /// Gnoss Web client to override the default behaviour of autoredirect property
    /// </summary>
    public class GnossWebClient : WebClient
    {

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public GnossWebClient() { }

        /// <summary>
        /// Constructor with autoredirect option
        /// </summary>
        /// <param name="pAutoRedirect">Set the autoredirect property</param>
        public GnossWebClient(bool pAutoRedirect)
        {
            AutoRedirect = pAutoRedirect;
        }

        #endregion

        /// <summary>
        /// Gets a web request
        /// </summary>
        /// <param name="address">Uri address to request</param>
        /// <returns>WebRequest</returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AllowAutoRedirect = AutoRedirect;
            return request;
        }


        #region Properties

        /// <summary>
        /// Sets the behaviour for autoredirect
        /// </summary>
        public bool AutoRedirect { get; set; }

        #endregion
    }
}
