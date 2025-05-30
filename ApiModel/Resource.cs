﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gnoss.ApiWrapper.ApiModel
{
    #region Enumerations

    /// <summary>
    /// Resource visibility
    /// </summary>
    public enum ResourceVisibility
    {
        /// <summary>
        /// All users can view the resource
        /// </summary>
        open = 0,
        /// <summary>
        /// Only editors can view the resource
        /// </summary>
        editors = 1,
        /// <summary>
        /// Only community members can view the resource
        /// </summary>
        communitymembers = 2,
        /// <summary>
        /// Specific users can view the resource
        /// </summary>
        specific = 3
    }

    /// <summary>
    /// Resource property to modify in loads
    /// </summary>
    public enum GnossResourceProperty
    {
        /// <summary>
        /// Any property
        /// </summary>
        none = 0,
        /// <summary>
        /// resource title property
        /// </summary>
        title = 1,
        /// <summary>
        /// resource description property
        /// </summary>
        description = 2
    }

    /// <summary>
    /// Document types
    /// </summary>
    public enum TiposDocumentacion
    {
        /// <summary>
        /// HyperLink
        /// </summary>
        hyperlink = 0,
        /// <summary>
        /// Video
        /// </summary>
        video = 2,
        /// <summary>
        /// Server file
        /// </summary>
        server_file = 3,
        /// <summary>
        /// Ontology
        /// </summary>
        ontology = 5,
        /// <summary>
        /// Image
        /// </summary>
        image = 6,        
        /// <summary>
        /// Note
        /// </summary>
        note = 8,
        /// <summary>
        /// Newsletter
        /// </summary>
        newsletter = 11,
        /// <summary>
        /// Audio
        /// </summary>
        audio = 21,
        /// <summary>
        /// Secondary ontology
        /// </summary>
        secondary_ontology = 23
    }

    /// <summary>
    /// Types of properties of the file for the attached resources
    /// </summary>
    public enum AttachedResourceFilePropertyTypes
    {
        /// <summary>
        /// Indicates the attached resource is a file
        /// </summary>
        file = 0,
        /// <summary>
        /// Indicates the attached resource is an image
        /// </summary>
        image = 1,
        /// <summary>
        /// Indicates the attached resource is a link file
        /// </summary>
        downloadableFile = 2
    }

    #endregion

    /// <summary>
    /// Represents a list of triples to modify
    /// </summary>
    public class ModifyTripleListModel
    {

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// URL of the secondary ontology
        /// </summary>
        public string secondary_ontology_url { get; set; }

        /// <summary>
        /// Identificator of the secondary entity
        /// </summary>
        public string secondary_entity { get; set; }

        /// <summary>
        /// Triple list to modify. It's a array of string arrays with three items: Old object, Predicate, New object. To delete a triple, send the New object empty
        /// </summary>
        public string[][] triple_list { get; set; }
    }

    /// <summary>
    /// Parameters for massive modification
    /// </summary>
    public class MassiveTripleModifyParameters
    {
        /// <summary>
        /// Triples to modify
        /// </summary>
        public List<MassiveTriple> triples { get; set; }

        /// <summary>
        /// Ontology to modify
        /// </summary>
        public string ontology { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

    }

    /// <summary>
    /// Triple for massive modification
    /// </summary>
    public class MassiveTriple
    {
        /// <summary>
        /// Main resource id, like http://gnoss.com/items/Product_0015bb97-81fd-5ee7-a70d-4474f4c723e9_9b8d4c13-b443-4e3c-9a93-252d9e12df8a
        /// </summary>
        public string main_resource_id { get; set; }

        /// <summary>
        /// Subject of the triple
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// Predicate of the triple
        /// </summary>
        public string predicate { get; set; }

        /// <summary>
        /// (Optional) New value of the triple
        /// </summary>
        public string new_value { get; set; }

        /// <summary>
        /// (Optional) Old value of the triple
        /// </summary>
        public string old_value { get; set; }

        /// <summary>
        /// (Optional) Indicates if the new value is the subject of a new auxiliary entity. 
        /// You must set it to true if you are creating a new auxiliary entity. 
        /// If it's a new auxiliary entity, you must send the rdf:type and rdfs:label triples too. 
        /// </summary>
        public bool is_new_auxiliary_entity { get; set; }

        /// <summary>
        /// (Optional) Indicates the language of the object
        /// </summary>
        public string language { get; set; }
    }

    /// <summary>
    /// Represents a secondary entity
    /// </summary>
    public class SecondaryEntityModel
    {
        /// <summary>
        /// Ontology url
        /// </summary>
        public string ontology_url { get; set; }

        /// <summary>
        /// Communtiy short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Entity identifier
        /// </summary>
        public string entity_id { get; set; }

        /// <summary>
        /// RDF of the secondary entity
        /// </summary>
        public byte[] rdf { get; set; }
    }

    /// <summary>
    /// Represents an Ontology file
    /// </summary>
    public class FileOntology
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Ontology name
        /// </summary>
        public string ontology_name { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string file_name { get; set; }

        /// <summary>
        /// The file bytes
        /// </summary>
        public byte[] file { get; set; }
    }

    /// <summary>
    /// Represents a sparql query
    /// </summary>
    public partial class SparqlQuery
    {
        /// <summary>
        /// Ontology name or community identifier to query. It will be used in the form clause
        /// </summary>
        public string ontology { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Select clause of the sparql query
        /// </summary>
        public string query_select { get; set; }

        /// <summary>
        /// Where clause of the sparql query
        /// </summary>
        public string query_where { get; set; }

        /// <summary>
        /// Use virtuoso master connection if true and the afinity connection if false
        /// </summary>
        public bool use_virtuoso_balancer { get; set; } = true;
    }

    /// <summary>
    /// Parameters to unshared a resource
    /// </summary>
    public class UnsharedResourceParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }
    }

    /// <summary>
    /// Triples representation
    /// </summary>
    public class Triples
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// True if the resource must be published in the home of the community (by default false)
        /// </summary>
        public bool publish_home { get; set; }

        /// <summary>
        /// True if it's the end of the load and must delete the cache
        /// </summary>
        public bool end_of_load { get; set; }
    }

    /// <summary>
    /// Resource editors
    /// </summary>

    /// <summary>
    /// Resource readers
    /// </summary>
    public class KeyReaders
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Users short names of the resource editors
        /// </summary>
        public List<string> readers { get; set; }

        /// <summary>
        /// Editors group
        /// </summary>
        public List<ReaderGroup> reader_groups { get; set; }
    }

    /// <summary>
    /// Represents a reader group
    /// </summary>
    public class ReaderGroup
    {
        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Organization short name
        /// </summary>
        public string organization_short_name { get; set; }
    }

    /// <summary>
    /// Resource editors
    /// </summary>
    public class KeyEditors
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Users short names of the resource editors
        /// </summary>
        public List<string> editors { get; set; }

        /// <summary>
        /// Editors group
        /// </summary>
        public List<EditorGroup> editor_groups { get; set; }
    }

    /// <summary>
    /// Represents an editor group
    /// </summary>
    public class EditorGroup
    {
        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Organization short name
        /// </summary>
        public string organization_short_name { get; set; }
    }

    /// <summary>
    /// Parameters for get the download url of a resource
    /// </summary>
    public class GetDownloadUrlParams
    {
        /// <summary>
        /// Resource list to get their download URL
        /// </summary>
        public List<Guid> resource_id_list { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }
    }

    /// <summary>
    /// Voted parameters model
    /// </summary>
    public class VotedParameters
    {

        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// User identificator who has voted the resource
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Project identificator
        /// </summary>
        public Guid project_id { get; set; }

        /// <summary>
        /// Vote value
        /// </summary>
        public float vote_value { get; set; }
    }

    /// <summary>
    /// Parameters to get a resource url
    /// </summary>
    public class ResponseGetUrl
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Url of the resource
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// Parameters to get the resource url
    /// </summary>
    public class GetUrlParams
    {
        /// <summary>
        /// Resource list to get their download URL
        /// </summary>
        public List<Guid> resource_id_list { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Language of the url
        /// </summary>
        public string language { get; set; }
    }

    /// <summary>
    /// Parameters to set the resource editors
    /// </summary>
    public class SetReadersEditorsParams
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Visibility of the resurce <see cref="ResourceVisibility"/> enumeration
        /// </summary>
        public short visibility { get; set; }

        /// <summary>
        /// Users short names of the resource editors or readers
        /// </summary>
        public List<ReaderEditor> readers_list { get; set; }

        /// <summary>
        /// True if the resource must be published in the home of the community (by default false)
        /// </summary>
        public bool publish_home { get; set; }
    }

    /// <summary>
    /// Represents a resource reader or editor
    /// </summary>
    public class ReaderEditor
    {
        /// <summary>
        /// User short name
        /// </summary>
        public string user_short_name { get; set; }

        /// <summary>
        /// Group short name
        /// </summary>
        public string group_short_name { get; set; }

        /// <summary>
        /// Organization short name
        /// </summary>
        public string organization_short_name { get; set; }
    }

    /// <summary>
    /// Represents the categories of a resource
    /// </summary>
    public class ResponseGetCategories
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Categories of the resource
        /// </summary>
        public List<ThesaurusCategory> category_id_list { get; set; }
    }

    /// <summary>
    /// Represents the tags of a resource
    /// </summary>
    public class ResponseGetTags
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource tags
        /// </summary>
        public List<string> tags { get; set; }
    }

    /// <summary>
    /// Represents the image of a resource
    /// </summary>
    public class ResponseGetMainImage
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// The image path
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// All the posible sizes for the image
        /// </summary>
        public List<short> sizes { get; set; }
    }

    /// <summary>
    /// Parameters to insert attributes
    /// </summary>
    public class InsertAttributeParams
    {
        /// <summary>
        /// Graph Url
        /// </summary>
        public string graph { get; set; }

        /// <summary>
        /// Value to insert
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Parameters to delete a resource
    /// </summary>
    public class DeleteParams
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// True if it's the end of the load
        /// </summary>
        public bool end_of_load { get; set; }
    }

    /// <summary>
    /// Parameters to delete persistent a resource
    /// </summary>
    public class PersistentDeleteParams
    {
        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// True if it's the end of the load
        /// </summary>
        public bool end_of_load { get; set; }

        /// <summary>
        /// True if the resource attached file must be deleted
        /// </summary>
        public bool delete_attached { get; set; }
    }

    /// <summary>
    /// Parameters to check if exists a url
    /// </summary>
    public class ExistsUrlParams
    {
        /// <summary>
        /// Url to check
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }
    }

    /// <summary>
    /// Parameters to share a resource
    /// </summary>
    public class ShareParams
    {
        /// <summary>
        /// Community short name where the resource is going to be published
        /// </summary>
        public string destination_communitiy_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Categories of the destination community
        /// </summary>
        public List<Guid> categories { get; set; }
        
        /// <summary>
        /// Mail of the resource publisher
        /// </summary>
        public string publisher_email { get; set; }

        public string shortNameOrEmail { get; set; }

        public Guid userId { get; set; }
    }

    /// <summary>
    /// Parameters to set the main images
    /// </summary>
    public class SetMainImageParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Path of the image
        /// </summary>
        public string path { get; set; }
    }

    /// <summary>
    /// Parameters to set the publisher of a resource
    /// </summary>
    public class SetPublisherParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Email of the resource publisher
        /// </summary>
        public string publisher_email { get; set; }

        /// <summary>
        /// Resource identifier list
        /// </summary>
        public List<Guid> resource_id_list { get; set; }

        /// <summary>
        /// True if the original editors must be keeped
        /// </summary>
        public bool keep_editors { get; set; }
    }

    /// <summary>
    /// Parameters for comments
    /// </summary>
    public class CommentParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// User short name
        /// </summary>
        public string user_short_name { get; set; }

        /// <summary>
        /// Description of the comment
        /// </summary>
        public string html_description { get; set; }

        /// <summary>
        /// Parent comment identifier (if the comment is an answer to another comment)
        /// </summary>
        public Guid parent_comment_id { get; set; }

        /// <summary>
        /// Comment date
        /// </summary>
        public DateTime comment_date { get; set; }

        /// <summary>
        /// True if the comment must be pusblished in the community home
        /// </summary>
        public bool publish_home { get; set; }
        public Guid UserId { get; set; }

        public string login { get; set; }
    }

    /// <summary>
    /// Parameters to link a resource
    /// </summary>
    public class LinkedParams
    {
        /// <summary>
        /// Resource to be linked by the resource list
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// List of resources to link
        /// </summary>
        public List<Guid> resoruce_list_to_link { get; set; }
    }

    /// <summary>
    /// Parameters to modify categories a resorce
    /// </summary>
    public class ModifyResourceCategories
    {

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource to be modify categories
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// List of categories to modify
        /// </summary>
        public List<Guid> categories { get; set; }
    }

    /// <summary>
    /// Parameters to upload images of a resource
    /// </summary>
    public class UploadImagesParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        [Required]
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource attached files
        /// </summary>
        [Required]
        public List<SemanticAttachedResource> resource_attached_files { get; set; }

        /// <summary>
        /// Identifier of the resource
        /// </summary>
        [Required]
        public Guid resource_id { get; set; }

        /// <summary>
        /// Main image of the resource
        /// </summary>
        public string main_image { get; set; }
    }

    /// <summary>
    /// Parameters to upload a resource
    /// </summary>
    public class LoadResourceParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>        
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource title
        /// </summary>        
        public string title { get; set; }

        /// <summary>
        /// Resource description
        /// </summary>        
        public string description { get; set; }

        /// <summary>
        /// Resource tags
        /// </summary>        
        public List<string> tags { get; set; }

        /// <summary>
        /// Resource categories
        /// </summary>
        public List<Guid> categories { get; set; }

        /// <summary>
        /// Resource type
        /// </summary>        
        public short resource_type { get; set; }

        /// <summary>
        /// Resource url
        /// </summary>
        public string resource_url { get; set; }

        /// <summary>
        /// Resource attached file
        /// </summary>
        public byte[] resource_file { get; set; }

        /// <summary>
        /// Resource attached files
        /// </summary>
        public List<SemanticAttachedResource> resource_attached_files { get; set; }

        /// <summary>
        /// True if the resource creator is the author
        /// </summary>

        public bool creator_is_author { get; set; }

        /// <summary>
        /// Resource authors (comma separated)
        /// </summary>
        public string authors { get; set; }

        /// <summary>
        /// Tags auto extracted of title
        /// </summary>
        public string auto_tags_title_text { get; set; }

        /// <summary>
        /// Tags auto extracted of description
        /// </summary>
        public string auto_tags_description_text { get; set; }

        /// <summary>
        /// True if a screenshot of the resource must be generated
        /// </summary>
        public bool create_screenshot { get; set; }

        /// <summary>
        /// Url to make a screenshot
        /// </summary>
        public string url_screenshot { get; set; }

        /// <summary>
        /// Screenshot predicate
        /// </summary>
        public string predicate_screenshot { get; set; }

        /// <summary>
        /// Screenshot posible sizes
        /// </summary>
        public List<int> screenshot_sizes { get; set; }

        /// <summary>
        /// Priority of the upload
        /// </summary>
        public int priority { get; set; }

        /// <summary>
        /// Resource visibility <see cref="ResourceVisibility"/> enumeration
        /// </summary>
        public short visibility { get; set; }

        /// <summary>
        /// Resource readers list
        /// </summary>
        public List<ReaderEditor> readers_list { get; set; }

        /// <summary>
        /// Resource editors list
        /// </summary>
        public List<ReaderEditor> editors_list { get; set; }

        /// <summary>
        /// Resource creation date
        /// </summary>
        public DateTime? creation_date { get; set; }

        /// <summary>
        /// Resource publisher email
        /// </summary>

        public string publisher_email { get; set; }

        /// <summary>
        /// True if the resource must be published in the home of the community
        /// </summary>
        public bool publish_home { get; set; }

        /// <summary>
        /// Path of the resource main image
        /// </summary>
        public string main_image { get; set; }

        /// <summary>
        /// True if it's the end of the load
        /// </summary>
        public bool end_of_load { get; set; }

        /// <summary>
        /// True if the resource must be versioned
        /// </summary>
        public bool create_version { get; set; }

        /// <summary>
        /// The canonical url for the resource
        /// </summary>
        public string canonical_url { get; set; }

        /// <summary>
        /// aumented reading
        /// </summary>
        public AumentedReading aumented_reading { get; set; }

    }

    /// <summary>
    /// Parameters to modify a resource subtype
    /// </summary>
    public class ModifyResourceSubtype
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Ontology name of the resource to modify
        /// </summary>
        public string ontology_name { get; set; }

        /// <summary>
        /// Subtype of the resource to modify
        /// </summary>
        public string subtype { get; set; }

        /// <summary>
        /// Previous type of the resource
        /// </summary>
        public string previous_type { get; set; }

        /// <summary>
        /// The user that try to modify the resource
        /// </summary>
        public Guid? user_id { get; set; }
    }

    /// <summary>
    /// Parameters to create a massive data load
    /// </summary>
    public class MassiveDataLoadResource
    {
        /// <summary>
        /// Load identifier
        /// </summary>
        public Guid load_id { get; set; }
        /// <summary>
        /// Load name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// State of the data load
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// Date creation of the data load
        /// </summary>
        public DateTime date_create { get; set; }
        /// <summary>
        /// Project identifier
        /// </summary>
        public Guid project_id { get; set; }
        /// <summary>
        /// Identity identifier
        /// </summary>
        public Guid identity_id { get; set; }
        /// <summary>
        /// Short name of the community
        /// </summary>
        public string community_name { get; set; }
        /// <summary>
        /// Ontology name 
        /// </summary>
        public string ontology { get; set; }
    }

    /// <summary>
    /// Parameters to create a massive data load test
    /// </summary>
    public class MassiveDataLoadTestResource
    {
        /// <summary>
        /// Url
        /// </summary>
        [Required]
        public string url { get; set; }
        /// <summary>
        /// File hash
        /// </summary>
        [Required]
        public byte[] fileHash { get; set; }
    }

    /// <summary>
    /// Parameters of a package of data for a massive data load
    /// </summary>
    public class MassiveDataLoadPackageResource
    {
        /// <summary>
        /// Package identifier
        /// </summary>
        public Guid package_id { get; set; }
        /// <summary>
        /// Load identifier
        /// </summary>
        public Guid load_id { get; set; }
        /// <summary>
        /// Ontology file rute
        /// </summary>
        public string ontology_rute { get; set; }
        /// <summary>
        /// Search graph file rute
        /// </summary>
        public string search_rute { get; set; }
        /// <summary>
        /// SQL file rute
        /// </summary>
        public string sql_rute { get; set; }
        /// <summary>
        /// Onotology bytes
        /// </summary>
        public byte[] ontology_bytes { get; set; }
        /// <summary>
        /// Search graph file bytes
        /// </summary>
        public byte[] search_bytes { get; set; }
        /// <summary>
        /// SQL file bytes
        /// </summary>
        public byte[] sql_bytes { get; set; }
        /// <summary>
        /// State of the package
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// Error in processing the package
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// Date of creation the package
        /// </summary>
        public DateTime date_creation { get; set; }
        /// <summary>
        /// Date when the package is processed.
        /// </summary>
        public DateTime? date_processing { get; set; }
        /// <summary>
        /// The data is compress
        /// </summary>
        public bool comprimido { get; set; }
        /// <summary>
        /// The package is the last one
        /// </summary>
        public bool isLast { get; set; }
    }
    /// <summary>
    /// Model for closing the massive data load
    /// </summary>
    public class CloseMassiveDataLoadResource
    {
        /// <summary>
        /// Data load identifier
        /// </summary>
        public Guid DataLoadIdentifier { get; set; }
    }
    /// <summary>
    /// Parameters to upload a resource
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>        
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource title
        /// </summary>        
        public string title { get; set; }

        /// <summary>
        /// Resource description
        /// </summary>        
        public string description { get; set; }

        /// <summary>
        /// Resource tags
        /// </summary>        
        public List<string> tags { get; set; }

        /// <summary>
        /// Resource categories
        /// </summary>
        public List<Guid> categories { get; set; }

        /// <summary>
        /// Resource type
        /// </summary>        
        public short resource_type { get; set; }

        /// <summary>
        /// Resource url
        /// </summary>
        public string resource_url { get; set; }

        /// <summary>
        /// Resource attached files
        /// </summary>
        public List<SemanticAttachedResource> resource_attached_files { get; set; }

        /// <summary>
        /// Resource authors (comma separated)
        /// </summary>
        public string authors { get; set; }

        /// <summary>
        /// Url to make a screenshot
        /// </summary>
        public string url_screenshot { get; set; }

        /// <summary>
        /// Screenshot predicate
        /// </summary>
        public string predicate_screenshot { get; set; }

        /// <summary>
        /// Screenshot posible sizes
        /// </summary>
        public List<int> screenshot_sizes { get; set; }

        /// <summary>
        /// Resource visibility <see cref="ResourceVisibility"/> enumeration
        /// </summary>
        public short visibility { get; set; }

        /// <summary>
        /// Ontology
        /// </summary>
        public string ontology { get; set; }

        /// <summary>
        /// Resource readers list
        /// </summary>
        public List<ReaderEditor> readers_list { get; set; }

        /// <summary>
        /// Resource editors list
        /// </summary>
        public List<ReaderEditor> editors_list { get; set; }

        /// <summary>
        /// Resource creation date
        /// </summary>
        public DateTime? creation_date { get; set; }

        /// <summary>
        /// Path of the resource main image
        /// </summary>
        public string main_image { get; set; }

        /// <summary>
        /// Object for increased reading
        /// </summary>
        public AumentedReading lecturaAumentada { get; set; }

        /// <summary>
        /// Resource link
        /// </summary>
        public string link { get; set; }

    }

    /// <summary>
    /// Model of the aumented reading
    /// </summary>
    public class AumentedReading
    {
        /// <summary>
        /// increased-reading title
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// increased-reading description
        /// </summary>
        public string description { get; set; }

    }

    /// <summary>
    /// Represents a attached file to a complex ontology resource
    /// </summary>
    public class SemanticAttachedResource
    {
        /// <summary>
        /// Property of the file
        /// </summary>
        public string file_rdf_property { get; set; }

        /// <summary>
        /// Property type
        /// </summary>
        public short file_property_type { get; set; }

        /// <summary>
        /// Bytes of the attached file
        /// </summary>
        public byte[] rdf_attached_file { get; set; }

        /// <summary>
        /// True if the file must be deleted
        /// </summary>
        public bool delete_file { get; set; }
    }

    /// <summary>
    /// Parameters to modify a resource triple list
    /// </summary>
    public class ModifyResourceTripleListParams
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Triples to modify
        /// </summary>
        public List<ModifyResourceTriple> resource_triples { get; set; }

        /// <summary>
        /// Attached files
        /// </summary>
        public List<SemanticAttachedResource> resource_attached_files { get; set; }

        /// <summary>
        /// True if the resource must be published in the home of the community
        /// </summary>
        public bool publish_home { get; set; }

        /// <summary>
        /// Resource main image
        /// </summary>
        public string main_image { get; set; }
        
        /// <summary>
        /// True if it's the end of the load and must delete the cache
        /// </summary>
        public bool end_of_load { get; set; }

        /// <summary>
        /// User that try to modify the resource
        /// </summary>
        public Guid? user_id { get; set; }
    }

    /// <summary>
    /// Model of massive load
    /// </summary>
    public class MassiveResourceLoad
    {
        /// <summary>
        /// List of resource to load
        /// </summary>
        public List<LoadResourceParams> resources { get; set; }
        /// <summary>
        /// Identifier of the load
        /// </summary>
        public Guid load_id { get; set; }
    }

    /// <summary>
    /// Parameters to modify a triple of a resource
    /// </summary>
    public class ModifyResourceTriple
    {
        /// <summary>
        /// Triple predicate
        /// </summary>
        public string predicate { get; set; }

        /// <summary>
        /// Old value of the object
        /// </summary>
        public string old_object { get; set; }

        /// <summary>
        /// New value of the object
        /// </summary>
        public string new_object { get; set; }

        /// <summary>
        /// Complete when the predicate is the resource title or description 
        /// </summary>
        public GnossResourceProperty gnoss_property { get; set; }
    }

    /// <summary>
    /// Parameters to modify a resource property
    /// </summary>
    public class ModifyResourceProperty
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identifier
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Property to modify
        /// </summary>
        public string property { get; set; }

        /// <summary>
        /// New value of the object
        /// </summary>
        public string new_object { get; set; }
    }

    /// <summary>
    /// Properties of a resource
    /// </summary>
    public class ResourceNoveltiesModel
    {
        /// <summary>
        /// Community short name
        /// </summary>

        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Resource description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Resource tags
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// Resource categories
        /// </summary>
        public List<Guid> categories { get; set; }

        /// <summary>
        /// Resource type
        /// </summary>
        public short resource_type { get; set; }

        /// <summary>
        /// Resource type names in different languages
        /// </summary>
        public List<ResourceTypeName> resource_type_names { get; set; }

        /// <summary>
        /// Item type names in different languages
        /// </summary>
        public List<ItemTypeName> item_type_names { get; set; }

        /// <summary>
        /// Resource url
        /// </summary>
        public string resource_url { get; set; }

        /// <summary>
        /// Resource authors (comma separated)
        /// </summary>
        public string authors { get; set; }

        /// <summary>
        /// Resource visibility <see cref="ResourceVisibility"/> enumeration
        /// </summary>
        public short visibility { get; set; }

        /// <summary>
        /// Resource readers list
        /// </summary>
        public List<ReaderEditor> readers_list { get; set; }

        /// <summary>
        /// Resource editors list
        /// </summary>
        public List<ReaderEditor> editors_list { get; set; }

        /// <summary>
        /// Resource creation date in ISO 8601 format
        /// </summary>
        public DateTime? creation_date { get; set; }

        /// <summary>
        /// Resource last edition date in ISO 8601 format
        /// </summary>
        public DateTime? last_edition_date { get; set; }

        /// <summary>
        /// Resource edition dates in ISO 8601 format by user. <see cref="ResourceEditionDateByUser"/>
        /// </summary>
        public List<ResourceEditionDateByUser> edition_date_by_user { get; set; }

        /// <summary>
        /// Resource version dates in ISO 8601 format by user. <see cref="ResourceVersionDateByUser"/>
        /// </summary>
        public List<ResourceVersionDateByUser> version_date_by_user { get; set; }

        /// <summary>
        /// Resource deleted date in ISO 8601 format by user. <see cref="ResourceDeleteDateByUser"/>
        /// </summary>
        public ResourceDeleteDateByUser delete_date_by_user { get; set; }

        /// <summary>
        /// Path of the resource main image
        /// </summary>
        public string main_image { get; set; }

        /// <summary>
        /// Resource version dates in ISO 8601 format by user. <see cref="ResourceVersionDateByUser"/>
        /// </summary>
        public List<CommentModel> comments { get; set; }

        /// <summary>
        /// Resource number of views
        /// </summary>
        public int views { get; set; }

        /// <summary>
        /// Resource number of plays
        /// </summary>
        public int plays { get; set; }

        /// <summary>
        /// Resource number of downloads
        /// </summary>
        public int downloads { get; set; }

        /// <summary>
        /// Resource last view date in ISO 8601 format
        /// </summary>
        public DateTime? last_view_date { get; set; }

        /// <summary>
        /// Resource votes. <see cref="VoteModel"/>
        /// </summary>
        public List<VoteModel> votes { get; set; }

        /// <summary>
        /// Resource sharing. <see cref="ShareModel"/>
        /// </summary>
        public List<ShareModel> shared_on { get; set; }

        /// <summary>
        /// Resource saved to user personal space. <see cref="PersonalSpaceModel"/>
        /// </summary>
        public List<PersonalSpaceModel> personal_spaces { get; set; }

        /// <summary>
        /// If the resource is a link to a web page, this property gets or sets the url of the web page
        /// </summary>
        public string link { get; set; }
    }

    /// <summary>
    /// Define the name of the resource type in different languages
    /// </summary>
    public class ResourceTypeName
    {
        /// <summary>
        /// Resource type name
        /// </summary>
        public string resource_type_name { get; set; }

        /// <summary>
        /// Language of the type name
        /// </summary>
        public string resource_type_name_language { get; set; }
    }

    /// <summary>
    /// Define the name of the resource type in different languages
    /// </summary>
    public class ItemTypeName
    {
        /// <summary>
        /// Resource type name
        /// </summary>
        public string item_type_name { get; set; }

        /// <summary>
        /// Language of the type name
        /// </summary>
        public string item_type_name_language { get; set; }
    }

    /// <summary>
    /// Properties of the resource edition
    /// </summary>
    public class ResourceEditionDateByUser
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource edition date by user in ISO 8601 format
        /// </summary>
        public DateTime? edition_date { get; set; }

        /// <summary>
        /// User identificator who has edited the resource
        /// </summary>
        public Guid user_id { get; set; }
    }

    /// <summary>
    /// Properties of the resource version
    /// </summary>
    public class ResourceVersionDateByUser
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource version date by user in ISO 8601 format
        /// </summary>
        public DateTime? version_date { get; set; }

        /// <summary>
        /// User identificator who has versioned the resource
        /// </summary>
        public Guid user_id { get; set; }
    }

    /// <summary>
    /// Properties of the resource deleted
    /// </summary>
    public class ResourceDeleteDateByUser
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Resource deleted date by user in ISO 8601 format
        /// </summary>
        public DateTime? delete_date { get; set; }

        /// <summary>
        /// User identificator who has deleted the resource
        /// </summary>
        public Guid user_id { get; set; }
    }

    /// <summary>
    /// Properties of a comment
    /// </summary>
    public class CommentModel
    {
        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// User identificator who has commented the resource
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Description of the comment
        /// </summary>
        public string html_description { get; set; }

        /// <summary>
        /// Parent comment identificator (if the comment is an answer to another comment)
        /// </summary>
        public Guid parent_comment_id { get; set; }

        /// <summary>
        /// Comment creation date
        /// </summary>
        public DateTime comment_date { get; set; }

    }

    /// <summary>
    /// Properties of a vote
    /// </summary>
    public class VoteModel
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// User identificator who has voted the resource
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Vote date
        /// </summary>
        public DateTime vote_date { get; set; }
    }

    /// <summary>
    /// Properties of sharing
    /// </summary>
    public class ShareModel
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// Origin community short name
        /// </summary>
        public string origin_community_short_name { get; set; }

        /// <summary>
        /// Destiny community short name
        /// </summary>
        public string destiny_community_short_name { get; set; }

        /// <summary>
        /// User identificator who has shared the resource
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Sharing date
        /// </summary>
        public DateTime share_date { get; set; }
    }

    /// <summary>
    /// Properties of saving the resource at personal space
    /// </summary>
    public class PersonalSpaceModel
    {
        /// <summary>
        /// Resource identificator
        /// </summary>
        public Guid resource_id { get; set; }

        /// <summary>
        /// User identificator who has saved the resource in his personal space
        /// </summary>
        public Guid user_id { get; set; }

        /// <summary>
        /// Saved in personal space date
        /// </summary>
        public DateTime saved_date { get; set; }
    }


    /// <summary>
    /// Model that generate labels from title and description
    /// </summary>
    public class TagsFromServiceModel
    {
        /// <summary>
        /// Title for generate the labels
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Description for generate the labels
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Short name of the project
        /// </summary>
        public string community_short_name { get; set; }
    }
}