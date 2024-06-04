using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Web;

namespace Gnoss.ApiWrapper.OAuth
{
    /// <summary>
    /// Class to generate oauth parameters and sign urls
    /// </summary>
    public class OAuthBase
    {

        #region Constants

        /// <summary>
        /// OAuth version
        /// </summary>
        private const string OAuthVersion = "1.0";

        /// <summary>
        /// OAuth parameter prefix
        /// </summary>
        private const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        private const string OAuthConsumerKeyKey = "oauth_consumer_key";
        private const string OAuthCallbackKey = "oauth_callback";
        private const string OAuthVersionKey = "oauth_version";
        private const string OAuthSignatureMethodKey = "oauth_signature_method";
        private const string OAuthSignatureKey = "oauth_signature";
        private const string OAuthTimestampKey = "oauth_timestamp";
        private const string OAuthNonceKey = "oauth_nonce";
        private const string OAuthTokenKey = "oauth_token";
        private const string OAuthTokenSecretKey = "oauth_token_secret";

        private const string HMACSHA1SignatureType = "HMAC-SHA1";
        private const string PlainTextSignatureType = "PLAINTEXT";
        private const string RSASHA1SignatureType = "RSA-SHA1";

        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        #endregion

        #region Members

        private Random _random;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consumerKey">Consumer key</param>
        /// <param name="consumerSecret">Consumer secret</param>
        public OAuthBase(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            _random = new Random();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the oauth parameters
        /// </summary>
        /// <param name="httpMethod">Http method of the request</param>
        /// <param name="url">url</param>
        /// <param name="requestToken">token</param>
        /// <param name="tokenSecret">token secret</param>
        /// <param name="verifier">verifier</param>
        /// <param name="callback">callback</param>
        /// <returns>Collection of parameter - value</returns>
        public NameValueCollection GetOAuthParameters(string httpMethod, string url, string requestToken, string tokenSecret, string verifier, string callback)
        {
            var oAuthParameters = new NameValueCollection
                                            {
                                               {"oauth_timestamp",GenerateTimeStamp()},
                                               {"oauth_nonce",GenerateNonce()},
                                               {"oauth_version", "1.0"},
                                               {"oauth_signature_method", "HMAC-SHA1"},
                                               {"oauth_consumer_key", ConsumerKey},
                                            };


            if (!string.IsNullOrEmpty(requestToken)) oAuthParameters.Add("oauth_token", requestToken);

            if (!string.IsNullOrEmpty(verifier)) oAuthParameters.Add("oauth_verifier", verifier);

            if (!string.IsNullOrEmpty(callback)) oAuthParameters.Add("oauth_callback", callback);

            var signatureBase = GetSignatureBase(httpMethod, NormalizeUrl(url), oAuthParameters);
            var signature = GetSignature(ConsumerSecret, signatureBase, tokenSecret);
            oAuthParameters.Add("oauth_signature", signature);

            return oAuthParameters;
        }

        /// <summary>
        /// Gets the oauth parameters without http encode
        /// </summary>
        /// <param name="httpMethod">Http method of the request</param>
        /// <param name="url">url</param>
        /// <param name="requestToken">token</param>
        /// <param name="tokenSecret">token secret</param>
        /// <param name="verifier">verifier</param>
        /// <param name="callback">callback</param>
        /// <returns>Collection of parameter - value</returns>
        public NameValueCollection GetOAuthParametersWithoutEncode(string httpMethod, string url, string requestToken, string tokenSecret, string verifier, string callback)
        {
            var oAuthParameters = new NameValueCollection
                                            {
                                               {"oauth_consumer_key", UrlEncode(ConsumerKey)}
                                            };

            var oAuthParametersEncode = new NameValueCollection
                                            {
                                               {"oauth_consumer_key", UrlEncode(ConsumerKey)}
                                            };


            if (!string.IsNullOrEmpty(requestToken)) oAuthParameters.Add("oauth_token", requestToken);
            if (!string.IsNullOrEmpty(requestToken)) oAuthParametersEncode.Add("oauth_token", UrlEncode(requestToken));

            oAuthParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oAuthParametersEncode.Add("oauth_signature_method", "HMAC-SHA1");

            oAuthParameters.Add("oauth_timestamp", GenerateTimeStamp());
            oAuthParametersEncode.Add("oauth_timestamp", oAuthParameters["oauth_timestamp"]);

            oAuthParameters.Add("oauth_nonce", GenerateNonce());
            oAuthParametersEncode.Add("oauth_nonce", oAuthParameters["oauth_nonce"]);

            oAuthParameters.Add("oauth_version", "1.0");
            oAuthParametersEncode.Add("oauth_version", "1.0");

            if (!string.IsNullOrEmpty(verifier)) oAuthParameters.Add("oauth_verifier", verifier);
            if (!string.IsNullOrEmpty(verifier)) oAuthParametersEncode.Add("oauth_verifier", UrlEncode(verifier));

            if (!string.IsNullOrEmpty(callback)) oAuthParameters.Add("oauth_callback", callback);
            if (!string.IsNullOrEmpty(callback)) oAuthParametersEncode.Add("oauth_callback", callback);

            var signatureBase = GetSignatureBaseEncoded(httpMethod, NormalizeUrl(url), oAuthParametersEncode);
            var signature = GetSignature(UrlEncode(ConsumerSecret), signatureBase, UrlEncode(tokenSecret));
            oAuthParameters.Add("oauth_signature", signature);
            oAuthParametersEncode.Add("oauth_signature", signature);

            return oAuthParametersEncode;
        }

        /// <summary>
        /// Gets signature base
        /// </summary>
        /// <param name="httpMethod">Http method</param>
        /// <param name="url">url</param>
        /// <param name="oAuthParameters">oauth parameters</param>
        /// <returns></returns>
        private static string GetSignatureBase(string httpMethod, string url, NameValueCollection oAuthParameters)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var key in oAuthParameters.AllKeys)
                parameters.Add(key, oAuthParameters[key]);

            var normalizedParameters = NormalizeParameters(parameters);
            
            return string.Format("{0}&{1}&{2}", httpMethod, Uri.EscapeDataString(url), normalizedParameters.Replace("=", "%3D").Replace("&", "%26"));
        }

        /// <summary>
        /// Gets signature base encoded
        /// </summary>
        /// <param name="httpMethod">Http method</param>
        /// <param name="url">url</param>
        /// <param name="oAuthParameters">oauth parameters</param>
        /// <returns></returns>
        private static string GetSignatureBaseEncoded(string httpMethod, string url, NameValueCollection oAuthParameters)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var key in oAuthParameters.AllKeys)
                parameters.Add(key, oAuthParameters[key]);

            var normalizedParameters = NormalizeParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(url));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate a signature
        /// </summary>
        /// <param name="consumerSecret">Consumer secret</param>
        /// <param name="signatureBase">Signature base</param>
        /// <param name="tokenSecret">Token secret</param>
        /// <returns>the sign</returns>
        private static string GetSignature(string consumerSecret, string signatureBase, string tokenSecret)
        {
            var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(string.Concat(consumerSecret, "&", tokenSecret)));

            var data = Encoding.ASCII.GetBytes(signatureBase);
            var hashData = hmacsha1.ComputeHash(data);

            return Uri.EscapeDataString(Convert.ToBase64String(hashData));
        }

        /// <summary>
        ///  The request parameters are collected, sorted and concatenated into a normalized string:
        ///  * Parameters in the OAuth HTTP Authorization header (Authorization Header) excluding the realm parameter.
        ///  * Parameters in the HTTP POST request body (with a content-type of application/x-www-form-urlencoded).
        ///  * HTTP GET parameters added to the URLs in the query part.
        ///  The oauth_signature parameter MUST be excluded.
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        private static string NormalizeParameters(Dictionary<string, string> parameters)
        {
            var normalizedParameters = new StringBuilder();
            var sortedParameters = from ps in parameters
                                   orderby ps.Key, ps.Value
                                   select ps;

            foreach (var parameter in sortedParameters)
            {
                if (normalizedParameters.Length > 0)
                    normalizedParameters.Append("&");
                normalizedParameters.AppendFormat("{0}={1}", parameter.Key, parameter.Value);
            }

            return normalizedParameters.ToString();
        }

        /// <summary>
        /// The Signature Base String includes the request absolute URL, tying the signature to a specific 
        /// endpoint. The URL used in the Signature Base String MUST include the scheme, authority, and path, 
        /// and MUST exclude the query.
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static string NormalizeUrl(string url)
        {
            var uri = new Uri(url);
            var port = string.Empty;

            if (uri.Scheme == "http" && uri.Port != 80 ||
                uri.Scheme == "https" && uri.Port != 443 ||
                uri.Scheme == "ftp" && uri.Port != 20)
                port = string.Format(":{0}", uri.Port);

            return string.Format("{0}://{1}{2}{3}", uri.Scheme, uri.Host, port, uri.AbsolutePath);
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        internal string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <param name="nonce">A random string, to be stored by the server provided. This make each sign a single use sign</param>
        /// <param name="normalizedRequestParameters">The parameters normalized</param>
        /// <param name="normalizedUrl">The url normalized</param>
        /// <param name="timeStamp">The timestamp when the request is made</param>
        /// <returns>The signature base</returns>
        public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            normalizedUrl = null;
            normalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += $":{url.Port}";
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce">A random string, to be stored by the server provided. This make each sign a single use sign</param>
        /// <param name="normalizedRequestParameters">The parameters normalized</param>
        /// <param name="normalizedUrl">The url normalized</param>
        /// <param name="timeStamp">The timestamp when the request is made</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="nonce">A random string, to be stored by the server provided. This make each sign a single use sign</param>
        /// <param name="normalizedRequestParameters">The parameters normalized</param>
        /// <param name="normalizedUrl">The url normalized</param>
        /// <param name="timeStamp">The timestamp when the request is made</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, out normalizedUrl, out normalizedRequestParameters);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - DateTime.UnixEpoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ConsumerKey
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerSecret
        /// </summary>
        public string ConsumerSecret { get; set; }

        #endregion
    }

    /// <summary>
    /// Provides a predefined set of algorithms that are supported officially by the protocol
    /// </summary>
    public enum SignatureTypes
    {
        /// <summary>
        /// HMACSHA1 signature
        /// </summary>
        HMACSHA1,

        /// <summary>
        /// PLAINTEXT signature
        /// </summary>
        PLAINTEXT,

        /// <summary>
        /// RSASHA1 signature
        /// </summary>
        RSASHA1
    }

    /// <summary>
    /// Provides an internal structure to sort the query parameter
    /// </summary>
    internal class QueryParameter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        public QueryParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the value of the parameter
        /// </summary>
        public string Value
        {
            get;
        }
    }

    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    internal class QueryParameterComparer : IComparer<QueryParameter>
    {

        #region IComparer<QueryParameter> Members

        /// <summary>
        /// Compares two parameters
        /// </summary>
        /// <param name="x">One parameter</param>
        /// <param name="y">Another parameter</param>
        /// <returns>True if both parameters are equals</returns>
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }

        #endregion
    }
}
