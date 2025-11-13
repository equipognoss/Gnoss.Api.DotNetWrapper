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
        public List<Rol> GetRolesCommunity()
        {
            List<Rol> list = new List<Rol>();
            try
            {
                string url = $"{ApiUrl}/roles/get-roles-community?community_short_name={CommunityShortName}";
                string response = WebRequest("GET", url, acceptHeader: "application/json");
                list = JsonConvert.DeserializeObject<List<Rol>>(response);
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
        public List<Rol> GetRolesCommunity(Guid communityId)
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
            return list;
        }

        /// <summary>
        /// Add a new role to the community. The parameter pSemanticResources is a dictionary to which the ontology guid is passed. 
        /// and a class with the permissions.
        /// </summary>
        /// <param name="pNombre">Role name</param>
        /// <param name="pDescripcion">Role description</param>
        /// <param name="pAmbito">Role scope</param>
        /// <param name="pPermisos">Community permissions (binary string)</param>
        /// <param name="pPermisosRecursos">Resource permissions (binary string)</param>
        /// <param name="pPermisosEcosistema">Ecosystem permissions (binary string)</param>
        /// <param name="pPermisosContenidos">Content permissions (binary string)</param>
        /// <param name="pPermisosRecursosSemanticos">Semantic resource permissions (JSON)</param>
        public void AddRoleCommunity(
            string pNombre,
            string pDescripcion,
            AmbitoRol pAmbito,
            PermisosDTO pPermisos,
            PermisosRecursosDTO pPermisosRecursos,
            PermisosEcosistemaDTO pPermisosEcosistema,
            PermisosContenidosDTO pPermisosContenidos,
            Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos)
        {
            try
            {
                string url = $"{ApiUrl}/roles/add-role-community";
                ParamsRoleCommunity model = new ParamsRoleCommunity()
                {
                    community_short_name = CommunityShortName,
                    pNombre = pNombre,
                    pDescripcion = pDescripcion,
                    pAmbito = pAmbito,
                    pPermisos = pPermisos,
                    pPermisosRecursos = pPermisosRecursos,
                    pPermisosEcosistema = pPermisosEcosistema,
                    pPermisosContenidos = pPermisosContenidos,
                    pPermisosRecursosSemanticos = pPermisosRecursosSemanticos
                };

                WebRequestPostWithJsonObject(url, model);

            }
            catch (Exception ex)
            {
                Log.Error($"Error adding role to community '{CommunityShortName}':  \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a new role to the community. The parameter pSemanticResources is a dictionary to which the ontology guid is passed. 
        /// and a class with the permissions.
        /// </summary>
        /// <param name="community_id">Community ID (optional if community_short_name is provided)</param>
        /// <param name="pNombre">Role name</param>
        /// <param name="pDescripcion">Role description</param>
        /// <param name="pAmbito">Role scope</param>
        /// <param name="pPermisos">Community permissions (binary string)</param>
        /// <param name="pPermisosRecursos">Resource permissions (binary string)</param>
        /// <param name="pPermisosEcosistema">Ecosystem permissions (binary string)</param>
        /// <param name="pPermisosContenidos">Content permissions (binary string)</param>
        /// <param name="pPermisosRecursosSemanticos">Semantic resource permissions (JSON)</param>
        public void AddRoleCommunity(
            Guid community_id,
            string pNombre,
            string pDescripcion,
            AmbitoRol pAmbito,
            PermisosDTO pPermisos,
            PermisosRecursosDTO pPermisosRecursos,
            PermisosEcosistemaDTO pPermisosEcosistema,
            PermisosContenidosDTO pPermisosContenidos,
            Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos)
        {
            try
            {
                string url = $"{ApiUrl}/roles/add-role-community";
                ParamsRoleCommunity model = new ParamsRoleCommunity()
                {
                    community_id = community_id,
                    pNombre = pNombre,
                    pDescripcion = pDescripcion,
                    pAmbito = pAmbito,
                    pPermisos = pPermisos,
                    pPermisosRecursos = pPermisosRecursos,
                    pPermisosEcosistema = pPermisosEcosistema,
                    pPermisosContenidos = pPermisosContenidos,
                    pPermisosRecursosSemanticos = pPermisosRecursosSemanticos
                };

                WebRequestPostWithJsonObject(url, model);

            }
            catch (Exception ex)
            {
                Log.Error($"Error adding role to community '{community_id}':  \r\n {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Add a new role to the community. The parameter pSemanticResources is a dictionary to which the ontology guid is passed. 
        /// and a class with the permissions. Keep in mind that you must define all the roles, if one already existed it will be deactivated by default.
        /// </summary>
        /// <param name="role_id">Identifier of the role to modify</param>
        /// <param name="community_id">Community ID (optional if community_short_name is provided)</param>
        /// <param name="pNombre">Role name</param>
        /// <param name="pDescripcion">Role description</param>
        /// <param name="pAmbito">Role scope</param>
        /// <param name="pPermisos">Community permissions (binary string)</param>
        /// <param name="pPermisosRecursos">Resource permissions (binary string)</param>
        /// <param name="pPermisosEcosistema">Ecosystem permissions (binary string)</param>
        /// <param name="pPermisosContenidos">Content permissions (binary string)</param>
        /// <param name="pPermisosRecursosSemanticos">Semantic resource permissions (JSON)</param>
        public void SetRoleCommunity(
            Guid role_id,
            Guid community_id,
            string pNombre,
            string pDescripcion,
            AmbitoRol pAmbito,
            PermisosDTO pPermisos,
            PermisosRecursosDTO pPermisosRecursos,
            PermisosEcosistemaDTO pPermisosEcosistema,
            PermisosContenidosDTO pPermisosContenidos,
            Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos)
        {
            try
            {
                string url = $"{ApiUrl}/roles/set-role-community";
                ParamsRoleCommunity model = new ParamsRoleCommunity()
                {
                    rol_id = role_id,
                    community_id = community_id,
                    pNombre = pNombre,
                    pDescripcion = pDescripcion,
                    pAmbito = pAmbito,
                    pPermisos = pPermisos,
                    pPermisosRecursos = pPermisosRecursos,
                    pPermisosEcosistema = pPermisosEcosistema,
                    pPermisosContenidos = pPermisosContenidos,
                    pPermisosRecursosSemanticos = pPermisosRecursosSemanticos
                };

                WebRequestPostWithJsonObject(url, model);

            }
            catch (Exception ex)
            {
                Log.Error($"Error adding role to community '{community_id}':  \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a new role to the community. The parameter pSemanticResources is a dictionary to which the ontology guid is passed. 
        /// and a class with the permissions. Keep in mind that you must define all the roles, if one already existed it will be deactivated by default.
        /// </summary>
        /// <param name="role_id">Identifier of the role to modify</param>
        /// <param name="pNombre">Role name</param>
        /// <param name="pDescripcion">Role description</param>
        /// <param name="pAmbito">Role scope</param>
        /// <param name="pPermisos">Community permissions (binary string)</param>
        /// <param name="pPermisosRecursos">Resource permissions (binary string)</param>
        /// <param name="pPermisosEcosistema">Ecosystem permissions (binary string)</param>
        /// <param name="pPermisosContenidos">Content permissions (binary string)</param>
        /// <param name="pPermisosRecursosSemanticos">Semantic resource permissions (JSON)</param>
        public void SetRoleCommunity(
            Guid role_id,
            string pNombre,
            string pDescripcion,
            AmbitoRol pAmbito,
            PermisosDTO pPermisos,
            PermisosRecursosDTO pPermisosRecursos,
            PermisosEcosistemaDTO pPermisosEcosistema,
            PermisosContenidosDTO pPermisosContenidos,
            Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos)
        {
            try
            {
                string url = $"{ApiUrl}/roles/set-role-community";
                ParamsRoleCommunity model = new ParamsRoleCommunity()
                {
                    rol_id = role_id,
                    community_short_name = CommunityShortName,
                    pNombre = pNombre,
                    pDescripcion = pDescripcion,
                    pAmbito = pAmbito,
                    pPermisos = pPermisos,
                    pPermisosRecursos = pPermisosRecursos,
                    pPermisosEcosistema = pPermisosEcosistema,
                    pPermisosContenidos = pPermisosContenidos,
                    pPermisosRecursosSemanticos = pPermisosRecursosSemanticos
                };

                WebRequestPostWithJsonObject(url, model);

            }
            catch (Exception ex)
            {
                Log.Error($"Error adding role to community '{CommunityShortName}':  \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a role from the community
        /// </summary>
        /// <param name="role_id">Identifier of the role to delete</param>
        public void DeleteRoleCommunity(Guid role_id)
        {
            try
            {
                string url = $"{ApiUrl}/roles/delete-role-community?community_short_name={CommunityShortName}&rol_id={role_id}";
                WebRequest("DELETE", url, acceptHeader: "application/json");
                Log.Debug($"Role '{role_id}' has been successfully deleted from community '{CommunityShortName}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting role '{role_id}' from community '{CommunityShortName}': \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a role from the community using its ID
        /// </summary>
        /// <param name="community_id">Community identifier</param>
        /// <param name="role_id">Identifier of the role to delete</param>
        public void DeleteRoleCommunity(Guid community_id, Guid role_id)
        {
            try
            {
                string url = $"{ApiUrl}/roles/delete-role-community?community_id={community_id}&rol_id={role_id}";
                WebRequest("DELETE", url, acceptHeader: "application/json");
                Log.Debug($"Role '{role_id}' has been successfully deleted from community '{community_id}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting role '{role_id}' from community '{community_id}': \r\n {ex.Message}");
                throw;
            }
        }


        #endregion
    }
}





