using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.OAuth;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.Apiwrapper.ApiModel;
using Newtonsoft.Json;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Gnoss.Apiwrapper
{
    /// <summary>
	/// Wrapper for GNOSS Rol API
	/// </summary>
    public class RolApi : GnossApiWrapper
    {
        #region Constructors

        /// <summary>
        /// Constructor of <see cref="RolApi"/>
        /// </summary>
        /// <param name="communityShortName">Community short name which you want to use the API</param>
        /// <param name="oauth">OAuth information to sign the Api requests</param>
        public RolApi(OAuthInfo oauth, ILogHelper logHelper = null) : base(oauth, logHelper)
        {
        }

        /// <summary>
        /// Consturtor of <see cref="RolApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public RolApi(string configFilePath) : base(configFilePath)
        {
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Returns the roles of the community as well as those of the ecosystem
        /// </summary>
        public List<Rol> GetRolesComunidad()
        {
            List<Rol> list = new List<Rol>();
            try
            {
                string url = $"{ApiUrl}/roles/get-roles-community?community_short_name={CommunityShortName}";
                string response = WebRequest("GET", url, acceptHeader: "application/json");
                list= JsonConvert.DeserializeObject<List<Rol>>(response);
                WebRequest($"GET", url);
                if (list.Count > 0)
                {
                    Log.Debug($"Community roles have been obtained correctly from communityShortName: {CommunityShortName}");
                }
                else
                {
                    Log.Debug($"Community roles could not be obtained from communityShortName: {CommunityShortName} or roles do not exist yet");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Cannot get community roles '{CommunityShortName}' \r\n {ex.Message}");
                throw;
            }
            return list;
        }

        /// <summary>
        /// Returns the roles of the community as well as those of the ecosystem
        /// <param name="communityId">Community identifier</param>
        /// </summary>
        public List<Rol> GetRolesComunidad(Guid communityId)
        {
            List<Rol> list = new List<Rol>();
            try
            {
                string url = $"{ApiUrl}/roles/get-roles-community?community_id={communityId}";
                string response = WebRequest("GET", url, acceptHeader: "application/json");
                list = JsonConvert.DeserializeObject<List<Rol>>(response);
                WebRequest($"GET", url);
                if (list.Count > 0)
                {
                    Log.Debug($"Community roles have been obtained correctly");
                }
                else
                {
                    Log.Debug($"Community roles could not be obtained from community id: {communityId} or roles do not exist yet");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Cannot get community roles'{communityId}' \r\n {ex.Message}");
                throw;
            }
            return list ;
        }

        #endregion
    }
}
