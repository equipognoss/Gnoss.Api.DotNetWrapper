﻿using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Exceptions;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.OAuth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gnoss.ApiWrapper
{

    /// <summary>
    /// Wrapper for GNOSS user API
    /// </summary>
    public class UserApi : GnossApiWrapper
    {

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="UserApi"/>
        /// </summary>
        /// <param name="communityShortName">Community short name which you want to use the API</param>
        /// <param name="oauth">OAuth information to sign the Api requests</param>
        public UserApi(OAuthInfo oauth, ILogHelper logHelper = null) : base(oauth, logHelper)
        {
        }

        /// <summary>
        /// Consturtor of <see cref="UserApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public UserApi(string configFilePath) : base(configFilePath)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get the data a user by user short name
        /// </summary>
        /// <param name="userShortName">User short name you want to get data</param>
        /// <returns>User data that has been requested</returns>
        public User GetUserByShortName(string userShortName)
        {
            User user = null;
            try
            {
                string url = $"{ApiUrl}/user/get-by-short-name?user_short_name={userShortName}&community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");
                user = JsonConvert.DeserializeObject<User>(response);

                if (user != null)
                {
                    Log.Debug($"The user {user.name} {user.last_name} has been obtained successfully");
                }
                else
                {
                    Log.Error($"Couldn't get the user {userShortName}: \r\n {response}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't get the user {userShortName}: \r\n {ex.Message}");
                throw;
            }
            return user;
        }

        /// <summary>
        /// Get the data a user by user identifier
        /// </summary>
        /// <param name="userId">User identifier you want to get data</param>
        /// <returns>User data that has been requested</returns>
        public User GetUserById(Guid userId)
        {
            User user = null;
            try
            {
                string url = $"{ApiUrl}/user/get-by-id?user_ID={userId}&community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");
                user = JsonConvert.DeserializeObject<User>(response);

                if (user != null)
                {
                    Log.Debug($"The user {user.name} {user.last_name} has been obtained successfully");
                }
                else
                {
                    Log.Error($"Couldn't get the user {userId}: \r\n {response}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't get the user {userId}: \r\n {ex.Message}");
                throw;
            }
            return user;
        }

        /// <summary>
        /// Get the data a user by user email
        /// </summary>
        /// <param name="email">User email you want to get data</param>
        /// <returns>User data that has been requested</returns>
        public User GetUserByEmail(string email)
        {
            User user = null;
            try
            {
                string url = $"{ApiUrl}/user/get-by-email?email={email}&community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");
                user = JsonConvert.DeserializeObject<User>(response);

                if (user != null)
                {
                    Log.Debug($"The user {user.name} {user.last_name} has been obtained successfully");
                }
                else
                {
                    Log.Error($"Couldn't get the user {email}: \r\n {response}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't get the user {email}: \r\n {ex.Message}");
                throw;
            }
            return user;
        }


        /// <summary>
        /// Validate the user password
        /// </summary>
        /// <param name="user">User email</param>
        /// <param name="password">password</param>
        /// <returns>True if it's a valid password</returns>
        public bool ValidatePassword(string user, string password)
        {
            bool validPassword = false;

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    string url = $"{ApiUrl}/user/validate-password";

                    ParamsLoginPassword model = new ParamsLoginPassword() { login = user, password = password };

                    string postData = JsonConvert.SerializeObject(model);

                    string respuesta = WebRequest("POST", url, postData, "application/json");
                    validPassword = JsonConvert.DeserializeObject<bool>(respuesta);

                    if (validPassword)
                    {
                        Log.Debug($"The password for the user {user} is correct");
                    }
                    else
                    {
                        Log.Debug($"The password for the user {user} isn't correct");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    throw;
                }
            }
            else
            {
                Log.Error("The user and the password can't be null or empty");
            }

            return validPassword;
        }

        /// <summary>
        /// Gets the position of an organization profile in a community
        /// </summary>
        /// <param name="profileId">Organization profile ID</param>
        /// <returns>Position of the organization profile in a community</returns>
        public string GetProfileRoleInOrganization(Guid profileId)
        {
            string profileRol = "";
            try
            {
                string url = $"{ApiUrl}/user/get-profile-role-in-organization?profile_id={profileId}&community_short_name={CommunityShortName}";

                profileRol = WebRequest("GET", url)?.Trim('"');

                Log.Debug($"The profile role of {profileId} in {CommunityShortName} is {profileRol}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the profile role of {profileId}: {ex.Message}");
                throw;
            }

            return profileRol;
        }

        /// <summary>
        /// Create a user awaiting activation
        /// </summary>
        /// <param name="user">User data you want to create</param>
        /// <returns>User data</returns>
        public User CreateUserWaitingForActivate(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            User createdUser = null;

            try
            {
                string url = $"{ApiUrl}/user/create-user-waiting-for-activate";

                string response = WebRequest("POST", url, json, "application/json");
                createdUser = JsonConvert.DeserializeObject<User>(response);

                if (createdUser != null)
                {
                    Log.Debug($"User created: {createdUser.name} {createdUser.last_name}");
                }
                else
                {
                    Log.Error($"Error creating user {json}: {response}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating user {json}: \r\n{ex.Message}");
                throw;
            }

            return createdUser;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="user">User data you want to create</param>
        /// <returns>User data</returns>
        public User CreateUser(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            User createdUser = null;
            try
            {
                string url = $"{ApiUrl}/user/create-user";

                string response = WebRequest("POST", url, json, "application/json");
                createdUser = JsonConvert.DeserializeObject<User>(response);

                if (createdUser != null)
                {
                    Log.Debug($"User created: {createdUser.name} {createdUser.last_name}");
                }
                else
                {
                    Log.Error($"Error creating user {json}: {response}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating user {json}: \r\n{ex.Message}");
                throw;
            }

            return (createdUser);

        }

        /// <summary>
        /// Veerify User
        /// </summary>
        /// <param name="loginOrEmail">Login o email of the user</param>
        public void VerificarUsuario(string loginOrEmail)
        {
            try
            {
                string url = $"{ApiUrl}/user/verify-user?login={loginOrEmail}";

                WebRequest("POST", url, "", "application/json");

            }
            catch (Exception ex)
            {
                Log.Error($"Error validation user {loginOrEmail} from the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// Gets the URL to recover the password of a user
        /// </summary>
        /// <param name="loginOrEmail">Login o email of the user</param>
        /// <returns>URL to recover the password of a user</returns>
        public string GenerateForgottenPasswordUrl(string loginOrEmail)
        {
            string link = string.Empty;

            if (!string.IsNullOrEmpty(loginOrEmail))
            {
                try
                {
                    string url = $"{ApiUrl}/user/generate-forgotten-password-url?login={loginOrEmail}&community_short_name={CommunityShortName}";
                    link = WebRequest("GET", url)?.Trim('"');

                    Log.Debug($"Forgotten password url generated {link}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error generating forgotten password url for user {loginOrEmail}: {ex.Message}");
                    throw;
                }
            }

            return link;
        }

        /// <summary>
        /// Modify a user
        /// </summary>
        /// <param name="user">User data</param>
        public void ModifyUser(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            try
            {
                string url = $"{ApiUrl}/user/modify-user";
                WebRequest("POST", url, json, "application/json");

                Log.Debug($"User modify successfully {json}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error trying to modify user {json}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a user from a community
        /// </summary>
        /// <param name="userShortName">User short name to delete</param>
        public void DeleteUserFromCommunity(string userShortName)
        {
            try
            {
                string url = $"{ApiUrl}/user/delete-user-from-community";

                ParamsUserCommunity model = new ParamsUserCommunity() { user_short_name = userShortName, community_short_name = CommunityShortName };

                string postData = JsonConvert.SerializeObject(model);

                WebRequest("POST", url, postData, "application/json");

                Log.Debug($"User {userShortName} deleted successfully from the community {CommunityShortName}");

            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting user {userShortName} from the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId">User identifier to delete</param>
        public void DeleteUser(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/delete-user?user_ID={userId}";

                WebRequestPostWithJsonObject(url, userId);

                Log.Debug($"User {userId} deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting user {userId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a user to an organization
        /// </summary>
        /// <param name="userId">User ID to delete</param>
        /// <param name="organizationShortName">Short name of the organization</param>
        /// <param name="position">Position in the organization</param>
        /// <param name="communitiesShortNames">Short names of the communities that will be included</param>
        public void AddUserToOrganization(Guid userId, string organizationShortName, string position, List<string> communitiesShortNames)
        {
            try
            {
                string url = $"{ApiUrl}/user/add-user-to-organization";

                ParamsAddUserOrg model = new ParamsAddUserOrg() { user_id = userId, position = position, organization_short_name = organizationShortName, communities_short_names = communitiesShortNames };
                string postData = JsonConvert.SerializeObject(model);

                WebRequest("POST", url, postData, "application/json");

                Log.Debug($"User {userId} added successfully to organization {organizationShortName} in the communities: {string.Join(",", communitiesShortNames)}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding user {userId} to organization {organizationShortName} in the communities: {string.Join(",", communitiesShortNames)}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Change visibility of a user in a communities
        /// </summary>
        /// <param name="pUsuarioID">User's ID</param>
        /// <param name="pComunidadesID">Id´s of communities</param>
        /// <param name="pvisibilidad">Visibility of user</param>
        public void ChangeVisibilityUserCommunities(Guid userId, List<Guid> pComunidadesID, bool pvisibilidad = false)
        {
            try
            {
                string url = $"{ApiUrl}/user/change-user-visibility";

                ParamsChangeVisibility model = new ParamsChangeVisibility() { user_id = userId, communities_Id = pComunidadesID, visibility = pvisibilidad };
                string postData = JsonConvert.SerializeObject(model);

                WebRequest("POST", url, postData, "application/json");

                Log.Debug($"User {userId} change visibility in the communities: {string.Join(",", pComunidadesID)}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding user {userId} change visibility in the communities: {string.Join(",", pComunidadesID)}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a user to organization groups
        /// </summary>
        /// <param name="userId">User ID </param>
        /// <param name="organizationShortName">Short name of the organization</param>
        /// <param name="groupsShortNames">Short names of the organization groups that will be included</param>
        /// <returns></returns>
        public void AddUserToOrganizationGroup(Guid userId, string organizationShortName, List<string> groupsShortNames)
        {
            try
            {
                string url = $"{ApiUrl}/user/add-user-to-organization-group";

                ParamsAddUserOrgGroups model = new ParamsAddUserOrgGroups() { user_id = userId, organization_short_name = organizationShortName, groups_short_names = groupsShortNames };

                string postData = JsonConvert.SerializeObject(model);

                WebRequest("POST", url, postData, "application/json");

                Log.Debug($"User {userId} added successfully to organization {organizationShortName} in the groups: {string.Join(",", groupsShortNames)}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding user {userId} to organization {organizationShortName} in the groups: {string.Join(",", groupsShortNames)}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the data a user by user id
        /// </summary>
        /// <param name="listaIds">List of users ID´s</param>
        /// <returns>List with the users </returns>
        public Dictionary<Guid, Userlite> GetUsersByIds(List<Guid> listaIds)
        {
            Dictionary<Guid, Userlite> users = null;
            try
            {
                string url = $"{ApiUrl}/user/get-users-by-id";

                string result = WebRequestPostWithJsonObject(url, listaIds);

                users = JsonConvert.DeserializeObject<Dictionary<Guid, Userlite>>(result);

                Log.Debug($"Users obtained by Ids");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the users", ex.Message);
                throw;
            }
            return users;
        }

        /// <summary>
        /// Gets the modified users from a datetime in a community
        /// </summary>
        /// <param name="communityShortName">Community short name</param>
        /// <param name="searchDate">Start search datetime in ISO8601 format string ("yyyy-MM-ddTHH:mm:ss.mmm" (no spaces) OR "yyyy-MM-ddTHH:mm:ss.mmmZ" (no spaces))</param>
        /// <returns>List with the modified users identifiers</returns>
        public List<Guid> GetModifiedUsersFromDate(string searchDate)
        {
            List<Guid> users = null;
            try
            {
                DateTime fecha = DateTime.Now;
                bool esFecha = DateTime.TryParse(searchDate, out fecha);

                if (!esFecha)
                {
                    throw new Exception($"The search date string is not in the ISO8601 format {searchDate}");
                }

                string url = $"{ApiUrl}/user/get-modified-users?community_short_name={CommunityShortName}&search_date={searchDate}";

                string response = WebRequest("GET", url);

                users = JsonConvert.DeserializeObject<List<Guid>>(response);

                Log.Debug($"Users obtained of the community {CommunityShortName} from date {searchDate}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the users of {CommunityShortName} from date {searchDate}", ex.Message);
                throw;
            }
            return users;
        }

        /// <summary>
        /// Gets the short name of the communities that manages the user.
        /// </summary>
        /// <param name="login">Login of the user</param>
        /// <returns>Short name of the communities that manages the user</returns>
        public List<string> GetManagedCommunity(string login)
        {
            List<string> communities = null;
            try
            {
                string url = $"{ApiUrl}/user/get-admin-communities?login={login}";

                string response = WebRequest("GET", url);

                communities = JsonConvert.DeserializeObject<List<string>>(response);

                Log.Debug($"Communities obtained for user {login}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the communities of {login}", ex.Message);
                throw;
            }
            return communities;
        }

        /// <summary>
        /// Gets the novelties of the user from a datetime
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="searchDate">Start search datetime in ISO8601 format string ("yyyy-MM-ddTHH:mm:ss.mmm" (no spaces) OR "yyyy-MM-ddTHH:mm:ss.mmmZ" (no spaces))</param>
        /// <returns>UserNoveltiesModel with the novelties of the user from the search date</returns>
        public UserNoveltiesModel GetUserNoveltiesFromDate(Guid userId, string searchDate)
        {
            UserNoveltiesModel user = null;
            try
            {
                if (searchDate.Contains(" ") || !searchDate.Contains("T"))
                {
                    Log.Error($"The search date string is not in the ISO8601 format {searchDate}");
                    return null;
                }

                string url = $"{ApiUrl}/user/get-user-novelties?user_id={userId}&community_short_name={CommunityShortName}&search_date={searchDate}";
                string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                user = JsonConvert.DeserializeObject<UserNoveltiesModel>(response);

                if (user != null)
                {
                    Log.Debug($"Obtained the user {userId} of the community {CommunityShortName} from the date {searchDate}");
                }
                else
                {
                    Log.Debug($"The user {userId} could not be obtained of the community {CommunityShortName} from the date {searchDate}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the user {userId} of the community {CommunityShortName} from the date {searchDate}", ex.Message);
                throw;
            }
            return user;
        }

        /// <summary>
        /// Gets the UserID form Cookie
        /// </summary>
        /// <param name="pCookie">cookie</params>
        /// <returns>UserID from cookie</returns>
        public Guid GetUserIDFromCookie(string pCookie)
        {
            Guid userID = Guid.Empty;
            try
            {
                string url = $"{ApiUrl}/user/get-user-cookie?cookie={pCookie}";
                string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                userID = JsonConvert.DeserializeObject<Guid>(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the user from the cookie", ex.Message);
                throw;
            }
            return userID;
        }


        /// <summary>
        /// Gets a single use token or a long live token to use it in a login action. 
        /// To login a user, the user must be redirected to the Login service URL, at the page externallogin.aspx
        /// with the parameters ?loginToken={thistoken}, webToken={webToken}, redirect={urlRedirect}
        /// The webToken can be accessed by @Session["tokenCookie"] in the views. 
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="longLiveToken">True if the token is going to be used more than one time</param>
        /// <returns>Token</returns>
        public string GetLoginTokenForEmail(string email, bool longLiveToken = false)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new GnossAPIArgumentException("The email can't be null or empty", "email");
            }
            else
            {
                string url = $"{ApiUrl}/user/generate-login-token-for-email?email={email}&longLiveToken={longLiveToken}";
                string result = WebRequest($"POST", url, acceptHeader: "application/json");

                string token = JsonConvert.DeserializeObject<string>(result);

                return token;
            }
        }

        /// <summary>
        /// Gets the email asociated to a token. 
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="deleteSingleUseToken">True if it's a single use token, to delete it from the database</param>
        /// <returns>Token</returns>
        public string GetEmailByToken(Guid token, bool deleteSingleUseToken = false)
        {
            if (token.Equals(Guid.Empty))
            {
                throw new GnossAPIArgumentException("The token can't be an empty guid", "token");
            }
            else
            {
                string url = $"{ApiUrl}/user/get-email-by-token?token={token}&deleteSingleUseToken={deleteSingleUseToken}";
                string result = WebRequest($"GET", url, acceptHeader: "application/json");

                string email = JsonConvert.DeserializeObject<string>(result);

                return email;
            }
        }

        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="userId">User's identifier</param>
        public void BlockUser(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/block?user_id={userId}";
                WebRequestPostWithJsonObject(url, userId);
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The user '{userId}' could not be blocked");
                throw;
            }
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="userId">User's identifier</param>
        public void UnblockUser(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/unblock?user_id={userId}";
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The user '{userId}' could not be unblocked");
                throw;
            }
        }

        /// <summary>
        /// Adds a social network login to a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="socialNetworkUserId">Social network user's identifier</param>
        /// <param name="socialNetwork">Social network (Facebook, twitter, instagram...)</param>
        public void AddSocialNetworkLogin(Guid userId, string socialNetworkUserId, string socialNetwork)
        {
            try
            {
                string url = $"{ApiUrl}/user/add-social-network-login?user_id={userId}&social_network_user_id={HttpUtility.UrlEncode(socialNetworkUserId)}&social_network={HttpUtility.UrlEncode(socialNetwork)}";
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The social network login {socialNetworkUserId} at {socialNetwork} could not be added to user '{userId}'");
                throw;
            }
        }

        /// <summary>
        /// Modifies a social network login to a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="socialNetworkUserId">New social network user's identifier</param>
        /// <param name="socialNetwork">Social network (Facebook, twitter, instagram...)</param>
        public void ModifySocialNetworkLogin(Guid userId, string socialNetworkUserId, string socialNetwork)
        {
            try
            {
                string url = $"{ApiUrl}/user/modify-social-network-login?user_id={userId}&social_network_user_id={HttpUtility.UrlEncode(socialNetworkUserId)}&social_network={HttpUtility.UrlEncode(socialNetwork)}";
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The social network login {socialNetworkUserId} at {socialNetwork} could not be added to user '{userId}'");
                throw;
            }
        }

        /// <summary>
        /// Gets a user by a social network login
        /// </summary>
        /// <param name="socialNetworkUserId">Social network user's identifier</param>
        /// <param name="socialNetwork">Social network (Facebook, twitter, instagram...)</param>
        /// <returns></returns>
        public Guid GetUserBySocialNetworkLogin(string socialNetworkUserId, string socialNetwork)
        {
            Guid user_id;

            try
            {
                string url = $"{ApiUrl}/user/get-user_id-by-social-network-login?social_network_user_id={HttpUtility.UrlEncode(socialNetworkUserId)}&social_network={HttpUtility.UrlEncode(socialNetwork)}";
                string result = WebRequest($"GET", url, acceptHeader: "application/json");

                user_id = JsonConvert.DeserializeObject<Guid>(result);
            }
            catch (System.Exception)
            {
                Log.Error($"The social network login {socialNetworkUserId} at {socialNetwork} could not be found.");
                throw;
            }

            return user_id;
        }

        /// <summary>
        /// Checks if a user id in a social network exists in the system
        /// </summary>
        /// <param name="socialNetworkUserId">Social network user's identifier</param>
        /// <param name="socialNetwork">Social network (Facebook, twitter, instagram...)</param>
        /// <returns></returns>
        public bool ExistsSocialNetworkLogin(string socialNetworkUserId, string socialNetwork)
        {
            try
            {
                string url = $"{ApiUrl}/user/exists-social-network-login?social_network_user_id={HttpUtility.UrlEncode(socialNetworkUserId)}&social_network={HttpUtility.UrlEncode(socialNetwork)}";
                string result = WebRequest($"GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<bool>(result);
            }
            catch (System.Exception)
            {
                Log.Error($"The social network login {socialNetworkUserId} at {socialNetwork} could not be found.");
                throw;
            }
        }

        /// <summary>
        /// Checks if the emails already exists in the database
        /// </summary>
        /// <param name="emails">Email list that you want to check</param>
        /// <returns>Email list that already exists in the database</returns>
        /// <example>POST user/exists-email-in-database</example>
        public List<string> ExistsEmails(List<string> emails)
        {
            try
            {
                string url = $"{ApiUrl}/user/exists-email-in-database";
                string result = WebRequestPostWithJsonObject(url, emails);

                return JsonConvert.DeserializeObject<List<string>>(result);
            }
            catch (System.Exception)
            {
                Log.Error($"Impossible to check the emails {string.Join(",", emails)}");
                throw;
            }
        }

        /// <summary>
        /// Return short path of the personal profile of the user
        /// </summary>
        /// <param name="user_id">Identifier of the user</param>
        /// <returns>True or false if the user has or not photo</returns>
        /// <example>POST user/get-user-photo</example>
        public string GetUserPhoto(Guid user_id)
        {
            try
            {
                string url = $"{ApiUrl}/user/get-user-photo";
                return WebRequestPostWithJsonObject(url, user_id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the user's groups in a community
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns></returns>
        public List<string> GetGroupsPerCommunity(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/get-groups-per-community?user_id={userId}&community_short_name={CommunityShortName}";
                string result = WebRequest($"GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<List<string>>(result);
            }
            catch (System.Exception)
            {
                Log.Error($"Impossible to get groups of {userId} from community  {CommunityShortName}. ");
                throw;
            }
        }


        /// <summary>
        /// Gets a social network login by a user id
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="socialNetwork">Social network short name</param>
        /// <returns>Social network login of the user</returns>
        public string GetSocialNetworkLoginByUserId(string socialNetwork, Guid userId)
        {
            string socialNetworkLogin;

            try
            {
                string url = $"{ApiUrl}/user/get-social-network-login-by-user_id?user_id={userId}&social_network={HttpUtility.UrlEncode(socialNetwork)}";
                socialNetworkLogin = WebRequest($"GET", url, acceptHeader: "application/json")?.Trim('"');
            }
            catch (System.Exception)
            {
                Log.Error($"The user {userId} at {socialNetwork} could not be found.");
                throw;
            }

            return socialNetworkLogin;
        }

        /// <summary>
        /// Adds the Community CMS Admin rol to a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        public void AddCmsAdminRolToUser(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/add-permission?user_id={userId}&community_short_name={CommunityShortName}&admin_page_type={(short)AdministrationPageType.Page}";
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The community CMS admin rol could not be added to user '{userId}'");
                throw;
            }
        }

        /// <summary>
        /// Removes the Community CMS Admin rol to a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        public void RemoveCmsAdminRolToUser(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/user/remove-permission?user_id={userId}&community_short_name={CommunityShortName}&admin_page_type={(short)AdministrationPageType.Page}";
                WebRequest($"POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The community CMS admin rol could not be removed from user '{userId}'");
                throw;
            }
        }

        #endregion
    }
}
