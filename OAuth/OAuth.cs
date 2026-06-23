using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

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
        /// <param name="httpMethod">Method used GET, POST, PUT, DELETE</param>
        /// <param name="requestUrl">Url to sign</param>
        /// <param name="additionalParams">Aditionals params of the query string</param>
        /// <returns></returns>
        private NameValueCollection GetOAuthParameters(string httpMethod, string requestUrl, NameValueCollection additionalParams = null)
        {
            OAuthBase oauthBase = new OAuthBase(ConsumerKey, ConsumerSecret);
            return oauthBase.GetOAuthParametersWithoutEncode(
                httpMethod,
                requestUrl,   // la URL real del endpoint
                Token,
                TokenSecret,
                null,
                null,
                additionalParams
            );
        }

        /// <summary>
        /// Sign a url to make a oauth request
        /// </summary>
        /// <param name="httpMethod">Method used GET, POST, PUT, DELETE</param>
        /// <param name="requestUrl">Url to sign</param>
        /// <param name="additionalParams">Aditionals params of the query string</param>
        /// <returns>The formated OAuth header</returns>
        public string GetOAuthHeader(string httpMethod, string requestUrl, NameValueCollection additionalParams = null)
        {
            NameValueCollection parameters = GetOAuthParameters(httpMethod, requestUrl, additionalParams);

            return string.Format(
                "OAuth realm=\"Example\", " +
                "oauth_consumer_key=\"{0}\", " +
                "oauth_token=\"{1}\", " +
                "oauth_signature_method=\"{2}\", " +
                "oauth_signature=\"{3}\", " +
                "oauth_timestamp=\"{4}\", " +
                "oauth_nonce=\"{5}\", " +
                "oauth_version=\"{6}\"",
                OAuthBase.UrlEncode(parameters["oauth_consumer_key"]),  // puede tener caracteres especiales
                OAuthBase.UrlEncode(parameters["oauth_token"]),         // puede tener caracteres especiales
                parameters["oauth_signature_method"],
                OAuthBase.UrlEncode(parameters["oauth_signature"]),     // base64: siempre necesita encoding
                parameters["oauth_timestamp"],
                parameters["oauth_nonce"],
                parameters["oauth_version"]
            );
        }
    }
}
