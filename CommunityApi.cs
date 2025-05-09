using Gnoss.ApiWrapper.ApiModel;
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
    /// Wrapper for GNOSS community API
    /// </summary>
    public class CommunityApi : GnossApiWrapper
    {

        #region Members

        private List<ThesaurusCategory> _communityCategories;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="CommunityApi"/>
        /// </summary>
        /// <param name="communityShortName">Community short name which you want to use the API</param>
        /// <param name="oauth">OAuth information to sign the Api requests</param>
        public CommunityApi(OAuthInfo oauth, ILogHelper logHelper = null) : base(oauth, logHelper)
        {
        }

        /// <summary>
        /// Consturtor of <see cref="CommunityApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public CommunityApi(string configFilePath) : base(configFilePath)
        {
        }

        /// <summary>
        /// Consturtor of <see cref="CommunityApi"/>
        /// </summary>        
        public CommunityApi() : base() { }

        #endregion

        #region Methods

        #region Overrided methods

        /// <summary>
        /// Load the basic parameters for the API
        /// </summary>
        protected override void LoadApi()
        {
            if (_communityCategories != null)
            {
                _communityCategories = LoadCategories(CommunityShortName);
            }
        }

        #endregion

        #region Private methods

        internal List<ThesaurusCategory> LoadCategories(string community_short_name)
        {
            string url = $"{ApiUrl}/community/get-categories?community_short_name={community_short_name}";

            string response = WebRequest("GET", url, acceptHeader: "application/json");
            List<ThesaurusCategory> communityCategoriesWithoutHierarchy = JsonConvert.DeserializeObject<List<ThesaurusCategory>>(response);

            Log.Debug($"Loaded the categories of the communtiy {community_short_name}");

            LoadChildrenCategories(communityCategoriesWithoutHierarchy);

            return communityCategoriesWithoutHierarchy;
        }

        private void LoadChildrenCategories(List<ThesaurusCategory> categories)
        {
            foreach (ThesaurusCategory category in categories)
            {
                if (!category.parent_category_id.Equals(Guid.Empty))
                {
                    categories.Find(cat => cat.category_id == category.parent_category_id).Children.Add(category);
                }
            }
        }

        #endregion

        #region Public methods


        /// <summary>
        /// Create a community
        /// </summary>
        /// <param name="communityName">Community name</param>
        /// <param name="communityShortName">Short name of the community</param>
        /// <param name="description">Description of the community</param>
        /// <param name="tagList">Tags of the community</param>
        /// <param name="type">Type of the community</param>
        /// <param name="accessType">Access type of the community</param>
        /// <param name="parentCommunityShortName">Parent community short name of the community</param>
        /// <param name="administratorUserId">Admin ID of the community</param>
        /// <param name="organizationShortName">Admin organization short name of the community</param>
        /// <param name="logo">Logo of the community</param>
        public void CreateCommunity(string communityName, string communityShortName, string description, List<string> tagList, short type, short accessType, string parentCommunityShortName, Guid administratorUserId, string organizationShortName, byte[] logo = null, string domain = null)
        {
            string tags = StringHelper.UrlEncoderUTF8(string.Join(",", tagList));

            CommunityModel community = new CommunityModel() { community_name = communityName, community_short_name = communityShortName, description = description, tags = tags, type = type, access_type = accessType, parent_community_short_name = parentCommunityShortName, admin_id = administratorUserId, organization_short_name = organizationShortName, logo = logo, domain = domain };

            CreateCommunity(community);
        }
        /// <summary>
        /// Modify Logo
        /// </summary>
        /// <param name="logo"> Logo of the community</param>
        /// <param name="communityName">Community name</param>
        public void ModifyLogo(EditCommunityImageModel pEditCommunityImageModel)
        {
            string json = null;
            try
            {
                string url = $"{ApiUrl}/community/modify-community-image";

                WebRequestPostWithJsonObject(url, pEditCommunityImageModel);
                
                Log.Debug($"Modify Logo {json}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error Modify Logo {json}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a community
        /// </summary>
        /// <param name="communityShortName">Short name of the community</param>
        /// <param name="parentCommunityShortName">Parent community short name of the community</param>
        /// <param name="administratorUserId">Admin ID of the community</param>
        public void VincularComunidadPadre(string communityShortName, string parentCommunityShortName, Guid administratorUserId)
        {
            LinkParentCommunityModel linkparent = new LinkParentCommunityModel() { short_name = communityShortName, parent_community_short_name = parentCommunityShortName, admin_id = administratorUserId };

            LinkParentCommunity(linkparent);
        }

        /// <summary>
        /// Create a category
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="communityShortName">Community short name</param>
        /// <param name="parentCategoryID">Parent category ID</param>
        /// <param name="categoryImage">Category image</param>
        public Guid CreateCategory(string categoryName, Guid? parentCategoryID, byte[] categoryImage = null)
        {
            try
            {
                CommunityCategoryModel communityModel = new CommunityCategoryModel() { category_name = categoryName, community_short_name = CommunityShortName, parent_category_id = parentCategoryID, category_image = categoryImage };

                string url = $"{ApiUrl}/community/create-category";

                Log.Fatal($"Inicio llamada 1.{communityModel.category_name} | 2.{communityModel.community_short_name} | 3.{communityModel.parent_category_id}");

                string response = WebRequestPostWithJsonObject(url, communityModel);

                return JsonConvert.DeserializeObject<Guid>(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating category {categoryName}: \r\n{ex.Message}");
                throw;
            }
        }

        public void UploadContentFile(UploadContentModel pmodel)
        {
            try
            {
                string url = $"{ApiUrl}/community/uploaded-content-file";

                WebRequestPostWithJsonObject(url, pmodel);

                Log.Debug($"File update.");
            }
            catch (Exception)
            {
                Log.Error($"Error File update");
                throw;
            }
        }

        /// <summary>
        /// Create a community
        /// </summary>
        /// <param name="communityModel">Community model</param>
        public void CreateCommunity(CommunityModel communityModel)
        {
            string json = null;
            try
            {
                string url = $"{ApiUrl}/community/create-community";

                WebRequestPostWithJsonObject(url, communityModel);

                Log.Debug($"Community created {json}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating community {json}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a community
        /// </summary>
        /// <param name="linkParentCommunity">Community model</param>
        public void LinkParentCommunity(LinkParentCommunityModel linkParentCommunity)
        {
            string json = null;
            try
            {
                string url = $"{ApiUrl}/community/link-parent-community";

                WebRequestPostWithJsonObject(url, linkParentCommunity);

                Log.Debug($"Community created {json}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating community {json}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        ///  Get the data a user by user email and the community short name
        /// </summary>
        /// <param name="number_resources">bool to return number of resources</param>
        /// <param name="number_comments">bool to return number of comments</param>
        /// <param name="groups">bool to return groups</param>
        /// <param name="pFechaInit">date init</param>
        /// <param name="pFechaFin">date end</param>
        public List<UserCommunity> GetUsersByCommunityShortName(bool number_resources, bool number_comments, bool groups, DateTime? pFechaInit, DateTime? pFechaFin)
        {
            string json = null;
            try
            {
                long fechainit = pFechaInit.HasValue ? pFechaInit.Value.Ticks : 0;
                long fechafin = pFechaFin.HasValue ? pFechaFin.Value.Ticks : 0;
                string url = $"{ApiUrl}/community/get-users-by-community-short-name?community_short_name={CommunityShortName}&number_resources={number_resources}&number_comments={number_comments}&groups={groups}&pFechaInit={fechainit}&pFechaFin={fechafin}";

                string response = WebRequest("GET", url);

                return JsonConvert.DeserializeObject<List<UserCommunity>>(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating community {json}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the thesaurus of the current community 
        /// </summary>
        /// <returns>Xml of the thesaurus</returns>
        public string GetThesaurus()
        {
            try
            {
                string url = $"{ApiUrl}/community/get-thesaurus?community_short_name={CommunityShortName}";

                return WebRequest("GET", url);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the thesaurus from {CommunityShortName}: \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a thesaurus for a community
        /// </summary>
        /// <param name="thesaurusXml">Thesaurus to create</param>
        public void CreateThesaurus(string thesaurusXml)
        {
            try
            {
                string url = $"{ApiUrl}/community/create-thesaurus";

                ThesaurusModel model = new ThesaurusModel() { community_short_name = CommunityShortName, thesaurus = thesaurusXml };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Thesaurus created in {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating thesaurus {thesaurusXml} in {CommunityShortName}: \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Open an existing community
        /// </summary>
        public void OpenCommunity()
        {
            try
            {
                string url = $"{ApiUrl}/community/open-community";

                WebRequestPostWithJsonObject(url, CommunityShortName);

                Log.Debug($"Community opened {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error opening community {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Config graph of community
        /// </summary>
        public void ConfigGraphCommunity()
        {
            try
            {
                string url = $"{ApiUrl}/community/config-graph-community";

                WebRequestPostWithJsonObject(url, CommunityShortName);

                Log.Debug($"Config graph community {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error graph community {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Upload the community settings
        /// </summary>
        /// <param name="settingsXml">Community settings in XML format</param>
        public void UploadConfiguration(string settingsXml)
        {
            try
            {
                string url = $"{ApiUrl}/community/upload-configuration-xml";

                ConfigurationModel model = new ConfigurationModel() { community_short_name = CommunityShortName, settings = settingsXml };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Settings uploaded successfully to {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error uploading settings to community {CommunityShortName} {settingsXml}: \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Upload the cms community settings
        /// </summary>
        /// <param name="settingsCmsXml">Community CMS settings in XML format</param>
        public void UploadCMSConfiguration(string settingsCmsXml)
        {
            try
            {
                string url = $"{ApiUrl}/community/upload-cms-configuration-xml";

                ConfigurationModel model = new ConfigurationModel() { community_short_name = CommunityShortName, settings = settingsCmsXml };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"CMS settings uploaded successfully to {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error uploading the CMS settings to community {CommunityShortName} {settingsCmsXml}: \r\n {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Register a user in a community
        /// </summary>
        /// <param name="userId">User identifier that we will register in the community</param>
        /// <param name="organizationShortName">Short name of the user organization</param>
        /// <param name="identityType">Type of user identity in the community</param>
        public void AddMember(Guid userId, string organizationShortName = null, short identityType = 0)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-member";

                MemberModel model = new MemberModel() { community_short_name = CommunityShortName, user_id = userId, identity_type = identityType, organization_short_name = organizationShortName };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"User {userId} added to {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding member {userId} to {CommunityShortName}: {ex.Message} ");
                throw;
            }
        }

        /// <summary>
        /// Delete a user from a community
        /// </summary>
        /// <param name="userId">User identifier that we will delete from the community</param>
        public void DeleteMember(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/community/delete-member";

                UserCommunityModel model = new UserCommunityModel() { community_short_name = CommunityShortName, user_id = userId };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Member {userId} deleted from {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting member {userId} from {CommunityShortName}: {ex.Message} ");
                throw;
            }
        }

        /// <summary>
        /// Register the users of a organization group in a community 
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="organizationShortName">Short name of the user organization</param>
        /// <param name="identityType">Type of user identity in the community</param>
        public void AddMemberOrganizationGroupToCommunity(string organizationShortName, string groupShortName, short identityType)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-members-organization-group-to-community";

                GroupOrgCommunityModel model = new GroupOrgCommunityModel() { community_short_name = CommunityShortName, organization_short_name = organizationShortName, group_short_name = groupShortName, identity_type = identityType };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The group {groupShortName} of {organizationShortName} has been added to {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding the group {groupShortName} of {organizationShortName} from {CommunityShortName}: {ex.Message} ");
                throw;
            }
        }

        /// <summary>
        /// Delete the users of a organization group in a community
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="organizationShortName">Short name of the user organization</param>
        public void DeleteMemberOrganizationGroupFromCommunity(string organizationShortName, string groupShortName)
        {
            try
            {
                string url = $"{ApiUrl}/community/delete-group";

                GroupOrgCommunityModel model = new GroupOrgCommunityModel() { community_short_name = CommunityShortName, organization_short_name = organizationShortName, group_short_name = groupShortName };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"All the members from the group {groupShortName} of {organizationShortName} has been deleted to {CommunityShortName}");

            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting the group members {groupShortName} of {organizationShortName} from {CommunityShortName}: {ex.Message} ");
                throw;
            }
        }

		/// <summary>
		/// Upgrade a user changing is role to community administrator
		/// </summary>
		/// <param name="userId">User identifier that we will upgrade to community</param>
		public void UpgradeMemberToAdministrator(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-administrator-member";

                UserCommunityModel model = new UserCommunityModel() { community_short_name = CommunityShortName, user_id = userId };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The member {userId} has been upgraded to administrator of {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error upgrading member {userId} to administrator in {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add the users of a organization group as administrator in a community
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="organizationShortName">Short name of the user organization</param>
        public void UpgradeMembersOrganizationGroupToAdministrators(string organizationShortName, string groupShortName)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-administrator-group";
                GroupOrgCommunityModel model = new GroupOrgCommunityModel() { community_short_name = CommunityShortName, organization_short_name = organizationShortName, group_short_name = groupShortName };

                string result = WebRequestPostWithJsonObject(url, model);

                Log.Debug($"All the members from the group { groupShortName} of { organizationShortName} has been upgraded to administrator in { CommunityShortName}. \r\n{result}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error upgrading to administrator the members from the group { groupShortName} of { organizationShortName} in { CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a group to the community
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="description">Description of the group</param>
        /// <param name="tags">Tags of the group</param>
        /// <param name="members">List users that want to add</param>
        /// <param name="sendNotification">It indicates whether an email is going to be sent to users telling them that has been added to the group</param>
        public void CreateCommunityGroup(string groupName, string groupShortName, string description, List<string> tags, List<Guid> members, bool sendNotification = false)
        {
            try
            {
                string url = $"{ApiUrl}/community/create-community-group";

                CreateGroupCommunityModel model = new CreateGroupCommunityModel() { community_short_name = CommunityShortName, group_name = groupName, group_short_name = groupShortName, tags = tags, members = members, description = description, send_notification = sendNotification };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Group {groupName} created in {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating group {groupName} in {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a community group from the community
        /// </summary>
        /// <param name="groupShortName">Short name of the community group</param>
        public void DeleteCommunityGroup(string groupShortName)
        {
            try
            {
                string url = $"{ApiUrl}/community/delete-community-group";

                DeleteGroupCommunityModel model = new DeleteGroupCommunityModel() { community_short_name = CommunityShortName, group_short_name = groupShortName };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Group {groupShortName} deleted in {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting group {groupShortName} in {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a list of users to a community group
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="members">List users that want to add</param>
        /// <param name="sendNotification">It indicates whether a massage is going to be sent to users telling them has been added to the group</param>
        public void AddMembersToGroup(string groupShortName, List<Guid> members, bool sendNotification = false)
        {
            string miembros = string.Empty;
            try
            {
                string url = $"{ApiUrl}/community/add-members-to-community-group";

                MembersGroupCommunityModel model = new MembersGroupCommunityModel() { community_short_name = CommunityShortName, group_short_name = groupShortName, members = members, send_notification = sendNotification };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The users has been added to the group {groupShortName} of the communtiy {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding users {string.Join(",", members)} to group {groupShortName} of the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a list of users to a community group
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="members">List users that want to add</param>
        /// <param name="sendNotification">It indicates whether a massage is going to be sent to users telling them has been added to the group</param>
        public void CreateCertificatioLevels(List<string> certificationLevelsDescription, string certificationPolitics)
        {
            string miembros = string.Empty;
            try
            {
                string url = $"{ApiUrl}/community/create-certification-levels";

                CertificationLevelModel model = new CertificationLevelModel() { community_short_name = CommunityShortName, certification_levels = certificationLevelsDescription, certification_politics = certificationPolitics };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The certification levels has been added to the communtiy {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating certification levels of the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a list of users from a community group
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <param name="members">List users that want to add</param>
        public void DeleteMembersFromGroup(string groupShortName, List<Guid> members)
        {
            try
            {
                string url = $"{ApiUrl}/community/delete-members-of-community-group";

                MembersGroupCommunityModel model = new MembersGroupCommunityModel() { community_short_name = CommunityShortName, group_short_name = groupShortName, members = members };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The users has been deleted from the group {groupShortName} of the communtiy {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting users {string.Join(",", members)} from group {groupShortName} of the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the members of a community group 
        /// </summary>
        /// <param name="groupShortName">Short name of the group</param>
        /// <returns>List of members identifiers of the community group</returns> 
        public List<Guid> GetGroupMembers(string groupShortName)
        {
            List<Guid> members = null;
            try
            {
                string url = $"{ApiUrl}/community/get-members-from-community-group?community_short_name={CommunityShortName}&group_short_name={groupShortName}";

                string response = WebRequest("GET", url);

                members = JsonConvert.DeserializeObject<List<Guid>>(response);

                Log.Debug($"Users obtained from group {groupShortName} of the community {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining users from group {groupShortName} of the community {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
            return members;
        }

        /// <summary>
        /// Gets the members of a organization group 
        /// </summary>
        /// <param name="organizationShortName">Short name of the user organization</param>
        /// <param name="groupShortName">Short name of the group</param>
        /// <returns>List of members identifiers of the organization group</returns>
        public List<Guid> GetOrganizationGroupMembers(string organizationShortName, string groupShortName)
        {
            List<Guid> members = null;
            try
            {
                string url = $"{ApiUrl}/community/get-members-organization-group?organization_short_name={organizationShortName}&group_short_name={groupShortName}";

                string response = WebRequest("GET", url);

                members = JsonConvert.DeserializeObject<List<Guid>>(response);

                Log.Debug($"Users obtained from group {groupShortName} of the organization {organizationShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining users from group {groupShortName} of the organization {organizationShortName}: \r\n{ex.Message}");
                throw;
            }
            return members;
        }

        /// <summary>
        /// Expel a user from a community
        /// </summary>
        /// <param name="userId">User identifier to expel from the community</param>
        public void ExpelMember(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/community/expel-member";

                UserCommunityModel model = new UserCommunityModel() { community_short_name = CommunityShortName, user_id = userId };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"User {userId} expelled from {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error expelling member {userId} from {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Close a community
        /// </summary>
        public void CloseCommunity()
        {
            try
            {
                string url = $"{ApiUrl}/community/close-community";

                WebRequestPostWithJsonObject(url, CommunityShortName);

                Log.Debug($"Community {CommunityShortName} closed");
            }
            catch (Exception ex)
            {
                Log.Error($"Error closing {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Change the community name
        /// </summary>
        /// <param name="newName">New community name</param>
        public void ChangeCommunityName(string newName)
        {
            try
            {
                string url = $"{ApiUrl}/community/change-community-name";

                ChangeNameCommunityModel model = new ChangeNameCommunityModel() { community_name = newName, community_short_name = CommunityShortName };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Community name of {CommunityShortName} changed to {newName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error changing name of {CommunityShortName} to {newName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets a list with the groups of the community
        /// </summary>
        /// <returns>List with the groups of the community</returns>
        public List<GroupCommunityModel> GetCommunityGroups()
        {
            List<GroupCommunityModel> groups = null;
            try
            {
                string url = $"{ApiUrl}/community/get-community-groups?community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url);

                groups = JsonConvert.DeserializeObject<List<GroupCommunityModel>>(response);

                Log.Debug($"Groups obtained from {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining groups from {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
            return groups;
        }

        /// <summary>
        /// Gets the organization name of a user in a community
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Organization name of a user in a community</returns>
        public string GetOrganizationShortNameFromMember(string userId)
        {
            string organizationShortName = null;

            try
            {
                string url = $"{ApiUrl}/community/get-organization-short-name-from-member?community_short_name={CommunityShortName}&user_id={userId}";

                organizationShortName = WebRequest("GET", url)?.Trim('"');
                Log.Debug($"Organization short name obtained from {userId} in {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining the organization short name from {userId} in {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }

            return organizationShortName;
        }

        /// <summary>
        /// Get the extra register data of a community
        /// </summary>
        /// <returns>List of ExtraRegisterData</returns>
        public List<ExtraRegisterData> GetExtraRegisterData()
        {
            List<ExtraRegisterData> extraRegisterData = null;
            try
            {
                string url = $"{ApiUrl}/community/get-extra-register-data?community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url);

                extraRegisterData = JsonConvert.DeserializeObject<List<ExtraRegisterData>>(response);

                Log.Debug($"Extra register data obtained from {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining extra register data from {CommunityShortName}: \r\n{ex.Message}");
                throw;
            }
            return extraRegisterData;
        }

        /// <summary>
        /// 20141103
        /// Gets the community main language. Empty if it is not defined
        /// </summary>
        /// <returns>Community main language</returns>
        public string GetCommunityMainLanguage()
        {
            try
            {
                string url = $"{ApiUrl}/community/get-main-language?community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url, acceptHeader: "application/json")?.Trim('"');
                return response;
            }
            catch (System.Exception)
            {
                Log.Error("Imposible to obtain the main language");
                return null;
            }
        }

        /// <summary>
        /// Get the person identifier and his email from the community members
        /// </summary>
        /// <returns>dictionary with the person identifier and his email</returns>
        public Dictionary<Guid, string> GetCommunityPersonIDEmail()
        {
            try
            {
                string url = $"{ApiUrl}/community/get-community-personid-email?community_short_name={CommunityShortName}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");
                return JsonConvert.DeserializeObject<Dictionary<Guid, string>>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The person identifiers and emails of the community members '{CommunityShortName}' could not be obtained");
                return null;
            }
        }

        public List<TextosTraducidosIdiomas> GetTranslations(Guid? community_id, string community_short_name, string language)
        {
            try
            {
                string url = $"{ApiUrl}/community/get-language-translations?community_id={community_id}&community_short_name={community_short_name}&language={language}";
                string response = WebRequest("GET", url, acceptHeader: "application/json");
                return JsonConvert.DeserializeObject<List<TextosTraducidosIdiomas>>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The proyect could not be obtained");
                return null;
            }
        }

        public string GetTranslation(Guid? community_id, string community_short_name, string language, string text_id)
        {
            try
            {
                string url = $"{ApiUrl}/community/get-language-translation?community_id={community_id}&community_short_name={community_short_name}&language={language}&text_id={text_id}";
                string response = WebRequest("GET", url, acceptHeader: "application/json");
                return JsonConvert.DeserializeObject<string>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The proyect could not be obtained");
                return null;
            }
        }


        /// <summary>
        /// Gets the community identifier
        /// </summary>
        /// <param name="communityShortName">Community short name</param>
        public Guid GetCommunityId()
        {
            try
            {
                string url = $"{ApiUrl}/community/get-community-id?community_short_name={HttpUtility.UrlEncode(CommunityShortName)}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<Guid>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The community {CommunityShortName} could not be found");
                throw;
            }
        }

        /// <summary>
        /// Blocks a member in a community
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <param name="communityShortName">Community short name</param>
        public void BlockMember(Guid userId)
        {
            try
            {
                UserCommunityModel parameters = new UserCommunityModel() { user_id = userId, community_short_name = CommunityShortName };

                string url = $"{ApiUrl}/community/block-member";

                WebRequestPostWithJsonObject(url, parameters);
            }
            catch (System.Exception)
            {
                Log.Error($"The user {userId} of the community members '{CommunityShortName}' could not be blocked");
                throw;
            }
        }

        /// <summary>
        /// Unblocks a member in a community
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <param name="communityShortName">Community short name</param>
        public void UnblockMember(Guid userId)
        {
            try
            {
                UserCommunityModel parameters = new UserCommunityModel() { user_id = userId, community_short_name = CommunityShortName };

                string url = $"{ApiUrl}/community/block-member";

                WebRequestPostWithJsonObject(url, parameters);
            }
            catch (System.Exception)
            {
                Log.Error($"The user {userId} of the community members '{CommunityShortName}' could not be unblocked");
                throw;
            }
        }

        /// <summary>
        /// Refresh the caché of a CMS component
        /// </summary>
        /// <param name="componentId">Component id to refresh</param>
        /// <param name="communityShortName">Community short name</param>
        public void RefreshCMSComponent(Guid componentId)
        {
            try
            {
                string url = $"{ApiUrl}/community/refresh-cms-component?component_id={componentId.ToString()}&community_short_name={HttpUtility.UrlEncode(CommunityShortName)}";

                WebRequest("POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The component id {componentId} of the community '{CommunityShortName}' could not be refreshed");
                throw;
            }
        }

        /// <summary>
        /// Refresh the caché of all community's CMS components
        /// </summary>
        /// <param name="communityShortName">Community short name</param>
        public void RefreshAllCMSComponents()
        {
            try
            {
                string url = $"{ApiUrl}/community/refresh-all-cms-components?community_short_name={HttpUtility.UrlEncode(CommunityShortName)}";

                WebRequest("POST", url);
            }
            catch (System.Exception)
            {
                Log.Error($"The components of the community '{CommunityShortName}' could not be refreshed");
                throw;
            }
        }

        /// <summary>
        /// Gets the basic information of a community
        /// </summary>
        /// <param name="communityShortName">Community short name</param>
        /// <returns></returns>
        public CommunityInfoModel GetCommunityInfo()
        {
            try
            {
                string url = $"{ApiUrl}/community/get-community-information?community_short_name={HttpUtility.UrlEncode(CommunityShortName)}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<CommunityInfoModel>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The community {CommunityShortName} could not be found");
                throw;
            }
        }

        /// <summary>
        /// Gets the basic information of a community
        /// </summary>
        /// <param name="communityId">Community identifier</param>
        /// <returns></returns>
        public CommunityInfoModel GetCommunityInfo(Guid communityId)
        {
            try
            {
                string url = $"{ApiUrl}/community/get-community-information?community_id={communityId}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<CommunityInfoModel>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The community {communityId} could not be found");
                throw;
            }
        }

        /// <summary>
        /// Gets the name of a category in all of the languages that has been defined
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns></returns>
        public CategoryNames GetCategoryName(Guid categoryId)
        {
            try
            {
                string url = $"{ApiUrl}/community/get-category-name?category_id={categoryId}";

                string response = WebRequest("GET", url, acceptHeader: "application/json");

                return JsonConvert.DeserializeObject<CategoryNames>(response);
            }
            catch (System.Exception)
            {
                Log.Error($"The category {categoryId} could not be found");
                throw;
            }
        }

        public bool checkIsAdminCommunity(string pShortName, Guid pUserID)
        {
            bool esAdmin = false;
            UserCommunityModel model = new UserCommunityModel();
            model.community_short_name = pShortName;
            model.user_id = pUserID;
            string url = $"{ApiUrl}/community/check-administrator-community";
            string response = WebRequestPostWithJsonObject(url, model);
            esAdmin = JsonConvert.DeserializeObject<bool>(response);
            return esAdmin;
        }

        public void DowngradeMemberFromAdministrator(Guid userId)
        {
            try
            {
                string url = $"{ApiUrl}/community/delete-administrator-permission";

                UserCommunityModel model = new UserCommunityModel() { community_short_name = CommunityShortName, user_id = userId };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The member {userId} has been upgraded to administrator of {CommunityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error upgrading member {userId} to administrator in {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add the value sended to cache
        /// </summary>
        /// <param name="key">Key to add to cache</param>
        /// <param name="queryValue">Value to add to cache</param>
        /// <param name="duration">Duration of the cache expirtation</param>
        public void AddSearchToCache(string key, ConsultaCacheModel queryValue, double duration)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-search-to-cache";

                //ConsultaCacheModel queryValue = JsonConvert.DeserializeObject<ConsultaCacheModel>(value);

                AddSearchToCacheModel model = new AddSearchToCacheModel() { key = key, value = queryValue, community_short_name = CommunityShortName, duration = duration };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The value was added correctly to cache to the community: {CommunityShortName} ");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding the cache to {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add the value sended to cache
        /// </summary>
        /// <param name="key">Key to add to cache</param>
        /// <param name="value">Value to add to cache</param>
        /// <param name="duration">Duration of the cache expirtation</param>
        public void AddToCache(string key, string value, double duration)
        {
            try
            {
                string url = $"{ApiUrl}/community/add-to-cache";

                AddToCacheModel model = new AddToCacheModel() { key = key, value = value, community_short_name = CommunityShortName, duration = duration };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The value was added correctly to cache to the community: {CommunityShortName} ");
            }
            catch (Exception ex)
            {
                Log.Error($"Error adding the cache to {CommunityShortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get a text in other language
        /// </summary>
        /// <param name="language">Language of the text</param>
        /// <param name="textoID">ID of the text</param>
        public void GetTextByLanguage(string language, string textoID)
        {
            try
            {
                string url = $"{ApiUrl}/community/get-text-by-language";

                GetTextByLanguageModel model = new GetTextByLanguageModel() { community_short_name = CommunityShortName, language = language, texto_id = textoID };

                WebRequestPostWithJsonObject(url, model);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the text by the data given: {ex.Message}");
                throw;
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the community categories
        /// </summary>
        public List<ThesaurusCategory> CommunityCategories
        {
            get
            {
                if (_communityCategories == null)
                {
                    _communityCategories = LoadCategories(CommunityShortName);
                }
                return _communityCategories;
            }
            set
            {
                _communityCategories = value;
            }
        }

        #endregion
    }
}
