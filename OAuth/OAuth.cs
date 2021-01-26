using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections.Specialized;

namespace Gnoss.ApiWrapper.OAuth
{

    /// <summary>
    /// Class to get the basic info of an oauth account
    /// </summary>
    public class OAuthInfo
    {
        /// <summary>
        /// Gets the token of the account
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the token secret of the account
        /// </summary>
        public string TokenSecret { get; }

        /// <summary>
        /// Gets the consumer key associated to the token
        /// </summary>
        public string ConsumerKey { get; }

        /// <summary>
        /// Gets the consumer secret of the consumer
        /// </summary>
        public string ConsumerSecret { get; }
        
        /// <summary>
        /// Gets the Api URL
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Gets the email of the person responsible of the load
        /// </summary>
        public string DeveloperEmail { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiUrl">Api url</param>
        /// <param name="token">Token of the account</param>
        /// <param name="tokenSecret">Token secret of the account</param>
        /// <param name="consumerKey">Consumer key associated to the token</param>
        /// <param name="consumerSecret">Consumer secret of the consumer</param>
        /// <param name="developerEmail"></param>
        public OAuthInfo(string apiUrl, string token, string tokenSecret, string consumerKey, string consumerSecret, string developerEmail)
        {
            ApiUrl = apiUrl.TrimEnd('/');
            Token = token;
            TokenSecret = tokenSecret;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            DeveloperEmail = developerEmail;
        }

        /// <summary>
        /// Sign a url to make a oauth request
        /// </summary>
        /// <param name="url">Url to sign</param>
        /// <returns></returns>
        public string GetSignedUrl(string url)
        {
            OAuthBase oauthBase = new OAuthBase(ConsumerKey, ConsumerSecret);
            NameValueCollection parameters = oauthBase.GetOAuthParametersWithoutEncode("GET", url, Token, TokenSecret, null, null);

            string urlOauth = $"{url}?";

            foreach (string key in parameters.Keys)
            {
                urlOauth += $"{key}={parameters[key]}&";
            }

            urlOauth = urlOauth.Substring(0, urlOauth.Length - 1);

            return urlOauth;
        }

        /// <summary>
        /// Gets a signed url for the API
        /// </summary>
        public string OAuthSignedUrl
        {
            get
            {
                return GetSignedUrl(ApiUrl);
            }
        }
    }
}
