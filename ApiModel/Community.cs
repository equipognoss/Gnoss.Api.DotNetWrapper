using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gnoss.ApiWrapper.ApiModel
{

    /// <summary>
    /// Represents a community
    /// </summary>
    public class CommunityModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public string community_name { get; set; }

        /// <summary>
        /// Short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Brief Description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Tags (comma separated)
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        /// Type: 0 for standard communities, 5 for static catalogs, 8 for static catalogs without members
        /// </summary>
        public short type { get; set; }

        /// <summary>
        /// Acces type: 0: public (Any user can be member), 1: private (Only users with invitation can be members), 2: restricted (users can request access, but admin must accept the requests), 3: reserved (private community childred of other private community)
        /// </summary>
        public short access_type { get; set; }

        /// <summary>
        /// Parent community short name
        /// </summary>
        public string parent_community_short_name { get; set; }

        /// <summary>
        /// User identifier of the administrator of the community
        /// </summary>
        public Guid admin_id { get; set; }

        /// <summary>
        /// Organization short name of the user
        /// </summary>
        public string organization_short_name { get; set; }

        /// <summary>
        /// Logo for the community
        /// </summary>
        public byte[] logo { get; set; }
    }

    /// <summary>
    /// Link parent community
    /// </summary>
    public class LinkParentCommunityModel
    {
        /// <summary>
        /// Short name
        /// </summary>
        public string short_name { get; set; }

        /// <summary>
        /// Parent community short name
        /// </summary>
        public string parent_community_short_name { get; set; }

        /// <summary>
        /// User identificator of the administrator of the community
        /// </summary>
        public Guid admin_id { get; set; }
    }

    public class CommunityCategoryModel
    {
        public string community_short_name { get; set; }
        public string category_name { get; set; }
        public Guid? parent_category_id { get; set; }
    }

    /// <summary>
    /// Represents a community
    /// </summary>
    public class CommunityInfoModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Short name
        /// </summary>
        public string short_name { get; set; }

        /// <summary>
        /// Brief Description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Tags (comma separated)
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        /// Community's type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Community's access type
        /// </summary>
        public short access_type { get; set; }

        /// <summary>
        /// Community's categories
        /// </summary>
        public List<Guid> categories { get; set; }

        /// <summary>
        /// Community's users
        /// </summary>
        public List<Guid> users { get; set; }
    }

    /// <summary>
    /// Represents the name of a category in a specific language
    /// </summary>
    public class CategoryName
    {
        /// <summary>
        /// Language of the name
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// Category's name
        /// </summary>
        public string category_name { get; set; }
    }

    /// <summary>
    /// Represents the name of the category in all of the languages that has been defined
    /// </summary>
    public class CategoryNames
    {
        /// <summary>
        /// Category identifier
        /// </summary>
        public Guid category_id { get; set; }

        /// <summary>
        /// Name per language
        /// </summary>
        public List<CategoryName> category_names_list { get; set; }
    }

    /// <summary>
    /// Represents a thesaurus
    /// </summary>
    public class ThesaurusModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Thesaurus in XML Format
        /// </summary>
        public string thesaurus { get; set; }
    }

    /// <summary>
    /// Represents a thesaurus category
    /// </summary>
    public class ThesaurusCategory
    {
        /// <summary>
        /// Category identifier
        /// </summary>
        public Guid category_id { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string category_name { get; set; }

        /// <summary>
        /// Children categories
        /// </summary>
        public List<ThesaurusCategory> Children { get; set; }

        /// <summary>
        /// Parent category identifier
        /// </summary>
        public Guid parent_category_id { get; set; }

        /// <summary>
        /// ThesaurusCategory constructor
        /// </summary>
        public ThesaurusCategory()
        {
            Children = new List<ThesaurusCategory>();
        }
    }

    /// <summary>
    /// Represents the configuration of a community
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Settings in XML format
        /// </summary>
        public string settings { get; set; }
    }

    /// <summary>
    /// Parameters to Upload files.
    /// </summary>
    public class UploadContentModel
    {
        /// <summary>
        /// path
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Bytes of file
        /// </summary>
        public byte[] bytes_file { get; set; }
    }

    /// <summary>
    /// Parameters to add a member to a commmunity
    /// </summary>
    public class MemberModel
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Organization short name
        /// </summary>
        public string organization_short_name { get; set; }

        /// <summary>
        /// Identity type of the user: 0 personal, 1 personal professional, 2 corporative professional. By default: 0
        /// </summary>
        public short identity_type { get; set; }
    }

    /// <summary>
    /// Represents a user in a community
    /// </summary>
    public class UserCommunityModel
    {
        /// <summary>
        /// User identifier that we will delete from the community
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Short name of the community
        /// </summary>
        /// <example>ferdev</example>
        public string community_short_name { get; set; }
    }

    public class AddSearchToCacheModel
    {
        /// <summary>
        /// Key to add to cache
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Value to add to cache
        /// </summary>
        public ConsultaCacheModel value { get; set; }

        /// <summary>
        /// Short name of the community
        /// </summary>
        /// <example>ferdev</example>
        public string community_short_name { get; set; }

        /// <summary>
        /// Duration of cache expiration in seconds
        /// </summary>
        public double duration { get; set; }
    }

    [Serializable]
    public class ConsultaCacheModel
    {
        public string WhereSPARQL { get; set; }
        public string WhereFacetasSPARQL { get; set; }
        public string OrderBy { get; set; }
        public bool OmitirRdfType { get; set; }
    }

    /// <summary>
    /// Model to add an string to cache
    /// </summary>
    public class AddToCacheModel
    {
        /// <summary>
        /// Key to add to cache
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Value to add to cache
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Short name of the community
        /// </summary>
        /// <example>ferdev</example>
        public string community_short_name { get; set; }

        /// <summary>
        /// Duration of cache expiration in seconds
        /// </summary>
        public double duration { get; set; }
    }

    /// <summary>
    /// Represents a community group
    /// </summary>
    public class GroupCommunityModel
    {
        /// <summary>
        /// Group identifier
        /// </summary>
        public Guid group_id { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string group_name { get; set; }
    }

    /// <summary>
    /// Represents an organization group
    /// </summary>
    public class GroupOrgCommunityModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Organization short name
        /// </summary>
        public string organization_short_name { get; set; }

        /// <summary>
        ///  Identity type of the user: 0 personal, 1 personal professional, 2 corporative professional. By default: 0
        /// </summary>
        public short identity_type { get; set; }
    }

    /// <summary>
    /// Parameters to create a community group
    /// </summary>
    public class CreateGroupCommunityModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string group_name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Tags (comma separated)
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// Initial members of the group
        /// </summary>
        public List<Guid> members { get; set; }

        /// <summary>
        /// True if a welcom message must be sent to the members of the group (by default false)
        /// </summary>
        public bool send_notification { get; set; }
    }

    /// <summary>
    /// Parameters to delete a community group
    /// </summary>
    public class DeleteGroupCommunityModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }
    }

    /// <summary>
    /// Represents a CertificationLevel
    /// </summary>
    public class CertificationLevelModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// List of certification levels
        /// </summary>
        public List<string> certification_levels { get; set; }

        /// <summary>
        /// Certification politics
        /// </summary>
        public string certification_politics { get; set; }
    }

    /// <summary>
    /// Represents the members of a group community
    /// </summary>
    public class MembersGroupCommunityModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Group members
        /// </summary>
        public List<Guid> members { get; set; }

        /// <summary>
        /// True if a welcom message must be sent to the members of the group (by default false)
        /// </summary>
        public bool send_notification { get; set; }
    }

    /// <summary>
    /// Parameters to change the name of a community
    /// </summary>
    public class ChangeNameCommunityModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// New name
        /// </summary>
        public string community_name { get; set; }
    }

    /// <summary>
    /// Extra register data
    /// </summary>
    public class ExtraRegisterData
    {
        /// <summary>
        /// key of the estra register data
        /// </summary>
        public Guid key { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Options of the extra register data
        /// </summary>
        public Dictionary<Guid, string> options { get; set; }
    }
}