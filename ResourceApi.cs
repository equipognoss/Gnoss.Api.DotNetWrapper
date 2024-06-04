using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.OAuth;
using System.Data;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Gnoss.ApiWrapper.Exceptions;
using Gnoss.ApiWrapper.Web;
using Gnoss.ApiWrapper.ApiModel;
using System.Xml;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Gnoss.ApiWrapper
{
    /// <summary>
    /// Wrapper for GNOSS resource API
    /// </summary>
    public class ResourceApi : GnossApiWrapper
    {
        #region Members

        private IHttpContextAccessor _httpContextAccessor;
        private static string _ontologyUrl;
        private string _ontologyNameWithoutExtension;
        private CommunityApi _communityApi;
        private string _loadIdentifier;
        private const int _DEFAULT_LOCK_DURATION = 60;
        private const int _DEFAULT_TIMEOUT_LOCK = 60;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="ResourceApi"/>
        /// </summary>
        /// <param name="communityShortName">Community short name which you want to use the API</param>
        /// <param name="oauth">OAuth information to sign the Api requests</param>
        /// <param name="developerEmail">(Optional) If you want to be informed of any incident that may happends during a large load of resources, an email will be sent to this email address</param>
        /// <param name="ontologyName">(Optional) Ontology name of the resources that you are going to query, upload or modify</param>
        public ResourceApi(OAuthInfo oauth, ILogHelper logHelper = null) : base(oauth, logHelper)
        {
            LoadApi();
        }

        /// <summary>
        /// Consturtor of <see cref="ResourceApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public ResourceApi(string configFilePath) : base(configFilePath)
        {
        }

        /// <summary>
        /// Consturtor of <see cref="ResourceApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public ResourceApi() : base()
        {
        }

        #endregion

        #region Public methods

        #region Complex semantic resource methods

        #region Load

        /// <summary>
        /// Load a complex semantic resources list
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void LoadComplexSemanticResourceList(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadComplexSemanticResourceListInt(resourceList, hierarquicalCategories, numAttemps);
        }

        /// <summary>
        /// Load resources of main entities in an otology and a community
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarchycalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="ontology">Ontology where resource will be loaded</param>
        public void LoadComplexSemanticResourceListWithOntologyAndCommunity(List<ComplexOntologyResource> resourceList, bool hierarchycalCategories, string ontology)
        {
            LoadComplexSemanticResourceListWithOntologyAndCommunityInt(resourceList, hierarchycalCategories, ontology);
        }

        /// <summary>
        /// Load a complex semantic resources list
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        /// <param name="rdfsPath">Default null. Path to save the RDF, if necessary</param>
        public void LoadComplexSemanticResourceListSavingLocalRdf(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5, string rdfsPath = null)
        {
            LoadComplexSemanticResourceListInt(resourceList, hierarquicalCategories, numAttemps, rdfsPath);
        }

        /// <summary>
        /// Load resources of main entities in a community
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        /// <param name="communityShortName">Default null. Defined if it is necessary the load in other community that the specified in the OAuth</param>
        public void LoadComplexSemanticResourceListCommunityShortName(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadComplexSemanticResourceListInt(resourceList, hierarquicalCategories, numAttemps);
        }

        /// <summary>
        /// Load a complex semantic resource <see cref="ComplexOntologyResource"/>
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        /// <returns>Resource identifier string</returns>
        public string LoadComplexSemanticResource(ComplexOntologyResource resource, bool hierarquicalCategories = false, bool isLast = false, int numAttemps = 5)
        {
            return LoadComplexSemanticResourceInt(resource, hierarquicalCategories, isLast, numAttemps);
        }

        /// <summary>
        /// Load a complex semantic resource <see cref="ComplexOntologyResource"/> saving the resource rdf
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="rdfsPath">Default null. Path to save the RDF, if necessary</param>
        /// <returns>Resource identifier string</returns>
        public string LoadComplexSemanticResourceSaveRdf(ComplexOntologyResource resource, string rdfsPath, bool hierarquicalCategories = false, bool isLast = false, int numAttemps = 2)
        {
            if (string.IsNullOrEmpty(rdfsPath) || string.IsNullOrWhiteSpace(rdfsPath))
            {
                throw new GnossAPIArgumentException($"You must set the parameter rdfsPath");
            }

            return LoadComplexSemanticResourceInt(resource, hierarquicalCategories, isLast, numAttemps, rdfsPath);
        }

        /// <summary>
        /// Load a complex semantic resource in the community
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="communityShortName">Default null. Defined if it is necessary the load in other community that the specified in the OAuth</param>
        /// <returns>Resource identifier string</returns>
        public string LoadComplexSemanticResourceCommunityShortName(ComplexOntologyResource resource, bool hierarquicalCategories = false, bool isLast = false, int numAttemps = 2)
        {
            return LoadComplexSemanticResourceInt(resource, hierarquicalCategories, isLast, numAttemps);
        }

        /// <summary>
        /// Load a complex semantic resource in the community with a rdf file
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        /// <param name="rdfsPath">Default null. Path to save the RDF, if necessary</param>
        /// <param name="communityShortName">Default null. Defined if it is necessary the load in other community that the specified in the OAuth</param>
        /// <param name="rdfFile">Resource rdf file</param>
        /// <returns>Resource identifier string</returns>
        public string LoadComplexSemanticResourceRdf(ComplexOntologyResource resource, byte[] rdfFile, bool hierarquicalCategories = false, bool isLast = false, int numAttemps = 5, string rdfsPath = null)
        {
            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                string documentId = string.Empty;

                resource.RdfFile = rdfFile;
                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                documentId = CreateComplexOntologyResource(model);
                resource.Uploaded = true;

                Log.Trace($"Loaded: \tID: {resource.ShortGnossId}\tTitle: {resource.Title}\tResourceID: {resource.GnossId}");
                if (resource.ShortGnossId != Guid.Empty && documentId != resource.GnossId)
                {
                    Log.Trace($"Resource loaded with the id: {documentId}\nThe IDGnoss provided to the method is different from the returned by the API", this.GetType().Name);
                }

                if (!string.IsNullOrEmpty(rdfsPath) && !string.IsNullOrWhiteSpace(rdfsPath))
                {
                    if (!Directory.Exists($"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}"))
                    {
                        Directory.CreateDirectory($"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}");
                    }

                    string ficheroRDF = $"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}/{resource.ShortGnossId}.rdf";
                    if (!File.Exists(ficheroRDF))
                    {
                        File.WriteAllText(ficheroRDF, resource.StringRdfFile);
                    }
                }

                resource.GnossId = documentId;
            }
            catch (GnossAPICategoryException gacex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {gacex.Message}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }
            return resource.GnossId;
        }

        /// <summary>
        /// Loads a <see cref="SecondaryResource"/>
        /// </summary>
        /// <param name="resource">The <see cref="SecondaryResource"/> to load</param>
        /// <returns>Bool that indicates whether the resource has been correctly loaded</returns>
        public bool LoadSecondaryResource(SecondaryResource resource)
        {
            bool success = false;

            try
            {
                CreateSecondaryEntity(resource.SecondaryOntology.OntologyUrl, CommunityShortName, resource.RdfFile);
                Log.Debug($"Loaded secondary resource with ID: {resource.SecondaryOntology.Identifier}", this.GetType().Name);
                success = true;
                resource.Uploaded = true;
                resource.Id = $"{GraphsUrl}items/{resource.SecondaryOntology.Identifier}";
            }
            catch (Exception ex)
            {
                Log.Error($"Resource has not been loaded the secondary resource with ID: {resource.Id}. Mensaje: {ex.Message}", this.GetType().Name);
            }
            return success;
        }

        /// <summary>
        /// Loads a list of <see cref="SecondaryResource"/>
        /// </summary>
        /// <param name="resourceList">List of <see cref="SecondaryResource"/> to load</param>
        public void LoadSecondaryResourceList(List<SecondaryResource> resourceList)
        {
            foreach (SecondaryResource rs in resourceList)
            {
                SecondaryResource rec = rs;
                LoadSecondaryResource(rec);
            }
        }

        /// <summary>
        /// Loads the attached files of a not yet loaded resource.
        /// </summary>
        /// <param name="resource">
        /// community_short_name = community short name string
        /// resource_id = resource identifier guid
        /// resource_attached_files = resource attached files. List of SemanticAttachedResource
        /// SemanticAttachedResource:
        ///     file_rdf_properties = image name
        ///     file_property_type = type of file
        ///     rdf_attached_files = image to load byte[]
        /// main_image = main image string
        /// </param>
        [Obsolete("MassiveUploadFiles is deprecated, please use UploadImages instead.")]
        public void MassiveUploadFiles(LoadResourceParams resource)
        {
            try
            {
                UploadImages(resource.resource_id, resource.resource_attached_files, resource.main_image);

                string imageId = null;

                if (resource.resource_attached_files != null && resource.resource_attached_files.Count() > 0)
                {
                    imageId = resource.resource_attached_files[0].file_rdf_property;
                }

                Log.Debug($"Massive uploading files correct of the resource '{imageId}' del recurso {resource.resource_id}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error uploading files of the resource{resource.resource_id} {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the attached images of a not yet loaded resource.
        /// </summary>
        /// <param name="resource">
        /// community_short_name = community short name string
        /// resource_id = resource identifier guid
        /// resource_attached_files = resource attached files. List of SemanticAttachedResource
        /// SemanticAttachedResource:
        ///     file_rdf_properties = image name
        ///     file_property_type = type of file
        ///     rdf_attached_files = image to load byte[]
        /// main_image = main image string
        /// </param>
        [Obsolete("MassiveUploadImages is deprecated, please use UploadImages instead.")]
        public void MassiveUploadImages(LoadResourceParams resource)
        {
            try
            {
                UploadImages(resource.resource_id, resource.resource_attached_files, resource.main_image);

                Log.Debug($"Massive uploading images correct of the resource '{resource.resource_id}'");
            }
            catch (Exception ex)
            {
                Log.Error($"Error uploading images of the resource {resource.resource_id} {ex.Message}");
                throw new GnossAPIException($"Error uploading images of the resource {resource.resource_id} {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a partitioned xml of the ontology.
        /// </summary>
        public bool LoadPartitionedXmlOntology(byte[] xmlFile, string fileName)
        {
            try
            {
                string url = $"{ApiUrl}/ontology/upload-partitioned-xml";

                FileOntology model = new FileOntology() { community_short_name = CommunityShortName, ontology_name = OntologyUrl, file_name = fileName, file = xmlFile };

                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"The xml file {fileName} of the ontology {OntologyUrl} has been upload in the communtiy {CommunityShortName}");

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"El XML {fileName} no se ha subido.{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads a partitioned ontology
        /// </summary>
        /// <param name="ontologyFile">Ontology file</param>
        /// <param name="fileName">File name</param>
        public bool LoadPartitionedOntology(byte[] ontologyFile, string fileName)
        {
            try
            {
                string url = $"{ApiUrl}/ontology/upload-partitioned-ontology";
                FileOntology model = new FileOntology() { community_short_name = CommunityShortName, ontology_name = OntologyUrl, file_name = fileName, file = ontologyFile };
                WebRequestPostWithJsonObject(url, model);
                Log.Debug($"The file of the ontology has been upload in the communtiy {CommunityShortName}");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"El file {fileName} has not been uploaded.{e.Message}");
                return false;
            }
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modifies the complex ontology resource
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        public void ModifyComplexOntologyResource(ComplexOntologyResource resource, bool hierarquicalCategories, bool isLast)
        {
            Log.Trace($"******************** Begin the resource modification: {resource.GnossId}", this.GetType().Name, CommunityShortName);

            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                resource.Modified = ModifyComplexOntologyResource(model);

                if (resource.Modified)
                {
                    Log.Debug($"Successfully modified the resource with id: {resource.Id} and Gnoss identifier {resource.ShortGnossId} belonging to the ontology '{resource.Ontology.OntologyUrl}' with RdfType = '{resource.Ontology.RdfType}'", this.GetType().Name);
                }
                else
                {
                    Log.Error($"The resource with id: {resource.ShortGnossId} of the ontology '{resource.Ontology.OntologyUrl}' has not been modified.", this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"The resource with id {resource.GnossId} has not been modified,{ex.Message}");
            }
        }

        /// <summary>
        /// Modifies the complex ontology resource with the rdf
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="rdfFile">Resource rdf file</param>
        public void ModifyComplexOntologyResourceRDF(ComplexOntologyResource resource, byte[] rdfFile, bool hierarquicalCategories, bool isLast)
        {
            Log.Trace($"******************** Begin the resource modification: {resource.GnossId}", this.GetType().Name, CommunityShortName);

            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                resource.RdfFile = rdfFile;
                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                resource.Modified = ModifyComplexOntologyResource(model);

                if (resource.Modified)
                {
                    Log.Debug($"Successfully modified the resource with id: {resource.ShortGnossId}");
                }
                else
                {
                    Log.Error($"ERROR --> The resource with id: {resource.ShortGnossId}' has not been modified.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR --> The resource with id: {resource.ShortGnossId}' has not been modified.{ex.Message}");
            }
        }

        /// <summary>
        /// Modifies the complex ontology resource
        /// </summary>
        /// <param name="resource">cccc</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="communityShortName">Community short name where the resources will be loaded</param>
        public void ModifyComplexSemanticResourceCommunityShortName(ComplexOntologyResource resource, bool hierarquicalCategories, bool isLast)
        {
            Log.Trace($"******************** Begin modification of resource: {resource.GnossId}", this.GetType().Name, CommunityShortName);
            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                resource.Modified = ModifyComplexOntologyResource(model);

                if (resource.Modified)
                {
                    Log.Debug($"Successfully modified the resource with id: {resource.Id} and Gnoss identifier {resource.ShortGnossId} belonging to the ontology '{resource.Ontology.OntologyUrl}' and RdfType = '{resource.Ontology.RdfType}'", this.GetType().Name);
                }
                else
                {
                    Log.Error($"The resource with id: {resource.ShortGnossId} of the ontology '{resource.Ontology.OntologyUrl}' has not been modified.", this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"The resource with id{resource.GnossId} has not been modified {ex.Message}");
            }
        }

        /// <summary>
        /// Modifies the basic ontology resource
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        public void ModifyBasicOntologyResource(BasicOntologyResource resource, bool hierarquicalCategories, bool isLast)
        {
            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                LoadResourceParams model = GetResourceModelOfBasicOntologyResource(resource, isLast);
                resource.Uploaded = ModifyBasicOntologyResource(model);
            }
            catch (Exception ex)
            {
                Log.Error($"The basic ontology resource with id: {resource.GnossId} has not been modified,{ex.Message}");
            }
        }

        /// <summary>
        /// Modifies the basic ontology resource of the list <paramref name="resourceList"/>. It is necessary that the basic ontology resource has assigned the property <see cref="BaseResource.GnossId"/>
        /// </summary>
        /// <param name="resourceList">Resource list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        public void ModifyBasicOntologyResourceList(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 2)
        {
            List<BasicOntologyResource> originalResourceList = new List<BasicOntologyResource>(resourceList);
            List<BasicOntologyResource> resorucesToModify = new List<BasicOntologyResource>(resourceList);
            int processedNumber = 0;
            int attempNumber = 0;

            while (resorucesToModify != null && resorucesToModify.Count > 0 && attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                }

                foreach (BasicOntologyResource rec in resourceList)
                {
                    processedNumber = processedNumber + 1;

                    try
                    {
                        ModifyBasicOntologyResource(rec, hierarquicalCategories, processedNumber == resourceList.Count());
                        resorucesToModify.Remove(rec);
                    }
                    catch (GnossAPICategoryException gacex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {gacex.Message}", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }
                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Lap number: {attempNumber} finalizada", this.GetType().Name);
                }
            }
        }

        /// <summary>
        /// Modifies the basic ontology resource of the list <paramref name="resourceList"/>. It is necessary that the basic ontology resource has assigned the property <see cref="BaseResource.GnossId"/>
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="ontology">Ontology where resource will be loaded</param>
        /// <param name="communityShortName">Community short name where the resources will be loaded</param>
        /// <param name="numAttemps">Default 1. Number of retries loading of the failed load of a resource</param>
        public void ModifyComplexSemanticResourceListWithOntologyAndCommunity(ref List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, string ontology, int numAttemps = 1)
        {
            int resourcesToModify = resourceList.Where(r => !r.Modified).Count();
            int processedNumber = 0;
            int attempNumber = 0;

            while (resourceList != null && resourceList.Count > 0 && resourcesToModify > 0 && attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                }

                foreach (ComplexOntologyResource rec in resourceList.Where(r => !r.Modified))
                {
                    processedNumber = processedNumber + 1;

                    try
                    {
                        ModifyComplexSemanticResourceWithOntologyAndCommunity(rec, hierarquicalCategories, processedNumber == resourceList.Count(), ontology);

                        resourcesToModify = resourcesToModify - 1;
                        Log.Debug($"There are {resourcesToModify} resources left to modify.");
                    }
                    catch (GnossAPICategoryException gacex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {gacex.Message}", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }
                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Lap number: {attempNumber} finalizada", this.GetType().Name);
                }
            }
        }

        /// <summary>
        /// Modifies the basic ontology resource. It is necessary that the basic ontology resource has assigned the property <see cref="BaseResource.GnossId"/>
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="ontology">Ontology where resource will be loaded</param>
        /// <param name="communityShortName">Community short name where the resources will be loaded</param>
        /// <param name="isLast">There are not resources left to load</param>
        public void ModifyComplexSemanticResourceWithOntologyAndCommunity(ComplexOntologyResource resource, bool hierarquicalCategories, bool isLast, string ontology)
        {
            Log.Trace($"******************** Begin the resource modification: {resource.GnossId}", this.GetType().Name, CommunityShortName);

            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                string ontologyUrl = ontology.ToLower().Replace(".owl", "");
                ontologyUrl = OntologyUrl.Replace(OntologyUrl.Substring(OntologyUrl.LastIndexOf("/") + 1), $"{ontologyUrl}.owl");
                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                model.resource_url = ontologyUrl;
                resource.Modified = ModifyComplexOntologyResource(model);

                if (resource.Modified)
                {
                    Log.Debug($"Successfully modified the resource with id: {resource.ShortGnossId} and Gnoss identifier {resource.ShortGnossId} belonging to the ontology '{ontologyUrl}' and RdfType = '{resource.Ontology.RdfType}'", this.GetType().Name);
                }
                else
                {
                    Log.Error($"The resource with id: {resource.ShortGnossId} of the ontology '{ontologyUrl}' has not been modified.", this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"The resource with id {resource.GnossId} has not been modified, {ex.Message}");
            }
        }

        /// <summary>
        /// Modifies the basic ontology resource. It is necessary that the basic ontology resource has assigned the property <see cref="BaseResource.GnossId"/>
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 1. Number of retries loading of the failed load of a resource</param>
        public void ModifyComplexSemanticResourceList(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 2)
        {
            int resourcesToModify = resourceList.Where(r => !r.Modified).Count();
            int processedNumber = 0;
            int attempNumber = 0;

            while (resourceList != null && resourceList.Count > 0 && resourcesToModify > 0 && attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                }

                foreach (ComplexOntologyResource rec in resourceList.Where(r => !r.Modified))
                {
                    processedNumber = processedNumber + 1;

                    try
                    {
                        ModifyComplexOntologyResource(rec, hierarquicalCategories, processedNumber == resourceList.Count());

                        resourcesToModify = resourcesToModify - 1;
                        Log.Debug($"There are {resourcesToModify} resources left to modify.");
                    }
                    catch (GnossAPICategoryException gacex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {gacex.Message}", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId}. Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }

                if (numAttemps > 1)
                {
                    Log.Trace($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
                }
            }
        }

        /// <summary>
        /// Modifies a property(tags or categories) of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier guid</param>
        /// <param name="property">Property to modify. Categories: 'skos:ConceptID' string mask, Tags: 'sioc_t:Tag' string mask</param>
        /// <param name="listToModify">Values list to set the property with them</param>
        /// <param name="hierarchicalCategories">Indicates whether the categories has hierarchy</param>
        public void ModifyCategoriesTagsComplexOntologyResource(Guid resourceId, string property, List<string> listToModify, bool hierarchicalCategories)
        {
            Log.Debug($"******************** Start modification: ", this.GetType().Name);

            List<Guid> categoriesIdentifiers = null;

            string newObject = string.Empty;
            if (listToModify != null && property.Equals($"skos:ConceptID"))
            {
                categoriesIdentifiers = new List<Guid>();
                if (hierarchicalCategories)
                {
                    categoriesIdentifiers = GetHierarquicalCategoriesIdentifiersList(listToModify);
                }
                else
                {
                    categoriesIdentifiers = GetNotHierarquicalCategoriesIdentifiersList(listToModify);
                }
            }
            else
            {
                if (listToModify != null && property.Equals($"sioc_t:Tag"))
                {
                    foreach (string identificador in listToModify)
                    {
                        if (!string.IsNullOrEmpty(identificador))
                        {
                            newObject += $"{identificador.Trim()},";
                        }
                    }
                }
            }

            if (categoriesIdentifiers != null)
            {
                foreach (Guid identifier in categoriesIdentifiers)
                {
                    if (identifier.Equals(Guid.Empty))
                    {
                        newObject = $"{identifier},";
                    }
                    else
                    {
                        newObject += $"{identifier},";
                    }
                }
            }

            ModifyProperty(resourceId, property, newObject);
            Log.Debug($"The resource '{resourceId}' has been modified correctly....{DateTime.Now}");
            Log.Debug($"******************** End modification", this.GetType().Name);
        }

        /// <summary>
        /// Modifies the complex ontology resource generating a new rdf, and also changes the physical image with the new image
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void ModifyComplexOntologyResourceWithImages(ComplexOntologyResource resource, bool hierarquicalCategories, int numAttemps = 5)
        {
            int attempNumber = 0;

            while (attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Begin the lap number: {attempNumber}", this.GetType().Name);

                try
                {
                    if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                    {
                        if (hierarquicalCategories)
                        {
                            resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                        }
                        else
                        {
                            resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                        }
                    }

                    LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, true);
                    resource.Modified = ModifyComplexOntologyResource(model);

                    Log.Debug($"Successfully modified the resource with ID: {resource.ShortGnossId} and Gnoss identifier {resource.GnossId}", this.GetType().Name);
                    if (resource.Modified)
                    {
                        try
                        {
                            OntologyProperty propOntoImage = resource.Ontology.Properties.Where(po => po.GetType().Equals(DataTypes.OntologyPropertyImage)).Distinct().ToList()[0];

                            string prefijoPredicado = propOntoImage.Name.Split(CharArrayDelimiters.Colon)[0];
                            string nombreEtiquetaSinPrefijo = propOntoImage.Name.Split(CharArrayDelimiters.Colon)[1];
                            string nombreImagen = propOntoImage.Value.ToString();
                            string predicado = resource.Ontology.PrefixList.Where(prefijo => prefijo.Contains($"xmlns:{prefijoPredicado}")).ToList()[0].Split(CharArrayDelimiters.Equal)[1].ToString().Replace("\"", "") + nombreEtiquetaSinPrefijo;

                            List<ModifyResourceTriple> triplesList = new List<ModifyResourceTriple>();
                            ModifyResourceTriple triple = new ModifyResourceTriple() { old_object = string.Empty, predicate = predicado, new_object = nombreImagen, gnoss_property = GnossResourceProperty.none };
                            triplesList.Add(triple);

                            List<SemanticAttachedResource> resourceAttachedFiles = new List<SemanticAttachedResource>();
                            if (resource.AttachedFilesName.Count > 0)
                            {
                                int i = 0;
                                foreach (string name in resource.AttachedFilesName)
                                {
                                    SemanticAttachedResource adjunto = new SemanticAttachedResource();
                                    adjunto.file_rdf_property = name;
                                    adjunto.file_property_type = (short)resource.AttachedFilesType[i];
                                    adjunto.rdf_attached_file = resource.AttachedFiles[i];
                                    adjunto.delete_file = resource.AttachedFiles[i].Equals(null);
                                    i++;
                                    resourceAttachedFiles.Add(adjunto);
                                }
                            }

                            ModifyTripleList(resource.ShortGnossId, triplesList, resource.PublishInHome, resource.MainImage, resourceAttachedFiles, true);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"ERROR replacing the image of the resource: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
                        }
                    }
                    else
                    {
                        Log.Error($"The resource with id: {resource.GnossId} has not been modified.", this.GetType().Name);
                    }
                }
                catch (GnossAPICategoryException gacex)
                {
                    Log.Error($"ERROR at: {resource.ShortGnossId} . Title: {resource.Title}. Message: {gacex.Message}", this.GetType().Name);
                }
                catch (Exception ex)
                {
                    Log.Error($"ERROR at: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
                }

                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
            }
        }

        /// <summary>
        /// Removes attached files of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier guid</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="removeTripleList">Resource triples list to modify</param>
        public void RemoveResourceAttachedFiles(Guid resourceId, List<RemoveTriples> removeTripleList, bool publishHome = false)
        {
            int numTriples = removeTripleList.Count();
            string attachedValue = string.Empty;
            string attachedPredicate = string.Empty;
            short attachedObjectType = -1;

            if (!resourceId.Equals(Guid.Empty))
            {
                List<ModifyResourceTriple> triplesList = new List<ModifyResourceTriple>();
                List<SemanticAttachedResource> resourceAttachedFiles = new List<SemanticAttachedResource>();

                foreach (RemoveTriples tripleToDelete in removeTripleList)
                {
                    if (tripleToDelete != null)
                    {
                        attachedValue = tripleToDelete.Value;
                        attachedPredicate = tripleToDelete.Predicate;
                        attachedObjectType = tripleToDelete.ObjectType;

                        if (!string.IsNullOrEmpty(attachedValue) && !string.IsNullOrEmpty(attachedPredicate) && attachedObjectType != -1)
                        {
                            ModifyResourceTriple triple = new ModifyResourceTriple() { old_object = attachedValue, predicate = attachedPredicate, new_object = string.Empty, gnoss_property = GnossResourceProperty.none };
                            triplesList.Add(triple);

                            SemanticAttachedResource attach = new SemanticAttachedResource() { file_rdf_property = attachedValue, file_property_type = attachedObjectType, rdf_attached_file = null, delete_file = true };
                            resourceAttachedFiles.Add(attach);
                        }
                    }
                }

                if (removeTripleList.Count > 0)
                {
                    ModifyTripleList(resourceId, triplesList, publishHome, null, resourceAttachedFiles, true);
                    Log.Debug($"Modified resource with attached. ResourceId: {resourceId}");
                }
            }
        }

        /// <summary>
        /// Attaches a file to a semantic resource.
        /// </summary>
        /// <param name="resourceId">Resource identifier guid</param>
        /// <param name="filePredicate">Resource predicate on the ontology</param>
        /// <param name="fileName">File name to attach</param>
        /// <param name="fileRdfPropertiesList">Attached files names list</param>
        /// <param name="filePropertiesTypeList">It indicates whether the attachment is a file or a ArchivoLink</param>
        /// <param name="attachedFilesList">Attached files list</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        public void AttachFileToResource(Guid resourceId, string filePredicate, string fileName, List<string> fileRdfPropertiesList, List<short> filePropertiesTypeList, List<byte[]> attachedFilesList, bool publishHome = false)
        {
            List<ModifyResourceTriple> triplesList = new List<ModifyResourceTriple>();
            List<SemanticAttachedResource> resourceAttachedFiles = new List<SemanticAttachedResource>();

            ModifyResourceTriple triple = new ModifyResourceTriple() { old_object = string.Empty, predicate = filePredicate, new_object = fileName, gnoss_property = GnossResourceProperty.none };
            triplesList.Add(triple);

            if (fileRdfPropertiesList.Count > 0)
            {
                int i = 0;
                foreach (string name in fileRdfPropertiesList)
                {
                    SemanticAttachedResource attach = new SemanticAttachedResource();
                    attach.file_rdf_property = name;
                    attach.file_property_type = filePropertiesTypeList[i];
                    attach.rdf_attached_file = attachedFilesList[i];
                    attach.delete_file = attachedFilesList[i].Equals(null);
                    i++;
                    resourceAttachedFiles.Add(attach);
                }

                ModifyTripleList(resourceId, triplesList, publishHome, null, resourceAttachedFiles, true);
            }

            Log.Debug($"Modified the resource with attached file: {resourceId}");
        }

        /// <summary>
        /// Replaces the resource attached images
        /// </summary>
        /// <param name="resourceId">Resource identifier guid</param>
        /// <param name="oldImageName">Old image file name</param>
        /// <param name="newImageName">New image file name</param>
        /// <param name="imagePredicate">The predicate that will collect the image. Watch out, it does not support prefix, must be whole URIs.
        /// For example, with "gnoss:mainImage" setting, it would not work, it should be: "http://www.gnoss.com/mainImage"</param>
        /// <param name="fileRdfPropertiesList">Images names to load</param>
        /// <param name="filePropertiesTypeList">Attached files types</param>
        /// <param name="attachedFilesList">Images to attach</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        public void ReplaceResourceImage(Guid resourceId, string oldImageName, string newImageName, string imagePredicate, List<string> fileRdfPropertiesList, List<short> filePropertiesTypeList, List<byte[]> attachedFilesList, bool publishHome = false)
        {
            List<ModifyResourceTriple> triplesList = new List<ModifyResourceTriple>();
            List<SemanticAttachedResource> resourceAttachedFiles = new List<SemanticAttachedResource>();
            ModifyResourceTriple triple = new ModifyResourceTriple() { old_object = oldImageName, predicate = imagePredicate, new_object = newImageName, gnoss_property = GnossResourceProperty.none };
            triplesList.Add(triple);

            if (fileRdfPropertiesList.Count > 0)
            {
                int i = 0;
                foreach (string name in fileRdfPropertiesList)
                {
                    SemanticAttachedResource attach = new SemanticAttachedResource();
                    attach.file_rdf_property = name;
                    attach.file_property_type = filePropertiesTypeList[i];
                    attach.rdf_attached_file = attachedFilesList[i];
                    attach.delete_file = attachedFilesList[i].Equals(null);
                    i++;
                    resourceAttachedFiles.Add(attach);
                }

                ModifyTripleList(resourceId, triplesList, publishHome, null, resourceAttachedFiles, true);
            }

            Log.Debug($"Modified the resource with attached image: {resourceId}");
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete resources list <paramref name="resourceList"/>. It is necessary that the resource has assigned the properties <see cref="BaseResource.GnossId"/> or <see cref="BaseResource.ShortGnossId"/>
        /// </summary>
        /// <param name="resourceList">Resources list to delete</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void DeleteResourceList(ref List<ComplexOntologyResource> resourceList, int numAttemps = 5)
        {
            int processedNumber = 0;
            int attempNumber = 0;
            int numResourcesLeft = resourceList.Count;

            List<ComplexOntologyResource> originalResourceList = new List<ComplexOntologyResource>(resourceList);
            List<ComplexOntologyResource> resourcesToDelete = new List<ComplexOntologyResource>(resourceList);
            while (originalResourceList != null && originalResourceList.Where(rec => !rec.Deleted).Count() > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Starts lap number: {attempNumber}", this.GetType().Name);

                foreach (ComplexOntologyResource resource in originalResourceList)
                {
                    processedNumber = processedNumber + 1;
                    try
                    {
                        while (!resource.Deleted)
                        {
                            Delete(resource.ShortGnossId, processedNumber == originalResourceList.Count);
                            numResourcesLeft--;

                            Log.Debug($"Successfully deleted the resource with ID: {resource.GnossId}. {numResourcesLeft} resources left", this.GetType().Name);
                            resource.Deleted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR deleting: {processedNumber} of {originalResourceList.Count}\tID: {resource.GnossId}. Message: {ex.Message}", this.GetType().Name);
                    }
                }
                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);

                originalResourceList = new List<ComplexOntologyResource>(resourcesToDelete);
                resourceList = originalResourceList;
            }
        }

        /// <summary>
        /// Persistent delete of a resources list. if not specified, deletes the attachments of the resource.
        /// </summary>
        /// <param name="guidList">Resource identifiers list</param>
        /// <param name="deleteAttached">Indicates if the attached resources must be deleted</param>
        public void PersistentDeleteResourceIdList(List<Guid> guidList, bool deleteAttached = true)
        {
            int count = guidList.Count();
            foreach (Guid guid in guidList)
            {
                count--;
                PersistentDelete(guid, deleteAttached, count == 0);
            }
        }

        /// <summary>
        /// Persistent delete of a resources list
        /// </summary>
        /// <param name="resourceList">Resources list to delete</param>
        /// <param name="deleteAttached">Indicates if the attached resources must be deleted</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void PersistentDeleteResourceList(ref List<ComplexOntologyResource> resourceList, bool deleteAttached, int numAttemps = 5)
        {
            int processedNumber = 0;
            int attempNumber = 0;
            int numResourcesLeft = resourceList.Count;

            bool last = false;
            List<ComplexOntologyResource> originalResourceList = new List<ComplexOntologyResource>(resourceList);
            List<ComplexOntologyResource> resourcesToDelete = new List<ComplexOntologyResource>(resourceList);

            while (originalResourceList != null && originalResourceList.Count(rec => rec.Deleted == false) > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Starts lap number: {attempNumber}", this.GetType().Name);

                foreach (ComplexOntologyResource resource in resourcesToDelete.Where(rec => rec.Deleted == false))
                {
                    processedNumber = processedNumber + 1;
                    try
                    {
                        if (processedNumber == resourcesToDelete.Count(rec => rec.Deleted == false))
                        {
                            last = true;
                        }

                        resource.Deleted = PersistentDelete(resource.ShortGnossId, deleteAttached, last);
                        numResourcesLeft--;
                        Log.Debug($"Successfully deleted the resource with ID: {resource.GnossId}. {numResourcesLeft} resources left", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR deleting: {processedNumber} of {originalResourceList.Count}\tID: {resource.GnossId}. Message: {ex.Message}", this.GetType().Name);
                    }
                }
                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);

                originalResourceList = new List<ComplexOntologyResource>(resourcesToDelete);
            }
        }

        #endregion

        #endregion

        #region Basic semantic resource methods

        #region Load

        /// <summary>
        /// Loads a basic ontology resource
        /// </summary>
        /// <param name="resourceList">Resources list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void LoadBasicOntologyResourceListIntNote(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadBasicOntologyResourceListInt(ref resourceList, hierarquicalCategories, TiposDocumentacion.note, numAttemps);
            Log.Debug("Resources successfully loaded. End of load");
        }

        /// <summary>
        /// Loads a basic ontology resource list with link (Link to create the screenshot)
        /// </summary>
        /// <param name="resourceList">Resources list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        public void LoadBasicOntologyResourceListLink(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 2)
        {
            LoadBasicOntologyResourceListInt(ref resourceList, hierarquicalCategories, TiposDocumentacion.hyperlink, numAttemps);
            Log.Debug("Resources successfully loaded. End of load");
        }

        /// <summary>
        /// Loads a basic ontology resource list with link (Link to create the screenshot)
        /// </summary>
        /// <param name="resourceList">Resources list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void LoadBasicOntologyResourceListFile(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadBasicOntologyResourceListInt(ref resourceList, hierarquicalCategories, TiposDocumentacion.server_file, numAttemps);
            Log.Debug("Resources successfully loaded. End of load");
        }

        /// <summary>
        /// Loads a basic ontology resource list with a link to a Youtube video with resource type HyperLink
        /// </summary>
        /// <param name="resourceList">Resources list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void LoadBasicOntologyResourceListLinkVideo(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadBasicOntologyResourceListIntVideo(ref resourceList, hierarquicalCategories, TiposDocumentacion.hyperlink, numAttemps);
            Log.Debug("Resources successfully loaded. End of load");
        }

        /// <summary>
        /// Loads a basic ontology resource list with resource type Video
        /// </summary>
        /// <param name="resourceList">Resources list to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void LoadBasicOntologyResourceListVideo(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 5)
        {
            LoadBasicOntologyResourceListInt(ref resourceList, hierarquicalCategories, TiposDocumentacion.video, numAttemps);
            Log.Debug("Resources successfully loaded. End of load");
        }

        #endregion

        #endregion

        #region Secondary entities methods

        #region Modify

        /// <summary>
        /// Modifies secondary entities loaded in a normal way (one RDF by resource).
        /// </summary>
        /// <param name="resourceList">Resource list to delete</param>
        public void ModifySecondaryResourcesList(List<SecondaryResource> resourceList)
        {
            Log.Debug($"Modifying {resourceList.Count} resources");
            int left = resourceList.Count;
            foreach (SecondaryResource rs in resourceList)
            {
                if (ModifySecondaryResource(rs))
                {
                    left--;
                    Log.Debug($"Still remaining {left} resources");
                }
            }
        }

        /// <summary>
        /// Modifies secondary entities loaded in a normal way (one RDF by resource).
        /// </summary>
        /// /// <param name="resourceList">Resource list to delete</param>
        public bool ModifySecondaryResource(SecondaryResource resourceList)
        {
            bool modified = false;

            try
            {
                ModifySecondaryEntity(OntologyUrl, CommunityShortName, resourceList.RdfFile);
                resourceList.Modified = true;
                modified = true;
            }
            catch (Exception ex)
            {
                string mensaje = $"The secondary resource with ID: {resourceList.Id} cannot be modified . Menssage: {ex.Message}";
                Log.Error(mensaje, this.GetType().Name);
                resourceList.Modified = false;
                throw new GnossAPIException(mensaje);
            }
            return modified;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes secondary entities loaded in a normal way (one RDF by resource).
        /// </summary>
        /// <param name="urlList">Resouce urls list to delete</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void DeleteSecondaryEntitiesList(ref List<string> urlList, int numAttemps = 5)
        {
            int processedNumber = 0;
            int attempNumber = 0;

            List<string> originalResourceList = new List<string>(urlList);
            List<string> resourcesToDelete = new List<string>(urlList);
            while (urlList != null && urlList.Count > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                foreach (string Url in urlList)
                {
                    try
                    {
                        processedNumber = processedNumber + 1;
                        DeleteSecondaryEntity(OntologyUrl, CommunityShortName, Url);
                        Log.Debug($"Successfully deleted th resource with ID: {Url}", this.GetType().Name);
                        resourcesToDelete.Remove(Url);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR deleting: {processedNumber} of {urlList.Count}\tID: {Url}. Message: {ex.Message}", this.GetType().Name);
                    }
                }

                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
                urlList = new List<string>(resourcesToDelete);
            }
        }

        /// <summary>
        /// Deletes secondary entities loaded in a normal way (one RDF by resource).
        /// </summary>
        /// <param name="resourceList">Resource list to delete</param>
        /// <param name="numAttemps">Default 5. Number of retries loading of the failed load of a resource</param>
        public void DeleteSecondaryEntitiesList(ref List<SecondaryResource> resourceList, int numAttemps = 5)
        {
            int processedNumber = 0;
            int attempNumber = 0;

            List<SecondaryResource> originalResourceList = new List<SecondaryResource>(resourceList);
            List<SecondaryResource> resourcesToDelete = new List<SecondaryResource>(resourceList);
            List<SecondaryResource> resultList = new List<SecondaryResource>();

            while (resourceList != null && resourceList.Count > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                foreach (SecondaryResource resource in resourceList)
                {
                    resource.Deleted = false;
                    try
                    {
                        processedNumber = processedNumber + 1;
                        DeleteSecondaryEntity(resource.SecondaryOntology.OntologyUrl, CommunityShortName, resource.Id);
                        resource.Deleted = true;
                        if (!resultList.Contains(resource))
                        {
                            resultList.Add(resource);
                        }

                        Log.Debug($"Successfully deleted the resource with ID: {resource.Id}", this.GetType().Name);
                        resourcesToDelete.Remove(resource);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR deleting: {processedNumber} of {resourceList.Count}\tID: {resource.Id}. Menssage: {ex.Message}", this.GetType().Name);
                    }
                }

                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
                resourceList = new List<SecondaryResource>(resourcesToDelete);
            }

            List<SecondaryResource> result = originalResourceList.Except(resultList).ToList();
            resourceList = new List<SecondaryResource>(resultList);
            resourceList.AddRange(result);
        }

        #endregion

        #endregion

        #region Common methods for basic and complex ontology resources

        /// <summary>
        /// Method for modifying one or more properties of a loaded resource. In ModificarTriples can indicate whether title or description. By default false two fields. It influences the value of the resource searches
        /// </summary>
        /// <param name="resourceTriples">Contains as a key the resource guid identifier to modify and as a value a TriplesToModify list of the resource properties that will be modified</param> 
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="userId">Default null. User that try to modify the resource</param>
        /// <returns>Indicates whether the properties have been modified of the loaded resource</returns>
        public Dictionary<Guid, bool> ModifyPropertiesLoadedResources(Dictionary<Guid, List<TriplesToModify>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? userId = null)
        {
            return ModifyPropertiesLoadedResourcesInt(resourceTriples, numAttemps, publishHome, userId);
        }

        /// <summary>
        /// Method for adding one or more properties of a loaded resource. In IncluirTriples can indicate whether title or description. By default false two fields. It influences the value of the resource searches
        /// </summary>
        /// <param name="resourceTriples">Contains as a key the resource guid identifier to modify and as a value a TriplesToInclude list of the resource properties that will be included.</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="usuarioID">Default null. User that call the method</param>
        /// <returns>Indicates whether the properties have been added to the loaded resource</returns>
        public Dictionary<Guid, bool> InsertPropertiesLoadedResources(Dictionary<Guid, List<TriplesToInclude>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? usuarioID = null)
        {
            return InsertPropertiesLoadedResourcesInt(resourceTriples, numAttemps, publishHome, usuarioID);
        }

        /// <summary>
        /// Method for delete one or more properties of a loaded resource. In RemoveTriples can indicate whether title or description. By default false two fields. It influences the value of the resource searches.
        /// </summary>
        /// <param name="resourceTriples">Contains as a key the resource guid identifier to modify and as a value a RemoveTriples list of the resource properties that will be deleted.</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <returns>Dictionary resource identifier and boolean indicating the successfull, or not, result of the action</returns>
        public Dictionary<Guid, bool> DeletePropertiesLoadedResources(Dictionary<Guid, List<RemoveTriples>> resourceTriples, int numAttemps = 2, bool publishHome = false)
        {
            return DeletePropertiesLoadedResourcesInt(resourceTriples, numAttemps, publishHome);
        }

        /// <summary>
        /// Deletes, modifies and inserts triples in an already loaded resources.It is used in the case in which, to the same resource, want to do more than one of these actions at once.
        /// </summary>
        /// <param name="resourceTriplesModify">Contains as a key the resource guid identifier to modify and as a value a TriplesToModify list of the resource properties that will be modified</param>
        /// <param name="resourceTriplesDelete">Contains as a key the resource guid identifier to modify and as a value a RemoveTriples list of the resource properties that will be deleted</param>
        /// <param name="resourceTriplesInsert">Contains as a key the resource guid identifier to modify and as a value a TriplesToInclude list of the resource properties that will be inserted</param>        
        /// <param name="resourceTriplesAddAuxiliarEntity">Contains as a key the resource guid identifier to modify and as a value a AuxiliaryEntitiesTriplesToInclude list of the resource properties that will be inserted</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <returns>Dictionary resource identifier and boolean indicating the successfull, or not, result of the action</returns>
        public Dictionary<Guid, bool> ActionsOnPropertiesLoadedResources(Dictionary<Guid, List<TriplesToModify>> resourceTriplesModify, Dictionary<Guid, List<RemoveTriples>> resourceTriplesDelete, Dictionary<Guid, List<TriplesToInclude>> resourceTriplesInsert, Dictionary<Guid, List<AuxiliaryEntitiesTriplesToInclude>> resourceTriplesAddAuxiliarEntity, bool publishHome = false)
        {
            return ActionsOnPropertiesLoadedResourcesInt(resourceTriplesModify, resourceTriplesDelete, resourceTriplesInsert, resourceTriplesAddAuxiliarEntity, publishHome);
        }

        /// <summary>
        /// Modify one or more properties of an entity of a loaded resource.
        /// </summary>
        /// <param name="resourceTriples">Contains as a key the large Gnoss identifier of secondary resource to modify and as a value a ModificarTriples list of the resource properties to modified</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        public void ModifyPropertyLoadedSecondaryResource(Dictionary<string, List<TriplesToModify>> resourceTriples, int numAttemps = 2, bool publishHome = false)
        {
            ModifyPropertyLoadedSecondaryResourceInt(resourceTriples, numAttemps, publishHome);
        }

        /// <summary>
        /// Adds one or more properties to an auxiliary entity to a loaded resource. It influences the value of the resource searches. If not exists, an auxiliary entity is created
        /// </summary>
        /// <param name="resourceTriples">Contains as a key the resource guid identifier to modify and as a value a AuxiliaryEntitiesTriplesToInclude list of the resource properties that will be inserted.
        /// For example: in http://www.gnoss.com/items/Application_223b30c1-2552-4ed0-ba5f-e257585b08bf_9c126c3a-7850-4cdc-b176-95ae6fd0bb78 the entity identifier is: 9c126c3a-7850-4cdc-b176-95ae6fd0bb78
        /// If an entity has more than one property, that Guid indicates that all properties belong to the same entity. Without that Guid, an entity is created for each property.</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="communityShortName">Community short name where the AuxiliaryEntitiesTriplesToInclude will be loaded</param>
        /// <returns>Indicates whether the properties have been inserted in the auxiliar entity</returns>
        public bool InsertAuxiliarEntityOnPropertiesLoadedResource(Dictionary<Guid, List<AuxiliaryEntitiesTriplesToInclude>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? userId = null)
        {
            return InsertAuxiliarEntityOnPropertiesLoadedResourceInt(resourceTriples, numAttemps, publishHome, userId);
        }

        #endregion

        #region Statics methods

        /// <summary>
        /// Gets the ontology name (without extension) from the url of the ontology
        /// </summary>
        /// <param name="urlOntology">URL of the ontology</param>
        /// <returns>String with the ontology name</returns>
        public static string GetOntologyNameWithOutExtensionFromUrlOntology(string urlOntology)
        {
            return urlOntology.Substring(urlOntology.LastIndexOf(StringDelimiters.Slash) + StringDelimiters.Slash.Length).Replace($".owl", "");
        }

        /// <summary>
        /// Gets the ontology name (with extension) from the url of the ontology
        /// </summary>
        /// <param name="urlOntology">URL of the ontology</param>
        /// <returns>String with the ontology name</returns>
        public static string GetOntologyNameWithExtensionFromUrlOntology(string urlOntology)
        {
            return urlOntology.Substring(urlOntology.LastIndexOf(StringDelimiters.Slash) + StringDelimiters.Slash.Length);
        }

        #endregion

        #region Virtuoso Query

        /// <summary>
        /// Allows a virtuoso query, setting the 'SELECT' and 'WHERE' parts of the query and the graph name
        /// </summary>
        /// <param name="selectPart">The 'SELECT' query part</param>
        /// <param name="wherePart">The 'WHERE' query part</param>
        /// <param name="ontologiaName">Graph name where the query runs (without extension '.owl')</param>
        /// <param name="userMasterServer">Use Master Virtuoso</param>
        /// <returns>DataSet with the query result</returns>
        public SparqlObject VirtuosoQuery(string selectPart, string wherePart, string ontologiaName, bool userMasterServer = true)
        {
            Log.Trace("Entering the method", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return VirtuosoQueryInt(selectPart, wherePart, ontologiaName, userMasterServer);
        }

        /// <summary>
        /// Allows a virtuoso query, setting the 'SELECT' and 'WHERE' parts of the query and the community identifier
        /// </summary>
        /// <param name="selectPart">The 'SELECT' query part</param>
        /// <param name="wherePart">The 'WHERE' query part</param>
        /// <param name="communityId">Community identifier</param>
        /// <returns>DataSet with the query result</returns>
        public SparqlObject VirtuosoQuery(string selectPart, string wherePart, Guid communityId, bool userMasterServer = true)
        {
            Log.Trace("Entering the method", this.GetType().Name);
            return VirtuosoQueryInt(selectPart, wherePart, communityId.ToString(), userMasterServer);
        }

        /// <summary>
        /// Allows a virtuoso query, setting the 'SELECT' and 'WHERE' parts of the query and the graph name
        /// </summary>
        /// <param name="selectPart">The 'SELECT' query part</param>
        /// <param name="wherePart">The 'WHERE' query part</param>
        /// <param name="ontologiaName">Graph name where the query runs (without extension '.owl')</param>
        /// <param name="userMasterServer">Use Master Virtuoso</param>
        /// <returns>DataSet with the query result</returns>
        public DataSet VirtuosoQueryDataSet(string selectPart, string wherePart, string ontologiaName, bool userMasterServer = true)
        {
            Log.Trace("Entering the method", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return VirtuosoQueryIntDataSet(selectPart, wherePart, ontologiaName, userMasterServer);
        }

        /// <summary>
        /// Allows a virtuoso query, setting the 'SELECT' and 'WHERE' parts of the query and the community identifier
        /// </summary>
        /// <param name="selectPart">The 'SELECT' query part</param>
        /// <param name="wherePart">The 'WHERE' query part</param>
        /// <param name="communityId">Community identifier</param>
        /// <returns>DataSet with the query result</returns>
        public DataSet VirtuosoQueryDataSet(string selectPart, string wherePart, Guid communityId, bool userMasterServer = true)
        {
            Log.Trace("Entering the method", this.GetType().Name);
            return VirtuosoQueryIntDataSet(selectPart, wherePart, communityId.ToString(), userMasterServer);
        }

        /// <summary>
        /// Allows a virtuoso query, setting the 'SELECT' and 'WHERE' parts of the query and the community identifier
        /// </summary>
        /// <param name="selectPart">The 'SELECT' query part</param>
        /// <param name="wherePart">The 'WHERE' query part</param>
        /// <param name="graphs">List of the graphs in which you want search</param>
        /// <returns>DataSet with the query result</returns>
        public SparqlObject VirtuosoQueryMultipleGraph(string selectPart, string wherePart, List<string> graphs)
        {
            Log.Trace("Entering the method", this.GetType().Name);
            return VirtuosoQueryMultipleGraphInt(selectPart, wherePart, graphs);
        }

        #endregion

        /// <summary>
        /// Sorts the multilanguage list of title or description with the main language in the first element
        /// </summary>
        /// <param name="listToOrder">Lista posiblemente desordenada</param>
        /// <returns>The list with the main language in the first element</returns>
        public List<Multilanguage> ShortMultimediaTitleDescriptionString(List<Multilanguage> listToOrder)
        {
            string mainLanguage = CommunityApiWrapper.GetCommunityMainLanguage();
            if (mainLanguage == string.Empty)
            {
                mainLanguage = Languages.Spanish;
            }
            else if (mainLanguage == null)
            {
                mainLanguage = Languages.Spanish;
            }
            Multilanguage firstelement = new Multilanguage();
            foreach (Multilanguage titleDescription in listToOrder)
            {
                if (titleDescription.Language == mainLanguage)
                {
                    firstelement = titleDescription;
                }
            }
            listToOrder.Remove(firstelement);
            List<Multilanguage> orderedList = new List<Multilanguage>();
            orderedList.Add(firstelement);
            foreach (Multilanguage tituloDescripcion in listToOrder)
            {
                orderedList.Add(tituloDescripcion);
            }
            return orderedList;
        }

        /// <summary>
        /// Returns a guid from a resource large identifier. If it cannot get it, returns an empty guid.
        /// </summary>
        /// <param name="largeResourceId">Resource large identifier</param>
        /// <returns></returns>
        public Guid GetShortGuid(string largeResourceId)
        {
            try
            {
                if (largeResourceId.Contains("items"))
                {
                    string[] result = largeResourceId.Split('_');
                    return (new Guid(result[result.Count() - 2]));
                }
                else
                {
                    string[] result = largeResourceId.Split('/');
                    return (new Guid(result[result.Count() - 1]));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to get the guid of {largeResourceId}. {ex.Message}");
                return new Guid();
            }
        }

        /// <summary>
        /// Gets an OAuth signed url
        /// </summary>
        /// <param name="url">url to sign</param>
        /// <returns>Signed url string</returns>
        public string GetSignForUrl(string url)
        {
            string sign = OAuthInstance.GetSignedUrl(url);
            return sign.Replace("&", ",").Replace($"{url}?", "");
        }

        /// <summary>
        /// Gets the resource rdf and downloads it in the indicated directory path
        /// </summary>
        /// <param name="domain">Domain where the community belongs to</param>
        /// <param name="resourceId">Resource short identifier</param>
        /// <param name="directoryPath">Directory path where the resource will be downloaded</param>
        /// <returns>Resource rdf</returns>
        public string GetBasicOntologyResouceRdf(string domain, string resourceId, string directoryPath)
        {
            string rdf = string.Empty;
            string resourceUrl = $"{domain}/comunidad/{CommunityShortName}/recurso/nombre/{resourceId}";
            string urlRdf = $"{resourceUrl}?rdf";
            string filePath = $"{directoryPath}basicOntologyResouceRdf.rdf";

            GnossWebClient webClient = new GnossWebClient(false);
            try
            {
                webClient.Headers.Add($"Referer:{urlRdf}");
                webClient.Headers.Add("User-Agent:Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.131 Safari/537.36 gnossspider");
                webClient.Headers.Add($"Authorization: OAuth \n {GetSignForUrl(resourceUrl)}");
                webClient.DownloadFile(urlRdf, filePath);
                StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true);
                rdf = sr.ReadToEnd();
                sr.Close();
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Log.Debug($"Error downloading file: {urlRdf}. Error: {ex.Message}");
            }
            return rdf;
        }

        /// <summary>
        /// Downloads a file from the url
        /// </summary>
        /// <param name="URL">Resource url to download</param>
        /// <param name="fileName">File name where the resource will be downloaded</param>
        public void DownloadFilesFromURL(string URL, string fileName)
        {
            GnossWebClient webClient = new GnossWebClient(false);
            try
            {
                webClient.Headers.Add($"Referer:{URL}");
                webClient.Headers.Add("User-Agent:Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.131 Safari/537.36 gnossspider");
                string sign = GetSignForUrl(URL);
                webClient.Headers.Add($"Authorization: OAuth \n {sign}");
                webClient.DownloadFile(URL, fileName);
            }
            catch (WebException wex)
            {
                Console.WriteLine($"File {fileName} not downloaded. {wex.Message}");
            }
        }

        /// <summary>
        /// Given a title and/or description, returns the extracted labels from EtiquetadoAutomatico
        /// </summary>
        /// <param name="title">(Optional) Resource title</param>
        /// <param name="description">(Optional) Resource description</param>
        /// <returns>List of strings with each of the tags returned</returns>
        /// <remarks>Both the title and the description can be passed empty but they can never be  both empty at the same time.</remarks>
        public List<string> GetAutomaticLabelingTags(string title = "", string description = "")
        {
            List<string> tagList = null;

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(description))
            {
                throw new GnossAPIArgumentException("Both parameters at GetAutomaticLabelingTags cannot be empty at the same time. At least one of them must have value.");
            }
            else
            {

                string url = $"{ApiUrl}/resource/get-automatic-labeling";
                TagsFromServiceModel model = new TagsFromServiceModel{ title = title, description = description, community_short_name = CommunityShortName };

                string response = WebRequestPostWithJsonObject(url, model);

                tagList = response.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToList();
            }            

            return tagList;
        }

        /// <summary>
        /// Get the community members email list
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetCommunityMembersEmailList()
        {
            List<string> emails = new List<string>();
            Dictionary<Guid, string> emailPerPerson = CommunityApiWrapper.GetCommunityPersonIDEmail();

            foreach (Guid personaID in emailPerPerson.Keys)
            {
                emails.Add(emailPerPerson[personaID]);
            }

            return emails;
        }

        /// <summary>
        /// Sets the resource main image. The resource and the image must be loaded.  
        /// </summary>
        /// <param name="resourceId">resource identifier</param>
        /// <param name="imageName">image name string</param>
        /// <param name="sizes">Availables sizes of the image, the main size must be the first of the list. [IMGPrincipal][318,234,992,]cce87492-2a13-4fc5-80a9-b3d59b63a2f1.jpg.</param>
        public void SetMainImageLoadedImage(Guid resourceId, string imageName, List<string> sizes)
        {
            string sizeMask = "[";
            foreach (string size in sizes)
            {
                sizeMask = $"{sizeMask}{size},";
            }
            sizeMask = $"{sizeMask}]";
            string image = string.Empty;
            if (!string.IsNullOrEmpty(imageName))
            {
                image = $"[IMGPrincipal]{sizeMask}{imageName}";
            }
            Console.WriteLine($"Image string, the first number will be the main one: {image}");
            SetMainImage(resourceId, image);

            if (image != string.Empty)
            {
                Log.Debug($"Correct main image setting of resource {resourceId}, the of the new main image is {sizes[0]} and its name is {imageName}");
            }
            else
            {
                Log.Debug($"The main image of the resource {resourceId} has been deleted.");
            }
        }

        #endregion

        #region Private methods

        #region Categories

        private List<Guid> GetHierarquicalCategoriesIdentifiersList(List<string> hierarquicalCategoriesList)
        {
            List<Guid> resultList = null;

            List<ThesaurusCategory> categories = null;

            categories = CommunityApiWrapper.LoadCategories(CommunityShortName);

            if (hierarquicalCategoriesList != null && hierarquicalCategoriesList.Count > 0)
            {
                foreach (string cat in hierarquicalCategoriesList)
                {
                    ThesaurusCategory category = FindHierarquicalCategoryNameInCategories(cat, categories);
                    if (category != null)
                    {
                        if (resultList == null)
                        {
                            resultList = new List<Guid>();
                        }
                        resultList.Add(category.category_id);
                    }
                }
            }

            if (hierarquicalCategoriesList == null || resultList == null || hierarquicalCategoriesList.Count != resultList.Count)
            {
                throw new GnossAPICategoryException("Error obtaining the identifiers of one of the categories. It is possible that some of the introduced categories do not belong to the thesaurus");
            }
            return resultList;
        }

        private ThesaurusCategory FindHierarquicalCategoryNameInCategories(string hierarchicalName, List<ThesaurusCategory> categories)
        {
            ThesaurusCategory resultCategory = null;

            string[] path = hierarchicalName.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string category in path)
            {
                if (resultCategory == null)
                {
                    resultCategory = categories.Find(cat => cat.category_name.Equals(category));
                }
                else
                {
                    resultCategory = resultCategory.Children.Find(cat => cat.category_name.Equals(category));
                }

                if (resultCategory == null)
                {
                    // The path doesn't match with a category path
                    return null;
                }
            }

            return resultCategory;
        }

        /// <summary>
        /// Method to modify the resource's subtype
        /// </summary>
        /// <param name="resourceId">Resource identifier guid</param>
        /// <param name="ontologyName">The ontology name of the resource to modify</param>
        /// <param name="subtype">The subtype of the resource to modify</param>
        /// <param name="userId">User that try to modify the resource</param>
        public void ModifySubtype(Guid resourceId, string ontologyName, string subtype, string previousType, Guid? userId = null)
        {
            ModifyResourceSubtype model = null;
            try
            {
                string url = $"{ApiUrl}/resource/modify-subtype";
                model = new ModifyResourceSubtype() { community_short_name = CommunityShortName, ontology_name = ontologyName, resource_id = resourceId, subtype = subtype, previous_type = previousType, user_id = userId };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug("Ended resource subtype modification");
            }
            catch (Exception ex)
            {
                Log.Error($"Error modifying resource subtype. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }
        }

        private List<Guid> GetNotHierarquicalCategoriesIdentifiersList(List<string> notHierarquicalCategoriesList)
        {
            List<ThesaurusCategory> categoryList = null;

            categoryList = CommunityApiWrapper.LoadCategories(CommunityShortName);

            List<Guid> resultList = null;
            if (notHierarquicalCategoriesList != null && notHierarquicalCategoriesList.Count > 0)
            {
                string[] categories = null;
                string[] separator = new string[] { "|||" };

                foreach (ThesaurusCategory category in categoryList)
                {
                    if (!string.IsNullOrWhiteSpace(category.category_name) && category.category_name.Contains("|||"))
                    {

                        categories = category.category_name.Split(separator, StringSplitOptions.None);
                        for (int i = 0; i < categories.Length; i++)
                        {
                            string value = categories[i].Substring(0, categories[i].IndexOf("@"));
                            foreach (string cat in notHierarquicalCategoriesList)
                            {

                                if (categories[i].Substring(0, categories[i].IndexOf("@")).Equals(cat))
                                {
                                    if (resultList == null)
                                    {
                                        resultList = new List<Guid>();
                                    }
                                    if (!resultList.Contains(category.category_id))
                                    {
                                        resultList.Add(category.category_id);
                                    }

                                }
                                else
                                {
                                    ThesaurusCategory thesaurusCategory = categoryList.Find(comCat => comCat.category_name.Equals(cat));
                                    if (thesaurusCategory != null)
                                    {
                                        if (resultList == null)
                                        {
                                            resultList = new List<Guid>();
                                        }
                                        if (!resultList.Contains(thesaurusCategory.category_id))
                                        {
                                            resultList.Add(thesaurusCategory.category_id);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (string cat in notHierarquicalCategoriesList)
                        {
                            ThesaurusCategory thesaurusCategory = categoryList.Find(comCat => comCat.category_name.Equals(cat));
                            if (thesaurusCategory != null)
                            {
                                if (resultList == null)
                                {
                                    resultList = new List<Guid>();
                                }
                                if (!resultList.Contains(thesaurusCategory.category_id))
                                {
                                    resultList.Add(thesaurusCategory.category_id);
                                }
                            }
                        }
                    }

                }
            }

            if (notHierarquicalCategoriesList == null || resultList == null || notHierarquicalCategoriesList.Count != resultList.Count)
            {
                throw new GnossAPICategoryException("Error trying to get the categories identificators. It's possible that some categories not exists in the community");
            }

            return resultList;
        }

        #endregion

        #region Load

        #region Complex semantic ontology resources

        /// <summary>
        /// Load a complex semantic resources list
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="numAttemps">Default 2. Number of retries loading of the failed load of a resource</param>
        /// <param name="communityShortName">Default null. Defined if it is necessary the load in other community that the specified in the OAuth</param>
        /// <param name="rdfsPath">Default null. Path to save the RDF, if necessary</param>
        private void LoadComplexSemanticResourceListInt(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, int numAttemps = 2, string rdfsPath = null)
        {
            int processedNumber = 0;
            bool last = false;
            List<ComplexOntologyResource> originalResourceList = new List<ComplexOntologyResource>(resourceList);
            int attempNumber = 0;
            int resourcesToUpload = originalResourceList.Where(r => !r.Uploaded).Count();

            while (originalResourceList != null && originalResourceList.Count > 0 && resourcesToUpload > 0 && attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                int totalResources = originalResourceList.Count;
                int resourcesLeft = originalResourceList.Count;

                foreach (ComplexOntologyResource rec in originalResourceList.Where(r => !r.Uploaded))
                {
                    processedNumber = processedNumber + 1;
                    try
                    {
                        if (processedNumber == originalResourceList.Count)
                        {
                            last = true;
                        }
                        if (string.IsNullOrEmpty(rec.Ontology.OntologyUrl))
                        {
                            rec.Ontology.OntologyUrl = OntologyUrl;
                        }
                        LoadComplexSemanticResourceInt(rec, hierarquicalCategories, last, numAttemps, rdfsPath);
                        resourcesLeft = resourcesLeft - 1;
                    }
                    catch (GnossAPICategoryException gacex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {originalResourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {gacex.Message}", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {originalResourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }

                processedNumber = 0;
                resourcesToUpload = originalResourceList.Where(r => !r.Uploaded).Count();
            }
        }

        /// <summary>
        /// Load resources of main entities in an otology and a community
        /// </summary>
        /// <param name="resourceList">List of resources to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="ontology">Ontology where resource will be loaded</param>
        /// <param name="communityShortName">Community short name where the resources will be loaded</param>
        /// <param name="numAttemps">Default 1. Number of retries loading of the failed load of a resource</param>
        private void LoadComplexSemanticResourceListWithOntologyAndCommunityInt(List<ComplexOntologyResource> resourceList, bool hierarquicalCategories, string ontology, int numAttemps = 1)
        {
            int processedNumber = 0;
            bool last = false;
            List<ComplexOntologyResource> originalResourceList = new List<ComplexOntologyResource>(resourceList);
            int attempNumber = 0;
            int resourcesToUpload = originalResourceList.Where(r => !r.Uploaded).Count();

            while (originalResourceList != null && originalResourceList.Count > 0 && resourcesToUpload > 0 && attempNumber < numAttemps)
            {
                attempNumber = attempNumber + 1;
                int totalResources = originalResourceList.Count;
                int resourcesLeft = originalResourceList.Count;

                foreach (ComplexOntologyResource rec in originalResourceList.Where(r => !r.Uploaded))
                {
                    processedNumber = processedNumber + 1;
                    try
                    {
                        if (processedNumber == originalResourceList.Count)
                        {
                            last = true;
                        }
                        if (string.IsNullOrEmpty(rec.Ontology.OntologyUrl))
                        {
                            rec.Ontology.OntologyUrl = OntologyUrl;
                        }
                        LoadComplexSemanticResourceWithOntologyAndCommunityInt(rec, hierarquicalCategories, last, ontology);
                        resourcesLeft = resourcesLeft - 1;
                    }
                    catch (GnossAPICategoryException gacex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {originalResourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {gacex.Message}", this.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR at: {processedNumber} of {originalResourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }

                processedNumber = 0;
                resourcesToUpload = originalResourceList.Where(r => !r.Uploaded).Count();
            }
        }

        /// <summary>
        /// Load a complex semantic resources in an otology and a community
        /// </summary>
        /// <param name="resource">Resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="isLast">There are not resources left to load</param>
        /// <param name="ontology">Ontology where resource will be loaded</param>
        /// <param name="communityShortName">Community short name where the resources will be loaded</param>
        /// <returns>Resource identifier string</returns>
        private string LoadComplexSemanticResourceWithOntologyAndCommunityInt(ComplexOntologyResource resource, bool hierarquicalCategories, bool isLast, string ontology)
        {
            try
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                string documentId = string.Empty;
                ontology = ontology.ToLower().Replace($".owl", "");
                ontology = OntologyUrl.Replace(OntologyUrl.Substring(OntologyUrl.LastIndexOf("/") + 1), $"{ontology}.owl");

                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                documentId = CreateComplexOntologyResource(model);
                resource.Uploaded = true;

                Log.Debug($"Loaded: \tID: {resource.Id}\tTitle: {resource.Title}\tResourceID: {resource.Ontology.ResourceId}");
                resource.GnossId = documentId;
            }
            catch (GnossAPICategoryException gacex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {gacex.Message}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }
            return resource.GnossId;
        }

        private string LoadComplexSemanticResourceInt(ComplexOntologyResource resource, bool hierarquicalCategories = false, bool isLast = false, int numAttemps = 2, string rdfsPath = null)
        {
            try
            {
                if (resource.CategoriesIds == null)
                {
                    if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                    {
                        if (hierarquicalCategories)
                        {
                            resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                        }
                        else
                        {
                            resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                        }
                    }
                }
                string documentId = string.Empty;

                LoadResourceParams model = GetResourceModelOfComplexOntologyResource(resource, false, isLast);
                documentId = CreateComplexOntologyResource(model);
                resource.Uploaded = true;

                Log.Debug($"Loaded: \tID: {resource.ShortGnossId}\tTitle: {resource.Title}\tResourceID: {resource.Ontology.ResourceId}");

                if (!string.IsNullOrEmpty(rdfsPath) && !string.IsNullOrWhiteSpace(rdfsPath))
                {
                    if (!Directory.Exists($"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}"))
                    {
                        Directory.CreateDirectory($"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}");
                    }

                    string rdfFile = $"{rdfsPath}/{GetOntologyNameWithOutExtensionFromUrlOntology(resource.Ontology.OntologyUrl)}/{resource.ShortGnossId}.rdf";
                    if (!File.Exists(rdfFile))
                    {
                        File.WriteAllText(rdfFile, resource.StringRdfFile);
                    }
                }

                resource.GnossId = documentId;
            }
            catch (GnossAPICategoryException gacex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {gacex.Message}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading the resource: \tID: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }
            return resource.GnossId;
        }

        #endregion

        #region Basic semantic ontology resources

        /// <summary>
        /// Loads a basic ontology resource
        /// </summary>
        /// <param name="resource">Basic intology resource to load</param>
        /// <param name="hierarquicalCategories">Indicates whether the categories has hierarchy</param>
        /// <param name="resourceType">Indicates the type of resource to load</param>
        /// <param name="isLast">Indicates There are not resources left to load</param>
        private void LoadBasicOntologyResourceInt(BasicOntologyResource resource, bool hierarquicalCategories, TiposDocumentacion resourceType, bool isLast = false)
        {
            Log.Trace("******************** Begin Load ", this.GetType().Name);

            if (((resource.CategoriesIds != null && resource.CategoriesIds.Count == 0) || resource.CategoriesIds == null) && resource.TextCategories != null && resource.TextCategories.Count > 0)
            {
                if (hierarquicalCategories)
                {
                    resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                }
                else
                {
                    resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                }
            }

            try
            {
                LoadResourceParams model = GetResourceModelOfBasicOntologyResource(resource, isLast, (short)resourceType);
                string documentId = CreateBasicOntologyResource(model);
                resource.Uploaded = true;
                resource.ShortGnossId = new Guid(documentId.Trim('"'));
                Log.Trace($"Loaded: {resource.GnossId}\tTitle: {resource.Title}\tResourceID: {documentId}", this.GetType().Name);
                if (documentId != resource.GnossId)
                {
                    throw new GnossAPIException($"Resource loaded with the id: {documentId}\nThe IDGnoss provided to the method is different from the returned by the API");
                }
            }
            catch (GnossAPIException ex)
            {
                Log.Error($"Resource: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR at: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }

            Log.Trace("******************** End Load", this.GetType().Name);
        }

        private void LoadBasicOntologyResourceIntVideo(BasicOntologyResource resource, bool hierarquicalCategories, TiposDocumentacion resourceType, bool isLast = false)
        {
            Log.Trace("******************** Begin Load ", this.GetType().Name);

            if (hierarquicalCategories)
            {
                resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);
            }
            else
            {
                resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
            }

            try
            {
                LoadResourceParams model = GetResourceModelOfBasicOntologyResource(resource, isLast, (short)resourceType);
                string documentId = CreateBasicOntologyResource(model);
                resource.Uploaded = true;
                try
                {
                    documentId = JsonConvert.DeserializeObject<string>(documentId);
                }
                catch (JsonReaderException)
                {
                }
                resource.ShortGnossId = new Guid(documentId);
                Log.Debug($"Loaded: {resource.ShortGnossId}\tTitle: {resource.Title}\tResourceID: {documentId}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR at: {resource.ShortGnossId} . Title: {resource.Title}. Message: {ex.Message}", this.GetType().Name);
            }

            Log.Debug("******************** End Load", this.GetType().Name);
        }

        private void LoadBasicOntologyResourceListInt(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, TiposDocumentacion resourceType, int numAttemps = 2)
        {
            int processedNumber = 0;
            List<BasicOntologyResource> originalResourceList = new List<BasicOntologyResource>(resourceList);
            List<BasicOntologyResource> resourcesToLoad = new List<BasicOntologyResource>(resourceList);
            int attempNumber = 0;

            while (resourcesToLoad != null && resourcesToLoad.Count > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                foreach (BasicOntologyResource rec in resourceList)
                {
                    processedNumber = processedNumber + 1;

                    try
                    {
                        LoadBasicOntologyResourceInt(rec, hierarquicalCategories, resourceType, processedNumber == resourceList.Count());
                        if (rec.Uploaded)
                        {
                            Log.Debug($"Loaded: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId}\tTitle: {rec.Title}");
                        }
                        resourcesToLoad.Remove(rec);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR in: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Message: {ex.Message}", this.GetType().Name);
                    }
                }

                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
            }
        }

        private void LoadBasicOntologyResourceListIntVideo(ref List<BasicOntologyResource> resourceList, bool hierarquicalCategories, TiposDocumentacion resourceType, int numAttemps = 5)
        {
            int processedNumber = 0;
            List<BasicOntologyResource> originalResourceList = new List<BasicOntologyResource>(resourceList);
            List<BasicOntologyResource> resourcesToLoad = new List<BasicOntologyResource>(resourceList);
            int attempNumber = 0;
            while (resourcesToLoad != null && resourcesToLoad.Count > 0 && attempNumber <= numAttemps)
            {
                attempNumber = attempNumber + 1;
                Log.Trace($"******************** Begin lap number: {attempNumber}", this.GetType().Name);
                foreach (BasicOntologyResource rec in resourceList)
                {
                    processedNumber = processedNumber + 1;
                    try
                    {
                        LoadBasicOntologyResourceIntVideo(rec, hierarquicalCategories, resourceType, processedNumber == resourceList.Count());
                        Log.Debug($"Loaded: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId}\tTitle: {rec.Title}");
                        resourcesToLoad.Remove(rec);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR in: {processedNumber} of {resourceList.Count}\tID: {rec.ShortGnossId} . Title: {rec.Title}. Mensaje: {ex.Message}", this.GetType().Name);
                    }
                }
                Log.Debug($"******************** Finished lap number: {attempNumber}", this.GetType().Name);
            }
        }

        #endregion

        #endregion

        #region Secondary entities methods

        private void CreateSecondaryEntity(string ontology_url, string community_short_name, byte[] rdf)
        {
            string url = $"{ApiUrl}/secondary-entity/create";

            SecondaryEntityModel model = new SecondaryEntityModel() { ontology_url = ontology_url, community_short_name = community_short_name, rdf = rdf };

            WebRequestPostWithJsonObject(url, model);

            Log.Debug($"The secondary entity has been created in the graph {ontology_url} of the communtiy {community_short_name}");
        }

        private void ModifySecondaryEntity(string ontology_url, string community_short_name, byte[] rdf)
        {
            string url = $"{ApiUrl}/secondary-entity/modify";

            SecondaryEntityModel model = new SecondaryEntityModel() { ontology_url = ontology_url, community_short_name = community_short_name, rdf = rdf };

            WebRequestPostWithJsonObject(url, model);

            Log.Debug($"The secondary entity has been modified in the graph {ontology_url} of the communtiy {community_short_name}");
        }

        private void DeleteSecondaryEntity(string ontology_url, string community_short_name, string entity_id)
        {
            string url = $"{ApiUrl}/secondary-entity/delete";

            SecondaryEntityModel model = new SecondaryEntityModel() { ontology_url = ontology_url, community_short_name = community_short_name, entity_id = entity_id };

            WebRequestPostWithJsonObject(url, model);

            Log.Debug($"The secondary entity {entity_id} has been deleted in the graph {ontology_url} of the communtiy {community_short_name}");
        }

        #endregion

        #region Common methods for basic and complex ontology resources

        private Dictionary<Guid, bool> ModifyPropertiesLoadedResourcesInt(Dictionary<Guid, List<TriplesToModify>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? userId = null)
        {
            Dictionary<Guid, bool> result = new Dictionary<Guid, bool>();
            int processedNumber = 0;
            int attempNumber = 0;

            List<string> valores = new List<string>();
            Dictionary<Guid, List<TriplesToModify>> toModify = new Dictionary<System.Guid, List<TriplesToModify>>(resourceTriples);
            while (toModify != null && toModify.Count > 0 && attempNumber < numAttemps)
            {
                int i = 0;
                int contResources = resourceTriples.Keys.Count;
                foreach (Guid docID in resourceTriples.Keys)
                {
                    i++;
                    List<ModifyResourceTriple> listaValores = new List<ModifyResourceTriple>();
                    attempNumber = attempNumber + 1;
                    processedNumber = processedNumber + 1;
                    foreach (TriplesToModify mT in resourceTriples[docID])
                    {
                        ModifyResourceTriple triple = new ModifyResourceTriple();
                        triple.old_object = mT.OldValue;
                        triple.predicate = mT.Predicate;
                        triple.new_object = mT.NewValue;
                        triple.gnoss_property = GnossResourceProperty.none;

                        if (mT.Title)
                        {
                            triple.gnoss_property = GnossResourceProperty.title;
                        }
                        else if (mT.Description)
                        {
                            triple.gnoss_property = GnossResourceProperty.description;
                        }

                        listaValores.Add(triple);
                    }

                    try
                    {
                        bool endOfLoad = false;
                        if (i == contResources)
                        {
                            endOfLoad = true;
                        }
                        ModifyTripleList(docID, listaValores, publishHome, null, null, endOfLoad, userId);

                        Log.Debug($"{processedNumber} of {resourceTriples.Count}. Object: {docID}. Resource: {resourceTriples[docID].ToArray()}");
                        toModify.Remove(docID);
                        if (result.ContainsKey(docID))
                        {
                            result[docID] = true;
                        }
                        else
                        {
                            result.Add(docID, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Resource {docID} : {ex.Message}");
                        if (result.ContainsKey(docID))
                        {
                            result[docID] = false;
                        }
                        else
                        {
                            result.Add(docID, false);
                        }
                    }
                }
                Log.Debug($"******************** Lap number: {attempNumber} finished");
            }

            return result;
        }

        private Dictionary<Guid, bool> InsertPropertiesLoadedResourcesInt(Dictionary<Guid, List<TriplesToInclude>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? usuarioID = null)
        {
            Dictionary<Guid, bool> result = new Dictionary<Guid, bool>();
            int processedNumber = 0;
            int attempNumber = 0;
            Dictionary<Guid, List<TriplesToInclude>> toInsert = new Dictionary<System.Guid, List<TriplesToInclude>>(resourceTriples);
            while (toInsert != null && toInsert.Count > 0 && attempNumber < numAttemps)
            {
                int i = 0;
                int contResources = resourceTriples.Keys.Count;
                foreach (Guid docID in resourceTriples.Keys)
                {
                    i++;
                    List<ModifyResourceTriple> listaValores = new List<ModifyResourceTriple>();
                    attempNumber = attempNumber + 1;
                    processedNumber = processedNumber + 1;
                    foreach (TriplesToInclude iT in resourceTriples[docID])
                    {
                        ModifyResourceTriple triple = new ModifyResourceTriple();
                        triple.old_object = null;
                        triple.predicate = iT.Predicate;
                        triple.new_object = iT.NewValue;
                        triple.gnoss_property = GnossResourceProperty.none;

                        if (iT.Title)
                        {
                            triple.gnoss_property = GnossResourceProperty.title;
                        }
                        else if (iT.Description)
                        {
                            triple.gnoss_property = GnossResourceProperty.description;
                        }

                        listaValores.Add(triple);
                    }
                    try
                    {
                        bool endOfLoad = false;
                        if (i == contResources)
                        {
                            endOfLoad = true;
                        }
                        ModifyTripleList(docID, listaValores, publishHome, null, null, endOfLoad, usuarioID);

                        Log.Debug($"{processedNumber} of {resourceTriples.Count} Object: {docID}. Resource: {resourceTriples[docID].ToArray()}");
                        toInsert.Remove(docID);
                        if (result.ContainsKey(docID))
                        {
                            result[docID] = true;
                        }
                        else
                        {
                            result.Add(docID, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Resource{docID} : {ex.Message}");
                        if (result.ContainsKey(docID))
                        {
                            result[docID] = false;
                        }
                        else
                        {
                            result.Add(docID, false);
                        }
                    }
                }
                Log.Debug($"******************** Lap number: {attempNumber} finished");

            }

            return result;
        }

        private Dictionary<Guid, bool> DeletePropertiesLoadedResourcesInt(Dictionary<Guid, List<RemoveTriples>> resourceTriples, int numAttemps = 2, bool publishHome = false)
        {
            Dictionary<Guid, bool> result = new Dictionary<Guid, bool>();
            int processedNumber = 0;
            int attempNumber = 0;
            Dictionary<Guid, List<RemoveTriples>> toDelete = new Dictionary<System.Guid, List<RemoveTriples>>(resourceTriples);
            while (toDelete != null && toDelete.Count > 0 && attempNumber < numAttemps)
            {
                int i = 0;
                int contResources = resourceTriples.Keys.Count;
                foreach (Guid docID in resourceTriples.Keys)
                {
                    i++;
                    List<ModifyResourceTriple> listaValores = new List<ModifyResourceTriple>();
                    processedNumber = processedNumber + 1;
                    attempNumber = attempNumber + 1;
                    foreach (RemoveTriples iT in resourceTriples[docID])
                    {
                        ModifyResourceTriple triple = new ModifyResourceTriple();
                        triple.old_object = iT.Value;
                        triple.predicate = iT.Predicate;
                        triple.new_object = null;
                        triple.gnoss_property = GnossResourceProperty.none;

                        if (iT.Title)
                        {
                            triple.gnoss_property = GnossResourceProperty.title;
                        }
                        else if (iT.Description)
                        {
                            triple.gnoss_property = GnossResourceProperty.description;
                        }

                        listaValores.Add(triple);
                    }
                    try
                    {
                        bool endOfLoad = false;
                        if (i == contResources)
                        {
                            endOfLoad = true;
                        }
                        ModifyTripleList(docID, listaValores, publishHome, null, null, endOfLoad);

                        Log.Debug($"{processedNumber} of {resourceTriples.Count} Object: {docID}. Resource: {resourceTriples[docID].ToArray()}");
                        toDelete.Remove(docID);

                        if (result.ContainsKey(docID))
                        {
                            result[docID] = true;
                        }
                        else
                        {
                            result.Add(docID, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Resource {docID} : {ex.Message}");
                        if (result.ContainsKey(docID))
                        {
                            result[docID] = false;
                        }
                        else
                        {
                            result.Add(docID, false);
                        }
                    }
                }
                Log.Debug($"******************** Lap number: {attempNumber} finished");
            }

            return result;
        }


        private Dictionary<System.Guid, bool> ActionsOnPropertiesLoadedResourcesInt(Dictionary<System.Guid, List<TriplesToModify>> resourceTriples, Dictionary<System.Guid, List<RemoveTriples>> deleteList, Dictionary<System.Guid, List<TriplesToInclude>> insertList, Dictionary<System.Guid, List<AuxiliaryEntitiesTriplesToInclude>> auxiliaryEntitiesInsertTriplesList, bool publishHome)
        {
            Dictionary<Guid, bool> result = new Dictionary<Guid, bool>();
            int processedNumber = 0;
            int attempNumber = 0;
            List<ModifyResourceTriple> valuesList = new List<ModifyResourceTriple>();
            List<string> values = new List<string>();
            Dictionary<Guid, List<ModifyResourceTriple>> resources = new Dictionary<System.Guid, List<ModifyResourceTriple>>();

            // Delete triples
            if (deleteList != null)
            {
                Dictionary<Guid, List<RemoveTriples>> toDelete = new Dictionary<System.Guid, List<RemoveTriples>>(deleteList);
                while (toDelete != null && toDelete.Count > 0)
                {
                    foreach (Guid docID in deleteList.Keys)
                    {
                        valuesList = new List<ModifyResourceTriple>();
                        processedNumber = processedNumber + 1;
                        attempNumber = attempNumber + 1;
                        foreach (RemoveTriples iT in deleteList[docID])
                        {
                            ModifyResourceTriple triple = new ModifyResourceTriple();
                            triple.old_object = iT.Value;
                            triple.predicate = iT.Predicate;
                            triple.new_object = null;
                            triple.gnoss_property = GnossResourceProperty.none;

                            if (iT.Title)
                            {
                                triple.gnoss_property = GnossResourceProperty.title;
                            }
                            else if (iT.Description)
                            {
                                triple.gnoss_property = GnossResourceProperty.description;
                            }

                            valuesList.Add(triple);
                        }
                        resources.Add(docID, valuesList);
                        toDelete.Remove(docID);
                    }
                }
            }

            // Modify triples
            if (resourceTriples != null)
            {
                Dictionary<Guid, List<TriplesToModify>> toModify = new Dictionary<System.Guid, List<TriplesToModify>>(resourceTriples);
                while (toModify != null && toModify.Count > 0)
                {
                    foreach (Guid docID in resourceTriples.Keys)
                    {
                        valuesList = new List<ModifyResourceTriple>();

                        // If exists, gets the values list to add the modified values
                        if (resources.ContainsKey(docID))
                        {
                            valuesList = resources[docID];
                        }
                        processedNumber = processedNumber + 1;
                        attempNumber = attempNumber + 1;
                        foreach (TriplesToModify mT in resourceTriples[docID])
                        {
                            ModifyResourceTriple triple = new ModifyResourceTriple();
                            triple.old_object = mT.OldValue;
                            triple.predicate = mT.Predicate;
                            triple.new_object = mT.NewValue;
                            triple.gnoss_property = GnossResourceProperty.none;

                            if (mT.Title)
                            {
                                triple.gnoss_property = GnossResourceProperty.title;
                            }
                            else if (mT.Description)
                            {
                                triple.gnoss_property = GnossResourceProperty.description;
                            }

                            valuesList.Add(triple);

                        }

                        // If exists, replace the value list. Else, add the value list
                        if (resources.ContainsKey(docID))
                        {
                            resources[docID] = valuesList;
                        }
                        else
                        {
                            resources.Add(docID, valuesList);
                        }
                        toModify.Remove(docID);
                    }
                }
            }

            // Insert triples
            if (insertList != null)
            {
                Dictionary<Guid, List<TriplesToInclude>> toInsert = new Dictionary<System.Guid, List<TriplesToInclude>>(insertList);
                while (toInsert != null && toInsert.Count > 0)
                {
                    foreach (Guid docID in insertList.Keys)
                    {
                        valuesList = new List<ModifyResourceTriple>();
                        if (resources.ContainsKey(docID))
                        {
                            valuesList = resources[docID];
                        }
                        processedNumber = processedNumber + 1;
                        attempNumber = attempNumber + 1;
                        foreach (TriplesToInclude iT in insertList[docID])
                        {
                            ModifyResourceTriple triple = new ModifyResourceTriple();
                            triple.old_object = null;
                            triple.predicate = iT.Predicate;
                            triple.new_object = iT.NewValue;
                            triple.gnoss_property = GnossResourceProperty.none;

                            if (iT.Title)
                            {
                                triple.gnoss_property = GnossResourceProperty.title;
                            }
                            else if (iT.Description)
                            {
                                triple.gnoss_property = GnossResourceProperty.description;
                            }

                            valuesList.Add(triple);
                        }
                        if (resources.ContainsKey(docID))
                        {
                            resources[docID] = valuesList;
                        }
                        else
                        {
                            resources.Add(docID, valuesList);
                        }
                        toInsert.Remove(docID);
                    }
                }
            }

            // Insert Auxiliary entity triples
            if (auxiliaryEntitiesInsertTriplesList != null)
            {
                Dictionary<Guid, List<AuxiliaryEntitiesTriplesToInclude>> auxiliaryEntityTriplesToInsert = new Dictionary<System.Guid, List<AuxiliaryEntitiesTriplesToInclude>>(auxiliaryEntitiesInsertTriplesList);
                while (auxiliaryEntityTriplesToInsert != null && auxiliaryEntityTriplesToInsert.Count > 0)
                {
                    foreach (Guid docID in auxiliaryEntitiesInsertTriplesList.Keys)
                    {
                        valuesList = new List<ModifyResourceTriple>();
                        if (resources.ContainsKey(docID))
                        {
                            valuesList = resources[docID];
                        }
                        processedNumber = processedNumber + 1;
                        attempNumber = attempNumber + 1;
                        foreach (AuxiliaryEntitiesTriplesToInclude iT in auxiliaryEntitiesInsertTriplesList[docID])
                        {
                            ModifyResourceTriple triple = new ModifyResourceTriple();
                            triple.old_object = null;
                            triple.predicate = iT.Predicate;
                            triple.new_object = $"{GraphsUrl}items/{iT.Name}_{docID}_{iT.Identifier}|{iT.Value}";
                            triple.gnoss_property = GnossResourceProperty.none;

                            valuesList.Add(triple);
                        }
                        if (resources.ContainsKey(docID))
                        {
                            resources[docID] = valuesList;
                        }
                        else
                        {
                            resources.Add(docID, valuesList);
                        }
                        auxiliaryEntityTriplesToInsert.Remove(docID);
                    }
                }
            }

            int i = 0;
            int contResources = resources.Keys.Count;
            // Call to the api
            foreach (Guid docID in resources.Keys)
            {
                i++;
                try
                {
                    bool endOfLoad = false;
                    if (i == contResources)
                    {
                        endOfLoad = true;
                    }
                    ModifyTripleList(docID, valuesList, publishHome, null, null, endOfLoad);
                    valuesList = new List<ModifyResourceTriple>();

                    Log.Debug($" Object: {docID}");
                    if (result.ContainsKey(docID))
                    {
                        result[docID] = true;
                    }
                    else
                    {
                        result.Add(docID, true);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Resource {docID} : {ex.Message}");
                    valuesList = new List<ModifyResourceTriple>();
                    if (result.ContainsKey(docID))
                    {
                        result[docID] = false;
                    }
                    else
                    {
                        result.Add(docID, false);
                    }
                }
            }

            return result;
        }

        private void ModifyPropertyLoadedSecondaryResourceInt(Dictionary<string, List<TriplesToModify>> resourceTriples, int numAttemps = 2, bool publishHome = false)
        {
            int processedNumber = 0;
            int attempNumber = 0;
            List<string[]> valuesList = new List<string[]>();
            List<string> values = new List<string>();
            Dictionary<string, List<TriplesToModify>> toModify = new Dictionary<string, List<TriplesToModify>>(resourceTriples);
            while (toModify != null && toModify.Count > 0 && attempNumber < numAttemps)
            {
                foreach (string secondaryEntityId in resourceTriples.Keys)
                {
                    attempNumber = attempNumber + 1;
                    processedNumber = processedNumber + 1;
                    foreach (TriplesToModify mT in resourceTriples[secondaryEntityId])
                    {
                        string acido = "0";
                        values = new List<string>();
                        values.Add(mT.OldValue);
                        values.Add(mT.Predicate);
                        values.Add(mT.NewValue);
                        values.Add(acido);
                        valuesList.Add(values.ToArray<string>());
                    }
                    try
                    {
                        string url = $"{ApiUrl}/secondary-entity/modify-triple-list";
                        ModifyTripleListModel model = new ModifyTripleListModel() { community_short_name = CommunityShortName, secondary_ontology_url = _ontologyUrl, secondary_entity = secondaryEntityId, triple_list = valuesList.ToArray() };
                        WebRequestPostWithJsonObject(url, model);

                        valuesList = new List<string[]>();
                        Log.Debug($"{processedNumber} of {resourceTriples.Count} Object: {secondaryEntityId}. Resource: {resourceTriples[secondaryEntityId].ToArray()}");
                        toModify.Remove(secondaryEntityId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Resource {secondaryEntityId} : {ex.Message}");
                        valuesList = new List<string[]>();
                    }
                }
                Log.Debug($"******************** Finished lap number: {attempNumber}");
            }
        }

        private bool InsertAuxiliarEntityOnPropertiesLoadedResourceInt(Dictionary<Guid, List<AuxiliaryEntitiesTriplesToInclude>> resourceTriples, int numAttemps = 2, bool publishHome = false, Guid? userId = null)
        {
            int processedNumber = 0;
            int attempNumber = 0;
            bool inserted = false;
            Dictionary<Guid, List<AuxiliaryEntitiesTriplesToInclude>> pDiccionarioInsertar = new Dictionary<System.Guid, List<AuxiliaryEntitiesTriplesToInclude>>(resourceTriples);
            while (pDiccionarioInsertar != null && pDiccionarioInsertar.Count > 0 && attempNumber < numAttemps)
            {
                int i = 0;
                int contResources = resourceTriples.Keys.Count;
                foreach (Guid docID in resourceTriples.Keys)
                {
                    i++;
                    List<ModifyResourceTriple> listaValores = new List<ModifyResourceTriple>();
                    attempNumber = attempNumber + 1;
                    processedNumber = processedNumber + 1;
                    foreach (AuxiliaryEntitiesTriplesToInclude iT in resourceTriples[docID])
                    {
                        ModifyResourceTriple triple = new ModifyResourceTriple();
                        triple.old_object = null;
                        triple.predicate = iT.Predicate;
                        triple.new_object = $"{GraphsUrl}items/{iT.Name}_{docID}_{iT.Identifier}|{iT.Value}";
                        triple.gnoss_property = GnossResourceProperty.none;
                        listaValores.Add(triple);
                    }
                    try
                    {
                        bool endOfLoad = false;
                        if (i == contResources)
                        {
                            endOfLoad = true;
                        }
                        ModifyTripleList(docID, listaValores, publishHome, null, null, endOfLoad, userId);
                        Log.Debug($"{processedNumber} of {resourceTriples.Count} Object: {docID}. Resource: {resourceTriples[docID].ToArray()}");
                        pDiccionarioInsertar.Remove(docID);
                        inserted = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Resource {docID} : {ex.Message}");
                    }
                }
                Log.Debug($"******************** Lap number: {attempNumber} finished");

            }
            return inserted;
        }

        #endregion

        private SparqlObject VirtuosoQueryInt(string selectPart, string wherePart, string graph, bool userMasterServer)
        {
            Log.Trace("Entering in the method", this.GetType().Name);
            Log.Trace($"SELECT: {selectPart}", this.GetType().Name);
            Log.Trace($"Grafo name: {graph}", this.GetType().Name);
            Log.Trace($"WHERE: {wherePart}", this.GetType().Name);

            SparqlObject SO = new SparqlObject();

            try
            {
                Log.Trace("Query start", this.GetType().Name);

                string url = $"{ApiUrl}/sparql-endpoint/query";

                SparqlQuery model = new SparqlQuery() { ontology = graph, community_short_name = CommunityShortName, query_select = selectPart, query_where = wherePart, userMasterServer = userMasterServer };

                string response = WebRequestPostWithJsonObject(url, model);

                SO = JsonConvert.DeserializeObject<SparqlObject>(response);

                Log.Trace("Query end", this.GetType().Name);
            }
            catch (WebException wex)
            {
                string resultado = wex.Response?.Headers["ErrorDescription"]?.Replace("<br>", "\n");
                throw new GnossAPIException($"Could not make the query {selectPart} {wherePart} to the graph {graph}.\nError: {resultado}");
            }

            Log.Trace("Leaving the method", this.GetType().Name);
            return SO;
        }

        private DataSet VirtuosoQueryIntDataSet(string selectPart, string wherePart, string graph, bool userMasterServer)
        {
            Log.Trace("Entering in the method", this.GetType().Name);
            Log.Trace($"SELECT: {selectPart}", this.GetType().Name);
            Log.Trace($"Grafo name: {graph}", this.GetType().Name);
            Log.Trace($"WHERE: {wherePart}", this.GetType().Name);

            DataSet dataSet = new DataSet();

            try
            {
                Log.Trace("Query start", this.GetType().Name);

                string url = $"{ApiUrl}/sparql-endpoint/querycsv";

                sparqlQuery model = new sparqlQuery() { ontology = graph, community_short_name = CommunityShortName, query_select = selectPart, query_where = wherePart, userMasterServer = userMasterServer };

                string response = WebRequestPostWithJsonObject(url, model);
                lock (dataSet)
                {
                    LeerResultadosCSV(response, graph, dataSet);
                }
                Log.Trace("Query end", this.GetType().Name);
            }
            catch (WebException wex)
            {
                string resultado = wex.Response?.Headers["ErrorDescription"]?.Replace("<br>", "\n");
                throw new GnossAPIException($"Could not make the query {selectPart} {wherePart} to the graph {graph}.\nError: {resultado}");
            }

            Log.Trace("Leaving the method", this.GetType().Name);
            return dataSet;
        }

        private void LeerResultadosCSV(string pResultados, string pNombreTabla, DataSet pFacetadoDS)
        {
            if (pFacetadoDS != null && pFacetadoDS.Tables.Contains(pNombreTabla))
            {
                pFacetadoDS.Tables[pNombreTabla].Clear();
            }
            else if (pFacetadoDS != null && !pFacetadoDS.Tables.Contains(pNombreTabla))
            {
                pFacetadoDS.Tables.Add(new DataTable(pNombreTabla));
            }

            string[] lineas = pResultados.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (pFacetadoDS != null && !string.IsNullOrEmpty(pResultados) && lineas.Length > 1)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(pResultados);
                MemoryStream stream = new MemoryStream(byteArray);
                DataTable csvTable = pFacetadoDS.Tables[pNombreTabla];
                using (CsvReader.CsvReader csvReader = new CsvReader.CsvReader(new StreamReader(stream), true))
                {
                    char delimiter = csvReader.Delimiter;
                    csvTable.Load(csvReader);
                    foreach (System.Data.DataColumn col in csvTable.Columns)
                    {
                        col.ReadOnly = false;
                    }
                }
            }
            else if (lineas.Length == 1)
            {
                // Creo las columnas en la tabla porque si no hay métodos que fallan
                string[] columnas = lineas[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string columna in columnas)
                {
                    string nombreCol = columna.Trim('\"');

                    if (pFacetadoDS != null && !pFacetadoDS.Tables[pNombreTabla].Columns.Contains(nombreCol))
                    {
                        pFacetadoDS.Tables[pNombreTabla].Columns.Add(nombreCol);
                    }
                }
            }
        }

        private SparqlObject VirtuosoQueryMultipleGraphInt(string selectPart, string wherePart, List<string> graph_list)
        {
            Log.Trace("Entering in the method", this.GetType().Name);
            Log.Trace($"SELECT: {selectPart}", this.GetType().Name);
            Log.Trace($"Grafo name: {graph_list}", this.GetType().Name);
            Log.Trace($"WHERE: {wherePart}", this.GetType().Name);

            SparqlObject sparqlObject = new SparqlObject();

            try
            {
                Log.Trace("Query start", this.GetType().Name);

                string url = $"{ApiUrl}/sparql-endpoint/query-multiple-graph";

                SparqlQueryMultipleGraph model = new SparqlQueryMultipleGraph() { ontology_list = graph_list, community_short_name = CommunityShortName, query_select = selectPart, query_where = wherePart };

                string response = WebRequestPostWithJsonObject(url, model);

                sparqlObject = JsonConvert.DeserializeObject<SparqlObject>(response);

                Log.Trace("Query end", this.GetType().Name);
            }
            catch (WebException wex)
            {
                string resultado = wex.Response?.Headers["ErrorDescription"]?.Replace("<br>", "\n");
                throw new GnossAPIException($"Could not make the query {selectPart} {wherePart} to the graphs {string.Join(',', graph_list)}.\nError: {resultado}");
            }

            Log.Trace("Leaving the method", GetType().Name);
            return sparqlObject;
        }

        private void PrepareAttachedToLog(List<SemanticAttachedResource> resourceAttachedFiles)
        {
            if (resourceAttachedFiles != null)
            {
                foreach (SemanticAttachedResource adjunto in resourceAttachedFiles)
                {
                    adjunto.rdf_attached_file = null;
                }
            }
        }

        private LoadResourceParams GetResourceModelOfBasicOntologyResource(BasicOntologyResource rec, bool pEsUltimo, short pTipoDoc = -1)
        {
            LoadResourceParams model = new LoadResourceParams();
            model.resource_id = rec.ShortGnossId;
            model.community_short_name = CommunityShortName;
            model.title = rec.Title;
            model.description = rec.Description;
            model.tags = rec.Tags.ToList();

            List<Guid> listaCategorias = new List<Guid>();
            if (rec.CategoriesIds != null)
            {
                foreach (Guid categoria in rec.CategoriesIds)
                {
                    if (!listaCategorias.Contains(categoria))
                    {
                        listaCategorias.Add(categoria);
                    }
                }
            }

            if (pTipoDoc != -1)
            {
                model.resource_type = pTipoDoc;
            }

            model.categories = listaCategorias;
            model.resource_url = rec.DownloadUrl;
            model.resource_file = rec.AttachedFile;
            model.creator_is_author = rec.CreatorIsAuthor;
            model.authors = rec.Author;
            model.auto_tags_title_text = rec.AutomaticTagsTextFromTitle;
            model.auto_tags_description_text = rec.AutomaticTagsTextFromDescription;
            model.create_screenshot = rec.GenerateSnapshot;
            model.url_screenshot = rec.DownloadUrl;
            model.screenshot_sizes = rec.SnapshotSizes.ToList();
            if (rec.SnapshotSizes != null)
            {
                model.screenshot_sizes = rec.SnapshotSizes.ToList();
            }
            else
            {
                model.screenshot_sizes = new List<int>();
            }
            model.end_of_load = pEsUltimo;

            model.creation_date = rec.CreationDate;

            model.publish_home = rec.PublishInHome;
            model.visibility = (short)rec.Visibility;
            model.editors_list = rec.EditorsGroups;
            model.readers_list = rec.ReadersGroups;

            return model;
        }

        private LoadResourceParams GetResourceModelOfComplexOntologyResource(ComplexOntologyResource rec, bool pCrearVersion, bool pEsUltimo)
        {
            LoadResourceParams model = new LoadResourceParams();
            model.resource_id = rec.ShortGnossId;
            model.community_short_name = CommunityShortName;
            model.title = rec.Title;
            model.description = rec.Description;
            model.resource_type = (short)TiposDocumentacion.ontology;

            if (rec.Tags != null)
            {
                model.tags = rec.Tags.ToList();
            }

            if (pCrearVersion)
            {
                model.create_version = pCrearVersion;
            }

            List<Guid> listaCategorias = new List<Guid>();
            if (rec.CategoriesIds != null)
            {
                foreach (Guid categoria in rec.CategoriesIds)
                {
                    if (!listaCategorias.Contains(categoria))
                    {
                        listaCategorias.Add(categoria);
                    }
                }
            }

            model.categories = listaCategorias;
            model.resource_url = rec.Ontology.OntologyUrl;
            model.resource_file = rec.RdfFile;

            int i = 0;
            if (rec.AttachedFilesName.Count > 0)
            {
                model.resource_attached_files = new List<SemanticAttachedResource>();
                foreach (string nombre in rec.AttachedFilesName)
                {
                    SemanticAttachedResource adjunto = new SemanticAttachedResource();
                    adjunto.file_rdf_property = nombre;
                    adjunto.file_property_type = (short)rec.AttachedFilesType[i];
                    adjunto.rdf_attached_file = rec.AttachedFiles[i];
                    adjunto.delete_file = rec.AttachedFiles[i].Equals(null);
                    i++;
                    model.resource_attached_files.Add(adjunto);
                }
            }

            model.creator_is_author = rec.CreatorIsAuthor;
            model.authors = rec.Author;
            model.auto_tags_title_text = rec.AutomaticTagsTextFromTitle;
            model.auto_tags_description_text = rec.AutomaticTagsTextFromDescription;
            model.create_screenshot = rec.MustGenerateScreenshot;
            model.url_screenshot = rec.ScreenshotUrl;
            model.predicate_screenshot = rec.ScreenshotPredicate;
            if (rec.ScreenshotSizes != null)
            {
                model.screenshot_sizes = rec.ScreenshotSizes.ToList();
            }
            model.end_of_load = pEsUltimo;

            model.creation_date = rec.CreationDate;

            model.publisher_email = rec.PublisherEmail;
            model.publish_home = rec.PublishInHome;
            model.main_image = rec.MainImage;
            model.visibility = (short)rec.Visibility;
            model.editors_list = rec.EditorsGroups;
            model.readers_list = rec.ReadersGroups;

            model.aumented_reading = rec.AumentedReading;

            return model;
        }

        #endregion

        #region REST methods

        /// <summary>
        /// Method to know if there are pending resources in a community
        /// </summary>
        /// <returns>The number of pending actions in a community</returns>
        public int GetPendingActions(string ontologyUrl = null)
        {
            try
            {
                string ontology = ontologyUrl;
                if (string.IsNullOrEmpty(ontologyUrl))
                {
                    ontology = GetOntologyNameWithExtensionFromUrlOntology(OntologyUrl);
                }

                string url = $"{ApiUrl}/resource/get-pending-actions?ontology_name={ontology}&community_short_name={CommunityShortName}";

                string response = WebRequest($"GET", url, acceptHeader: "application/json");
                int pendingActions = int.Parse(response);

                Log.Debug($"The ontology {ontology} has {pendingActions} pending actions");

                return pendingActions;
            }
            catch (Exception ex)
            {
                Log.Error($"There has been an error trying to know if there are outstanding resources{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the community short name to which a resource belongs
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>Community short name"</returns>
        public string GetCommunityShortNameByResourceID(Guid resourceId)
        {
            string communityShortName = string.Empty;
            try
            {
                string url = $"{ApiUrl}/resource/get-community-short-name-by-resource_id?resource_id={resourceId}";

                communityShortName = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded")?.Trim('"');

                Log.Debug($"The community short name for the resource {resourceId} is {communityShortName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the community short name", ex.Message);
                throw;
            }
            return communityShortName;
        }

        /// <summary>
        /// Checks whether the user has permission on the resource editing
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>True if the user has editing permission on the resource. False if not.</returns>
        public bool HasUserEditingPermissionOnResourceByCommunityName(Guid resourceId, Guid userId)
        {
            bool result = false;

            if (!resourceId.Equals(Guid.Empty) && !userId.Equals(Guid.Empty))
            {
                try
                {
                    string url = $"{ApiUrl}/resource/get-user-editing-permission-on-resource-by-community-name?resource_id={resourceId}&user_id={userId}&community_short_name={CommunityShortName}";

                    string response = WebRequest($"GET", url);
                    result = JsonConvert.DeserializeObject<bool>(response);

                    if (result)
                    {
                        Log.Debug($"The user {userId} is allowed to edit the resource {resourceId} in {CommunityShortName}");
                    }
                    else
                    {
                        Log.Debug($"The user {userId} is not allowed to edit the resource {resourceId} in {CommunityShortName}");
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
                Log.Error($"Any of this are null or empty: resourceId, userId");
            }

            return result;
        }

        /// <summary>
        /// Checks whether the user has permission on the resource editing
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="userId">User identifier</param>
        /// <param name="communityId">Community identifier</param>
        /// <returns>True if the user has editing permission on the resource. False if not.</returns>
        public bool HasUserEditingPermissionOnResourceByCommunityID(Guid resourceId, Guid userId, Guid communityId)
        {
            bool result = false;

            if (!resourceId.Equals(Guid.Empty) && !userId.Equals(Guid.Empty))
            {
                try
                {
                    string url = $"{ApiUrl}/resource/get-user-editing-permission-on-resource?resource_id={resourceId}&user_id={userId}&community_id={communityId}";

                    string response = WebRequest($"GET", url);
                    result = JsonConvert.DeserializeObject<bool>(response);

                    if (result)
                    {
                        Log.Debug($"The user {userId} is allowed to edit the resource {resourceId} in {CommunityShortName}");
                    }
                    else
                    {
                        Log.Debug($"The user {userId} is not allowed to edit the resource {resourceId} in {CommunityShortName}");
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
                Log.Error($"Any of this are null or empty: resourceId, userId");
            }

            return result;
        }

        /// <summary>
        /// Gets the visibility of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>ResourceVisibility with the visibility of the resource. Null if it fails</returns>
        public ResourceVisibility? GetResourceVisibility(Guid resourceId)
        {
            ResourceVisibility? visibilidad = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-visibility?resource_id={resourceId}";
                string result = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                visibilidad = JsonConvert.DeserializeObject<ResourceVisibility>(result);

                if (visibilidad == null)
                {
                    Log.Error($"Resource visbility not obtained: {resourceId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the visibility of {resourceId}: {ex.Message}");
                throw;
            }
            return visibilidad;
        }

        /// <summary>
        /// Gets the related resources of a list of resources
        /// </summary>
        /// <param name="resourceIds">Resource identifiers</param>
        /// <returns>Related resources</returns>
        public Dictionary<Guid, List<Guid>> GetRelatedResourcesFromList(List<Guid> resourceIds)
        {
            Dictionary<Guid, List<Guid>> listaIds = null;
            try
            {
                var aux = new { resource_ids = resourceIds, community_short_name = CommunityShortName };
                string url = $"{ApiUrl}/resource/get-related-resources-from-list";
                string result = WebRequestPostWithJsonObject(url, aux);
                listaIds = JsonConvert.DeserializeObject<Dictionary<Guid, List<Guid>>>(result);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the related resources of {string.Join(",", resourceIds)}: {ex.Message}");
                throw;
            }
            return listaIds;
        }

        /// <summary>
        /// Gets the related resources of a resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>Related resources</returns>
        public List<Guid> GetRelatedResources(Guid resourceId)
        {
            List<Guid> listaIds = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-related-resources?resource_id={resourceId}&community_short_name={CommunityShortName}";
                string result = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                listaIds = JsonConvert.DeserializeObject<List<Guid>>(result);
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the related resources of {resourceId}: {ex.Message}");
                throw;
            }
            return listaIds;
        }

        //GetDocumentsPublishedByUser
        /// <summary>
        /// Gets the documents publisher by user
        /// </summary>
        /// <param name="userid">User identifier</param>
        /// <returns>List of community names</returns>
        public Dictionary<string, List<Guid>> GetDocumentsPublishedByUser(Guid userid)
        {
            Dictionary<string, List<Guid>> listaDocs = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-documents-published-by-user?user_id={userid}";
                string result = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");

                listaDocs = JsonConvert.DeserializeObject<Dictionary<string, List<Guid>>>(result);

                Log.Debug($"the user id {userid} published {result}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the shared communities of {userid}: {ex.Message}");
                throw;
            }
            return listaDocs;
        }

        public string ObtenerPathEstilos(Guid proyectoid)
        {
            string valor = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-path-styles?id_proyecto={proyectoid}";
                string result = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");

                valor = JsonConvert.DeserializeObject<string>(result);

                Log.Debug($"the proyect id {proyectoid} have styles path {result}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the shared communities of {proyectoid}: {ex.Message}");
                throw;
            }
            return valor;
        }



        /// <summary>
        /// Gets the communities where a resource has been shared
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>List of community names</returns>
        public List<string> GetCommunitiesResourceShared(Guid resourceId)
        {
            List<string> communities = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-communities-resource-shared?resource_id={resourceId}";
                string result = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");

                communities = JsonConvert.DeserializeObject<List<string>>(result);

                Log.Debug($"The resource {resourceId} has been shared in {result}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the shared communities of {resourceId}: {ex.Message}");
                throw;
            }
            return communities;
        }

        /// <summary>
        /// Gets the readers or the readers groups short name of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>List of strings with the short names</returns>
        public KeyReaders GetResourceReaders(Guid resourceId)
        {
            KeyReaders readers = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-resource-readers?resource_id={resourceId}";
                string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                readers = JsonConvert.DeserializeObject<KeyReaders>(response);
                if (readers != null)
                {
                    Log.Debug($"Resource readers of {resourceId}: {response}");
                }
                else
                {
                    Log.Error($"Couldn't get readers of: {resourceId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the resources of {resourceId}: {ex.Message}");
                throw;
            }
            return readers;
        }

        /// <summary>
        /// Unshare resource of a community
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="communityShortName">Community short name</param>
        /// <returns>True if the resource has been unshared. False if not.</returns>
        public bool UnsharedCommunityResource(Guid resourceId)
        {
            bool unshared = false;
            try
            {
                string url = $"{ApiUrl}/resource/unshared-community-resource";

                UnsharedResourceParams parameters = new UnsharedResourceParams() { resource_id = resourceId, community_short_name = CommunityShortName };

                string response = WebRequestPostWithJsonObject(url, parameters);
                unshared = JsonConvert.DeserializeObject<bool>(response);

                if (unshared)
                {
                    Log.Debug($"Resource {resourceId} unshared from {CommunityShortName}");
                }
                else
                {
                    Log.Debug($"Resource {resourceId} not unshared from {CommunityShortName}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error trying to unsare the resource {resourceId} from {CommunityShortName}: {ex.Message}");
                throw;
            }
            return unshared;
        }

        /// <summary>
        /// Gets the short names of resource editors and editors groups.
        /// </summary>
        /// <param name="resourceId_list">resources identifiers list</param>
        /// <returns>List with the short names of editors and editors groups</returns>
        public List<KeyEditors> GetEditors(List<Guid> resourceId_list)
        {
            List<KeyEditors> editorsList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-editors";
                string response = WebRequestPostWithJsonObject(url, resourceId_list);
                editorsList = JsonConvert.DeserializeObject<List<KeyEditors>>(response);

                if (editorsList != null && editorsList.Count > 0)
                {
                    Log.Debug($"Editors of the resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    Log.Debug($"There is no editors for resources: {JsonConvert.SerializeObject(resourceId_list)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the editors of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }

            return editorsList;
        }

        /// <summary>
        /// Gets the resources download urls
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <returns>ResponseGetUrl list with the existent resources download urls</returns>
        public List<ResponseGetUrl> GetDownloadUrl(List<Guid> resourceId_list)
        {
            List<ResponseGetUrl> urlsList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-download-url";
                GetDownloadUrlParams parameters = new GetDownloadUrlParams() { resource_id_list = resourceId_list, community_short_name = CommunityShortName };
                string response = WebRequestPostWithJsonObject(url, parameters);
                urlsList = JsonConvert.DeserializeObject<List<ResponseGetUrl>>(response);

                if (urlsList != null && urlsList.Count == 0)
                {
                    Log.Debug($"Download urls of resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    Log.Debug($"There is no download urls for resources: {JsonConvert.SerializeObject(resourceId_list)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the download urls of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return urlsList;
        }

        /// <summary>
        /// Gets the resources urls in the indicated language
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <param name="language">language code string</param>
        /// <returns>Resource.ResponseGetUrl list with the existent resources urls</returns>
        public List<ResponseGetUrl> GetUrl(List<Guid> resourceId_list, string language)
        {
            List<ResponseGetUrl> urlsList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-url";
                GetUrlParams parameters = new GetUrlParams() { resource_id_list = resourceId_list, community_short_name = CommunityShortName, language = language };
                string response = WebRequestPostWithJsonObject(url, parameters);
                urlsList = JsonConvert.DeserializeObject<List<ResponseGetUrl>>(response);

                if (urlsList != null && urlsList.Count == 0)
                {
                    Log.Debug($"Urls of resources {JsonConvert.SerializeObject(parameters)}: {response}");
                }
                else
                {
                    Log.Debug($"There is no urls for resources: {JsonConvert.SerializeObject(parameters)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the urls of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return urlsList;
        }

        /// <summary>
        /// Sets the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="visibility">Resource visibility</param>
        /// <param name="readers_list">Resource readers</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        public void SetReaders(Guid resourceId, ResourceVisibility visibility, List<ReaderEditor> readers_list, bool publishHome = false)
        {
            SetReadersEditorsParams readers = null;
            try
            {
                string url = $"{ApiUrl}/resource/set-readers";
                readers = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, publish_home = publishHome, readers_list = readers_list, visibility = (short)visibility };
                WebRequestPostWithJsonObject(url, readers);

                Log.Debug($"Ended resource readers setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} readers. \r\n Json: {JsonConvert.SerializeObject(readers)}", ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Adds the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="readers_list">Resource readers</param>
        public void AddReaders(Guid resourceId, List<ReaderEditor> readers_list)
        {
            SetReadersEditorsParams readers = null;
            try
            {
                string url = $"{ApiUrl}/resource/add-readers";
                readers = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, readers_list = readers_list };
                WebRequestPostWithJsonObject(url, readers);

                Log.Debug($"Ended resource readers setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} readers. \r\n Json: {JsonConvert.SerializeObject(readers)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// remove the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="readers_list">Resource readers</param>
        public void RemoveReaders(Guid resourceId, List<ReaderEditor> readers_list)
        {
            SetReadersEditorsParams readers = null;
            try
            {
                string url = $"{ApiUrl}/resource/remove-readers";
                readers = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, readers_list = readers_list };
                WebRequestPostWithJsonObject(url, readers);

                Log.Debug($"Ended resource readers setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} readers. \r\n Json: {JsonConvert.SerializeObject(readers)}", ex.Message);
                throw;
            }
        }




        /// <summary>
        /// Sets the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="visibility">Resource visibility</param>
        /// <param name="readers_list">Resource readers</param>
        /// <param name="publishHome">Indicates whether the home must be updated</param>
        public void SetEditors(Guid resourceId, ResourceVisibility visibility, List<ReaderEditor> readers_list, bool publishHome = false)
        {
            SetReadersEditorsParams editors = null;
            try
            {
                string url = $"{ApiUrl}/resource/set-editors";
                editors = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, publish_home = publishHome, readers_list = readers_list, visibility = (short)visibility };
                WebRequestPostWithJsonObject(url, editors);

                Log.Debug($"Ended resource editors setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} editors.\r\n Json: {JsonConvert.SerializeObject(editors)}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Add the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="readers_list">Resource readers</param>
        public void AddEditors(Guid resourceId, List<ReaderEditor> readers_list)
        {
            SetReadersEditorsParams editors = null;
            try
            {
                string url = $"{ApiUrl}/resource/add-editors";
                editors = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, readers_list = readers_list };
                WebRequestPostWithJsonObject(url, editors);

                Log.Debug($"Ended resource editors setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} editors.\r\n Json: {JsonConvert.SerializeObject(editors)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// remove the readers of the resuorce
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="readers_list">Resource readers</param>
        public void RemoveEditors(Guid resourceId, List<ReaderEditor> readers_list)
        {
            SetReadersEditorsParams editors = null;
            try
            {
                string url = $"{ApiUrl}/resource/remove-editors";
                editors = new SetReadersEditorsParams() { resource_id = resourceId, community_short_name = CommunityShortName, readers_list = readers_list };
                WebRequestPostWithJsonObject(url, editors);

                Log.Debug($"Ended resource editors setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource {resourceId} editors.\r\n Json: {JsonConvert.SerializeObject(editors)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Insert or update Votes for document
        /// </summary>
        /// <param name="pIdentidadID">Client ID</param>
        /// <param name="pValorVoto">Value of vote</param>
        /// <param name="pDocumentoID">Document ID</param>
        /// <param name="pProyectoID">Proyect ID</param>
        public void VoteDocument(Guid pIdentidadID, float pValorVoto, Guid pDocumentoID, Guid pProyectoID)
        {
            VotedParameters vote = null;
            try
            {
                string url = $"{ApiUrl}/resource/vote-document";
                vote = new VotedParameters() { user_id = pIdentidadID, vote_value = pValorVoto, resource_id = pDocumentoID, project_id = pProyectoID };
                WebRequestPostWithJsonObject(url, vote);

                Log.Debug($"Ended vote document setting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error vote Document {pDocumentoID}.\r\n Json: {JsonConvert.SerializeObject(vote)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the email of the resources creators
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <returns>ResponseGetCreatorEmail list with the email of the resources creators</returns>
        public Dictionary<Guid, string> GetCreatorEmail(List<Guid> resourceId_list)
        {
            GetDownloadUrlParams model = new GetDownloadUrlParams();
            model.community_short_name = CommunityShortName;
            model.resource_id_list = resourceId_list;

            Dictionary<Guid, string> emailsList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-creator-email";
                string response = WebRequestPostWithJsonObject(url, model);
                emailsList = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(response);

                if (emailsList != null && emailsList.Count == 0)
                {
                    Log.Debug($"Urls of resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    Log.Debug($"There is no urls for resources: {JsonConvert.SerializeObject(resourceId_list)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the urls of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return emailsList;
        }

        /// <summary>
        /// Gets the categories of the resources
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <returns>ResponseGetCategories list with the category identifiers of the existent resources</returns>
        public List<ResponseGetCategories> GetCategories(List<Guid> resourceId_list)
        {
            GetDownloadUrlParams parameters = new GetDownloadUrlParams() { community_short_name = CommunityShortName, resource_id_list = resourceId_list };
            List<ResponseGetCategories> categoriesList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-categories";
                string response = WebRequestPostWithJsonObject(url, parameters);
                categoriesList = JsonConvert.DeserializeObject<List<ResponseGetCategories>>(response);

                if (categoriesList != null && categoriesList.Count == resourceId_list.Count)
                {
                    Log.Debug($"Categories of resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    List<Guid> listaRecursosSinCategoria = resourceId_list.Where(item => !categoriesList.Any(item2 => item2.resource_id.Equals(item))).ToList();

                    Log.Debug($"There is no categories for resources: {JsonConvert.SerializeObject(listaRecursosSinCategoria)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the categories of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return categoriesList;
        }

        /// <summary>
        /// Gets the tags of the resources
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <returns>ResponseGetTags list with the tags of the resources</returns>
        public List<ResponseGetTags> GetTags(List<Guid> resourceId_list)
        {
            List<ResponseGetTags> tagsList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-tags";
                string response = WebRequestPostWithJsonObject(url, resourceId_list);
                tagsList = JsonConvert.DeserializeObject<List<ResponseGetTags>>(response);

                if (tagsList != null && tagsList.Count == resourceId_list.Count)
                {
                    Log.Debug($"Tags of resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    List<Guid> listaRecursosSinTags = resourceId_list.Where(item => !tagsList.Any(item2 => item2.resource_id.Equals(item))).ToList();

                    Log.Debug($"There is no tags for resources: {JsonConvert.SerializeObject(resourceId_list)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the tags of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return tagsList;
        }

        /// <summary>
        /// Gets the main image of the resources
        /// </summary>
        /// <param name="resourceId_list">Resources identifiers list</param>
        /// <returns>ResponseGetMainImage list with the path of the main image of the resources and their available sizes</returns>
        public List<ResponseGetMainImage> GetMainImage(List<Guid> resourceId_list)
        {
            List<ResponseGetMainImage> mainImagesList = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-main-image";
                string response = WebRequestPostWithJsonObject(url, resourceId_list);
                mainImagesList = JsonConvert.DeserializeObject<List<ResponseGetMainImage>>(response);

                if (mainImagesList != null && mainImagesList.Count == 0)
                {
                    Log.Debug($"Main images of resources {JsonConvert.SerializeObject(resourceId_list)}: {response}");
                }
                else
                {
                    Log.Debug($"There is no main images for resources: {JsonConvert.SerializeObject(resourceId_list)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the main images of resources: {JsonConvert.SerializeObject(resourceId_list)}. Error description: {ex.Message}");
                throw;
            }
            return mainImagesList;
        }

        //// <summary>
        /// Gets the resource novelties in the community from the search date
        /// </summary>
        /// <param name="resources_id">Resources identifier</param>
        /// <example>Get resource/get-increased-reading-by-resources</example>
        public Dictionary<Guid, AumentedReading> GetIncreasedReading(List<Guid> resourceId_list)
        {
            Dictionary<Guid, AumentedReading> resource = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-increased-reading-by-resources";
                string response = WebRequestPostWithJsonObject(url, resourceId_list);
                //string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded")?.Trim('"');
                resource = JsonConvert.DeserializeObject<Dictionary<Guid, AumentedReading>>(response);

                if (resource != null)
                {
                    Log.Debug($"Increased reading obtained");
                }
                else
                {
                    Log.Debug($"Error getting increased reading");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the increased reading", ex.Message);
                throw;
            }
            return resource;
        }

        public Resource GetResource(Guid resourceId)
        {
            Resource resource = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-resource?resource_id={resourceId}&community_short_name={HttpUtility.UrlEncode(CommunityShortName)}";
                string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded")?.Trim('"');
                resource = JsonConvert.DeserializeObject<Resource>(response);

                if (resource != null)
                {
                    Log.Debug($"Resource {resourceId} obtained");
                }
                else
                {
                    Log.Debug($"Error getting the resource {resourceId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the resource {resourceId}", ex.Message);
                throw;
            }
            return resource;
        }

        /// <summary>
        /// Gets the rdf of the complex semanthic resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <returns>String with the rdf of the resource</returns>
        public string GetRDF(Guid resourceId)
        {
            string rdf = string.Empty;
            try
            {
                string url = $"{ApiUrl}/resource/get-rdf?resource_id={resourceId}";
                rdf = WebRequest($"GET", url, acceptHeader: "application/json")?.Trim('"');

                if (!string.IsNullOrEmpty(rdf))
                {
                    Log.Debug($"Rdf obtained for the resource {resourceId}");
                }
                else
                {
                    Log.Debug($"There is no rdf for the resource {resourceId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the rdf for the resource {resourceId}", ex.Message);
                throw;
            }
            return rdf;
        }

        /// <summary>
        /// Inserts the value in the graph
        /// </summary>
        /// <param name="graph">Graph identifier</param>
        /// <param name="value">Value to insert in the graph</param>
        public void InsertAttribute(string graph, string value)
        {
            InsertAttributeParams insertAttribute = null;
            try
            {
                string url = $"{ApiUrl}/resource/insert-attribute";
                insertAttribute = new InsertAttributeParams() { graph = graph, value = value };
                WebRequestPostWithJsonObject(url, insertAttribute);

                Log.Debug($"Ended inserting the value: {value} in the graph: {graph}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error inserting value: {value} in the graph: {graph}. Error description: {ex.Message}.");
                throw;
            }
        }

        /// <summary>
        /// Logical delete of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="endOfCharge">marks the end of the charge</param>
        public void Delete(Guid resourceId, bool endOfCharge = false)
        {
            DeleteParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/delete";
                model = new DeleteParams() { resource_id = resourceId, community_short_name = CommunityShortName, end_of_load = endOfCharge };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug("Ended resource deleting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting resource {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Persistent delete of the resource
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="deleteAttached">indicates if the attached resources must be deleted</param>
        /// <param name="endOfCharge">Indicates the end of the load. If its false, cache will not be deleted</param>
        public bool PersistentDelete(Guid resourceId, bool deleteAttached = false, bool endOfCharge = false)
        {
            bool deleted = false;
            PersistentDeleteParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/persistent-delete";
                model = new PersistentDeleteParams() { resource_id = resourceId, community_short_name = CommunityShortName, end_of_load = endOfCharge, delete_attached = deleteAttached };
                WebRequestPostWithJsonObject(url, model);
                deleted = true;
                Log.Debug("Ended resource persistent deleting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error on persistent deleting resource {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return deleted;
        }

        /// <summary>
        /// Checks whether the url exists in a resource of the community. (Searchs on the resource description)
        /// </summary>
        /// <param name="url">link to search in the community</param>
        /// <returns>True if the link exists in a resource of the community</returns>
        public bool ExistsUrl(string url)
        {
            bool exists = false;
            ExistsUrlParams model = null;
            try
            {
                string urlMethod = $"{ApiUrl}/resource/exists-url";
                model = new ExistsUrlParams() { community_short_name = CommunityShortName, url = url };
                string response = WebRequestPostWithJsonObject(urlMethod, model);
                exists = JsonConvert.DeserializeObject<bool>(response);
                Log.Debug("Ended resource persistent deleting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error on a searching of an url. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return exists;
        }

        /// <summary>
        /// Loads the images of a not yet loaded resource.
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="imageList">resource attached files. List of SemanticAttachedResource</param>
        /// SemanticAttachedResource:
        ///     file_rdf_properties = image name
        ///     file_property_type = type of file
        ///     rdf_attached_files = image to load
        /// <param name="mainImage">main image string</param>
        public bool UploadImages(Guid resourceId, List<SemanticAttachedResource> imageList, string mainImage)
        {
            bool loaded = false;
            UploadImagesParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/upload-images";
                model = new UploadImagesParams() { resource_id = resourceId, community_short_name = CommunityShortName, resource_attached_files = imageList, main_image = mainImage };
                WebRequestPostWithJsonObject(url, model);
                loaded = true;
                Log.Debug("Ended images upload");
            }
            catch (Exception ex)
            {
                Log.Error($"Error uploading resource {resourceId} images. \r\n Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return loaded;
        }

        /// <summary>
        /// Link the resources in the list to another resource.
        /// </summary>
        /// <param name="resourceId">Resource that has been linked</param>
        /// <param name="resourceListToLink">Resource list to link
        public bool LinkResource(Guid resourceId, List<Guid> resourceListToLink)
        {
            bool loaded = false;
            LinkedParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/link-resource";
                model = new LinkedParams() { resource_id = resourceId, community_short_name = CommunityShortName, resoruce_list_to_link = resourceListToLink };
                WebRequestPostWithJsonObject(url, model);
                loaded = true;
                Log.Debug("Ended link resources");
            }
            catch (Exception ex)
            {
                Log.Error($"Error linked resource {resourceId}. \r\n Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return loaded;
        }

        /// <summary>
        /// Shares the resource in the target community
        /// </summary>
        /// <param name="targetCommunity">target community short name string</param>
        /// <param name="categories">categories guid list where the document is going to be shared to</param>
        /// <param name="resourceId">resource identifier Guid</param>
        public bool Share(string targetCommunity, Guid resourceId, List<Guid> categories, string publisher_email)
        {
            bool shared = false;
            ShareParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/share";
                model = new ShareParams() { destination_communitiy_short_name = targetCommunity, resource_id = resourceId, categories = categories, publisher_email = publisher_email };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug("Ended resource sharing");

                shared = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error sharing resource {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return shared;
        }


        /// <summary>
        /// Shares the resources
        /// </summary>
        /// <param name="parameters">List of shareParams model</param>
        public bool ShareResources(List<ShareParams> parameters)
        {
            bool shared = false;
            try
            {
                string url = $"{ApiUrl}/resource/share-resources";
                WebRequestPostWithJsonObject(url, parameters);

                Log.Debug("Ended resource sharing");

                shared = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error sharing resources. \r\n: Json: {JsonConvert.SerializeObject(parameters)}", ex.Message);
                throw;
            }

            return shared;
        }
        /// <summary>
        /// Sets the resource main image
        /// </summary>
        /// <param name="resourceId">resource identifier Guid</param>
        /// <param name="path">relative path with the image name, image sizes available and '[IMGPrincipal]' mask</param>
        public bool SetMainImage(Guid resourceId, string path)
        {
            bool setted = false;
            SetMainImageParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/set-main-image";
                model = new SetMainImageParams() { community_short_name = CommunityShortName, resource_id = resourceId, path = path };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug($"Ended resource main image setting");

                setted = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error setting resource main image {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return setted;
        }

        /// <summary>
        /// Adds a comment in a resource. It can be a response of another parent comment.
        /// </summary>
        /// <param name="resourceId">resource identifier</param>
        /// <param name="commentDate">publish date of the comment</param>
        /// <param name="description">Html content of the comment wrapped in a Html paragraph and special characters encoded in ANSI. Example: <p>Descripci&amp;oacute;n del comentario</p> string</param>
        /// <param name="parentCommentId">optional parent comment identifier Guid. The current comment is its answer</param>
        /// <param name="publishHome">indicates whether the home must be updated</param>
        /// <param name="userShortName">publisher user short name</param>
        /// <returns>Comment identifier</returns>
        public Guid Comment(Guid resourceId, string userShortName, string description, Guid parentCommentId, DateTime commentDate, bool publishHome)
        {
            Guid commentId = Guid.Empty;
            CommentParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/comment";
                model = new CommentParams() { resource_id = resourceId, community_short_name = CommunityShortName, user_short_name = userShortName, html_description = description, comment_date = commentDate, parent_comment_id = parentCommentId, publish_home = publishHome };
                string response = WebRequestPostWithJsonObject(url, model);

                if (Guid.TryParse(response, out commentId))
                {
                    Log.Debug($"Ended resource {resourceId} comment: {commentId}");
                }
                else
                {
                    Log.Debug($"Error commenting resource {resourceId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error commenting resource {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return commentId;
        }

        /// <summary>
        /// Modfies a property of a resource
        /// </summary>
        /// <param name="resourceId">resource identifier guid</param>
        /// <param name="newObject">new value of the property</param>
        /// <param name="property">property to modify</param>
        public void ModifyProperty(Guid resourceId, string property, string newObject)
        {
            ModifyResourceProperty model = null;
            try
            {
                string url = $"{ApiUrl}/resource/modify-property";
                model = new ModifyResourceProperty() { resource_id = resourceId, community_short_name = CommunityShortName, property = property, new_object = newObject };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug("Ended resource deleting");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting resource {resourceId}. \r\n: Json: {JsonConvert.SerializeObject(model)} Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a basic ontology resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>resource identifier guid</returns>
        public string CreateBasicOntologyResource(LoadResourceParams parameters)
        {
            string resourceId = string.Empty;

            try
            {
                string url = $"{ApiUrl}/resource/create-basic-ontology-resource";
                resourceId = WebRequestPostWithJsonObject(url, parameters)?.Trim('"');

                if (!string.IsNullOrEmpty(resourceId))
                {
                    Log.Debug($"Basic ontology resource created: {resourceId}");
                }
                else
                {
                    Log.Debug($"Basic ontology resource not created: {JsonConvert.SerializeObject(parameters)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error trying to create a basic ontology resource. \r\n Error description: {ex.Message}. \r\n: Json: {JsonConvert.SerializeObject(parameters)}");
                throw;
            }
            return resourceId;
        }

        /// <summary>
        /// Creates a complex ontology resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>resource identifier guid</returns>
        public string CreateComplexOntologyResource(LoadResourceParams parameters)
        {
            string resourceId = string.Empty;

            try
            {
                string url = $"{ApiUrl}/resource/create-complex-ontology-resource";
                resourceId = WebRequestPostWithJsonObject(url, parameters)?.Trim('"');

                if (!string.IsNullOrEmpty(resourceId))
                {
                    Log.Debug($"Complex ontology resource created: {resourceId}");
                }
                else
                {
                    PrepareAttachedToLog(parameters.resource_attached_files);
                    Log.Debug($"Complex ontology resource not created: {JsonConvert.SerializeObject(parameters)}");
                }
            }
            catch (Exception ex)
            {
                PrepareAttachedToLog(parameters.resource_attached_files);
                Log.Error($"Error trying to create a complex ontology resource. \r\n Error description: {ex.Message}. \r\n: Json: {JsonConvert.SerializeObject(parameters)}");
                throw;
            }

            return resourceId;
        }


        /// <summary>
        /// Creates a complex ontology resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>resource identifier guid</returns>
        public string MassiveComplexOntologyResourceCreation(List<ComplexOntologyResource> parameters, Guid pCargaID, bool hierarquicalCategories = false)
        {
            List<LoadResourceParams> listaLoadResourceParams = new List<LoadResourceParams>();
            foreach (ComplexOntologyResource resource in parameters)
            {
                if (resource.TextCategories != null && resource.TextCategories.Count > 0)
                {
                    if (hierarquicalCategories)
                    {
                        resource.CategoriesIds = GetHierarquicalCategoriesIdentifiersList(resource.TextCategories);

                    }
                    else
                    {
                        resource.CategoriesIds = GetNotHierarquicalCategoriesIdentifiersList(resource.TextCategories);
                    }
                }

                string documentId = string.Empty;

                LoadResourceParams resourceParam = GetResourceModelOfComplexOntologyResource(resource, false, false);
                listaLoadResourceParams.Add(resourceParam);
                resource.Uploaded = true;
            }
            MassiveResourceLoad massiveLoad = new MassiveResourceLoad();
            massiveLoad.resources = listaLoadResourceParams;
            massiveLoad.load_id = pCargaID;
            string resourceId = string.Empty;
            try
            {
                string url = $"{ApiUrl}/resource/massive-complex-ontology-resource-creation";
                WebRequestPostWithJsonObject(url, massiveLoad)?.Trim('"');

                if (!string.IsNullOrEmpty(resourceId))
                {
                    Log.Debug($"Complex ontology resource created: {resourceId}");
                }
                else
                {
                    Log.Debug($"Massive Complex ontology resource not created: {JsonConvert.SerializeObject(massiveLoad)}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error trying to create a Massive complex ontology resource. \r\n Error description: {ex.Message}. \r\n: Json: {JsonConvert.SerializeObject(massiveLoad)}");
                throw;
            }

            return resourceId;
        }

        /// <summary>
        /// Modify a categories resource
        /// </summary>
        /// <param name="pResouceID">Resource identifier</param>
        /// <param name="pCategoriesIDs">Categories to modify</param>
        /// <returns>True if modify correct</returns>
        public bool ModifyCategoriasRecursoInt(Guid pResouceID, List<Guid> pCategoriesIDs, string pCommunityShortName)
        {
            bool modified = false;
            try
            {
                string url = $"{ApiUrl}/resource/chage-categories-resource";

                ModifyResourceCategories parameters = new ModifyResourceCategories() { resource_id = pResouceID, categories = pCategoriesIDs, community_short_name = pCommunityShortName };
                string response = WebRequestPostWithJsonObject(url, parameters);

                if (!string.IsNullOrEmpty(response))
                {
                    modified = JsonConvert.DeserializeObject<bool>(response);
                    Log.Debug($"categories resource modified: {pResouceID}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error modifying categories {pResouceID}.", ex.Message);
                throw;
            }

            return modified;
        }

        /// <summary>
        /// Modifies a complex ontology resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>resource identifier guid</returns>
        public bool ModifyComplexOntologyResource(LoadResourceParams parameters)
        {
            bool modified = false;
            try
            {
                string url = $"{ApiUrl}/resource/modify-complex-ontology-resource";
                string response = WebRequestPostWithJsonObject(url, parameters);

                if (!string.IsNullOrEmpty(response))
                {
                    modified = JsonConvert.DeserializeObject<bool>(response);
                    Log.Debug($"Complex ontology resource modified: {parameters.resource_id}");
                }
            }
            catch (Exception ex)
            {
                PrepareAttachedToLog(parameters.resource_attached_files);
                Log.Error($"Error modifying resource {parameters.resource_id}. \r\n: Json: {JsonConvert.SerializeObject(parameters)}", ex.Message);
                throw;
            }

            return modified;
        }

        /// <summary>
        /// Modifies a basic ontology resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>resource identifier guid</returns>
        public bool ModifyBasicOntologyResource(LoadResourceParams parameters)
        {
            bool modified = false;
            try
            {
                string url = $"{ApiUrl}/resource/modify-basic-ontology-resource";
                string response = WebRequestPostWithJsonObject(url, parameters);

                Log.Debug("Ended resource main image setting");

                modified = JsonConvert.DeserializeObject<bool>(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Error modifying resource {parameters.resource_id}. \r\n: Json: {JsonConvert.SerializeObject(parameters)}", ex.Message);
                throw;
            }

            return modified;
        }

        /// <summary>
        /// Method to add / modify / delete triples of complex ontology resource
        /// </summary>
        /// <param name="resourceId">resource identifier guid</param>
        /// <param name="triplesList">resource triples list to modify</param>
        /// <param name="loadId">charge identifier string</param>
        /// <param name="resourceAttachedFiles">resource attached files list</param>
        /// <param name="mainImage">main image string</param>
        /// <param name="publishHome">indicates whether the home must be updated</param>
        /// <param name="endOfLoad">indicates the resource modified is the last and it must deletes cache</param>
        /// <param name="userId">User that try to modify the resource </param>
        public void ModifyTripleList(Guid resourceId, List<ModifyResourceTriple> triplesList, bool publishHome, string mainImage, List<SemanticAttachedResource> resourceAttachedFiles, bool endOfLoad, Guid? userId = null)
        {
            ModifyResourceTripleListParams model = null;
            try
            {
                string url = $"{ApiUrl}/resource/modify-triple-list";
                model = new ModifyResourceTripleListParams() { resource_triples = triplesList, resource_id = resourceId, community_short_name = CommunityShortName, publish_home = publishHome, main_image = mainImage, resource_attached_files = resourceAttachedFiles, end_of_load = endOfLoad, user_id = userId };
                WebRequestPostWithJsonObject(url, model);

                Log.Debug("Ended resource triples list modification");
            }
            catch (Exception ex)
            {
                PrepareAttachedToLog(model.resource_attached_files);
                Log.Error($"Error modifying resource triples list. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Method to add / modify / delete triples of multiple complex ontology resources
        /// </summary>
        /// <param name="multipleResourcesTriples">Dictionary with resource identifier guid and the resource triples list to modify. <see cref="ModifyResourceTriple"/></param>
        /// <param name="multipleResourcesAttachedFiles">Dictionary with resource identifier guid and the resource attached files list. <see cref="SemanticAttachedResource"/></param>
        /// <param name="mainImage">main image string</param>
        /// <param name="publishHome">indicates whether the home must be updated</param>
        public void ModifyMultipleResourcesTripleList(Dictionary<Guid, List<ModifyResourceTriple>> multipleResourcesTriples, bool publishHome, string mainImage, Dictionary<Guid, List<SemanticAttachedResource>> multipleResourcesAttachedFiles)
        {
            List<ModifyResourceTripleListParams> model = null;
            try
            {
                string url = $"{ApiUrl}/resource/modify-multiple-resources-triple-list";
                int i = 0;
                int contResources = multipleResourcesTriples.Keys.Count;
                foreach (Guid resourceID in multipleResourcesTriples.Keys)
                {
                    i++;
                    if (model == null)
                    {
                        model = new List<ModifyResourceTripleListParams>();
                    }

                    List<ModifyResourceTriple> triplesList = null;
                    if (multipleResourcesTriples != null && multipleResourcesTriples.ContainsKey(resourceID))
                    {
                        triplesList = multipleResourcesTriples[resourceID];
                    }

                    List<SemanticAttachedResource> attachedList = null;
                    if (multipleResourcesAttachedFiles != null && multipleResourcesAttachedFiles.ContainsKey(resourceID))
                    {
                        attachedList = multipleResourcesAttachedFiles[resourceID];
                    }

                    bool endOfLoad = false;
                    if (i == contResources)
                    {
                        endOfLoad = true;
                    }
                    model.Add(new ModifyResourceTripleListParams() { resource_triples = triplesList, resource_id = resourceID, community_short_name = CommunityShortName, publish_home = publishHome, main_image = mainImage, resource_attached_files = attachedList, end_of_load = endOfLoad });
                }

                WebRequestPostWithJsonObject(url, model);
                Log.Debug("Ended resource triples list modification");
            }
            catch (Exception ex)
            {
                Log.Error($"Error modifying multiple resource triples list. \r\n: Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// FlushDb of resource cache
        /// </summary>
        /// /// <param name="pProyectoID">Proyect ID</param>
        public void DeleteCacheResources(Guid pProyectoID)
        {
            try
            {
                string url = $"{ApiUrl}/resource/delete-cache-resources?project_id={pProyectoID}";

                WebRequest("POST", url, acceptHeader: "application/json");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting cache of resources", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Modify a big list of triples
        /// </summary>
        /// <param name="parameters">Parameters for the modification</param>
        public void MassiveTriplesModify(MassiveTripleModifyParameters parameters)
        {
            try
            {
                string url = $"{ApiUrl}/resource/masive-triple-modify";
                WebRequestPostWithJsonObject(url, parameters);
            }
            catch (Exception ex)
            {
                Log.Error($"Error modifying massive triples. \r\n: Json: {JsonConvert.SerializeObject(parameters)}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Locks a resource for editing for 60 seconds. Just this ResourceApi object can modify the resource from this moment. 
        /// Don't forget to create a try-finally block, making a request to UnlockResource in the finally clause
        /// </summary>
        /// <param name="resourceId">Resource identifier to lock</param>
        /// <param name="secondsTimeout">Timeout to wait a resource lock, in seconds (60 seconds by default)</param>
        /// <param name="secondsLockDuration">Max number of seconds a resource will be locked if isn't unlocked before (60 seconds by default)</param>
        public void LockResource(Guid resourceId, int secondsTimeout = _DEFAULT_TIMEOUT_LOCK, int secondsLockDuration = _DEFAULT_LOCK_DURATION)
        {
            string url = $"{ApiUrl}/resource/lock-document?community_short_name={CommunityShortName}&resource_id={resourceId}&lock_seconds_duration={secondsLockDuration}&timeout_seconds={secondsTimeout}";
            try
            {
                string token = WebRequest("POST", url);
                token = JsonConvert.DeserializeObject<string>(token);
                SetLockTokenForResource(resourceId, token);
            }
            catch (Exception ex)
            {
                Log.Error($"Error, the document {resourceId} can't be locked.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Unlocks a resource previously locked. 
        /// This method must be called from a finally clause, in order to be sure you unlock a locked resource if something goes wrong
        /// </summary>
        /// <param name="resourceId">Resource identifier to unlock</param>
        public void UnlockResource(Guid resourceId)
        {
            string token = GetLockTokenForResource(resourceId);
            string url = $"{ApiUrl}/resource/unlock-document?community_short_name={CommunityShortName}&resource_id={resourceId}&token={token}";
            try
            {
                WebRequest("POST", url);
                SetLockTokenForResource(resourceId, null);
            }
            catch (Exception ex)
            {
                Log.Error($"Error, the document {resourceId} can't be unlocked.", ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Checks if a resource has been previously locked. 
        /// </summary>
        /// <param name="resourceId">Resource identifier to verify</param>
        public bool CheckLockedResource(Guid resourceId)
        {
            string token = GetLockTokenForResource(resourceId);
            string url = $"{ApiUrl}/resource/check-document-is-locked?resource_id={resourceId}";
            try
            {
                return JsonConvert.DeserializeObject<bool>(WebRequest("GET", url));
            }
            catch (Exception ex)
            {
                Log.Error($"Error, we couldn't verify if resource {resourceId} was locked.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the modified resources from a datetime in a community
        /// </summary>
        /// <param name="communityShortName">Community short name</param>
        /// <param name="searchDate">Start search datetime in ISO8601 format string ("yyyy-MM-ddTHH:mm:ss.mmm" (no spaces) OR "yyyy-MM-ddTHH:mm:ss.mmmZ" (no spaces))</param>
        /// <returns>List with the modified resources identifiers</returns>
        public List<Guid> GetModifiedResourcesFromDate(string searchDate)
        {
            List<Guid> resources = null;
            try
            {
                //if (searchDate.Contains(" ") || !searchDate.Contains("T"))
                //{
                //    Log.Error($"The search date string is not in the ISO8601 format {searchDate}");
                //    return null;
                //}

                string url = $"{ApiUrl}/resource/get-modified-resources?community_short_name={CommunityShortName}&search_date={searchDate}";
                string response = WebRequest("GET", url);
                resources = JsonConvert.DeserializeObject<List<Guid>>(response);

                Log.Debug($"Resources obtained of the community {CommunityShortName} from date {searchDate}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the resources of {CommunityShortName} from date {searchDate}", ex.Message);
                throw;
            }
            return resources;
        }

        /// <summary>
        /// Get an attached file from a semantic resource
        /// </summary>
        /// <param name="resource_id">Identifier of the resource</param>
        /// <param name="file_name">Name of the file attached with extension</param>
        /// <param name="language">Only if the property is multilanguage. The language which we want the file. es, en, de, ca, eu, fr, gl, it, pt</param>
        /// <returns>An byte array with the content of the file</returns>
        public byte[] GetAttachedFileFromSemanticResource(Guid resource_id, string file_name, string language = "")
        {
            byte[] attachedFile = null;
            try
            {
                string url = $"{ApiUrl}/resource/get-attached-file-semantic-resource?resource_id={resource_id}&file_name={file_name}&community_short_name={CommunityShortName}&language={language}";
                string response = WebRequest("GET", url);
                attachedFile = JsonConvert.DeserializeObject<byte[]>(response);

            }
            catch(Exception ex)
            {
                Log.Error($"Error getting the file {file_name} from the resource {resource_id} in the community {CommunityShortName}.", ex.Message);
            }
            return attachedFile;
        }

        /// <summary>
        /// Gets the novelties of the resource from a datetime
        /// </summary>
        /// <param name="resourceId">Resource identifier</param>
        /// <param name="searchDate">Start search datetime in ISO8601 format string ("yyyy-MM-ddTHH:mm:ss.mmm" (no spaces) OR "yyyy-MM-ddTHH:mm:ss.mmmZ" (no spaces))</param>
        /// <returns>ResourceNoveltiesModel with the novelties of the resource from the search date</returns>
        public ResourceNoveltiesModel GetResourceNoveltiesFromDate(Guid resourceId, string searchDate)
        {
            ResourceNoveltiesModel resource = null;
            try
            {
                //if(searchDate.Contains(" ") || !searchDate.Contains("T"))
                //{
                //    Log.Error($"The search date string is not in the ISO8601 format {searchDate}");
                //    return null;
                //}

                string url = $"{ApiUrl}/resource/get-resource-novelties?resource_id={resourceId}&community_short_name={CommunityShortName}&search_date={searchDate}";
                string response = WebRequest($"GET", url, acceptHeader: "application/x-www-form-urlencoded");
                resource = JsonConvert.DeserializeObject<ResourceNoveltiesModel>(response);

                if (resource != null)
                {
                    Log.Debug($"Obtained the resource {resourceId} of the community {CommunityShortName} from date {searchDate}");
                }
                else
                {
                    Log.Debug($"The resource {resourceId} could not be obtained of the community {CommunityShortName} from date {searchDate}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting the resource {resourceId} of the community {CommunityShortName} from date {searchDate}", ex.Message);
                throw;
            }
            return resource;
        }

        #region Oauth methods

        /// <summary>
        /// Changes the current ontology by the indicated ontology.
        /// </summary>
        /// <param name="newOntology">New ontology name</param>
        [Obsolete("Se recomienda usar el nuevo metodo ChangeOntology")]
        public void ChangeOntoly(string newOntology)
        {
            string ontologia = newOntology.ToLower().Replace(".owl", "");

            OntologyUrl = null;

            if (!string.IsNullOrEmpty(ontologia))
            {
                OntologyUrl = $"{GraphsUrl}Ontologia/{ontologia}.owl";
            }

        }

        /// <summary>
        /// Changes the current ontology by the indicated ontology.
        /// </summary>
        /// <param name="newOntology">New ontology name</param>
        public void ChangeOntology(string newOntology)
        {
            string ontologia = newOntology.ToLower().Replace(".owl", "");

            OntologyUrl = null;

            if (!string.IsNullOrEmpty(ontologia))
            {
                OntologyUrl = $"{GraphsUrl}Ontologia/{ontologia}.owl";
            }

        }

        /// <summary>
        /// Check if a load identifier is already registered
        /// </summary>
        /// <param name="communityID">Identifier of the community</param>
        /// <param name="organizationID">Identifier of the organization</param>
        /// <returns>True if the load identifier is already registered</returns>
        public bool RefreshHeavyCache(Guid communityID, Guid organizationID)
        {
            try
            {
                string url = $"{ApiUrl}/community/refresh-heavy-cache?community_id={communityID}&organization_id={organizationID}";

                WebRequestPostWithJsonObject(url, "");

                Log.Trace($"community {communityID}. Organization: {organizationID}");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        private void GetGraphsUrl()
        {
            try
            {
                string url = $"{ApiUrl}/ontology/get-graphs-url";

                GraphsUrl = WebRequest($"GET", url, acceptHeader: "application/json")?.Trim('"');

                Log.Trace($"The url of the graphs is: {GraphsUrl}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error obtaining the intragnoss URL: {ex.Message}");
                throw;
            }
        }

        #endregion

        #endregion

        #region Override

        /// <summary>
        /// Load the basic parameters for the API
        /// </summary>
        protected override void LoadApi()
        {
            GetGraphsUrl();

            if (!string.IsNullOrEmpty(OntologyName))
            {
                if (!OntologyName.Contains(".owl"))
                {
                    _ontologyUrl = $"{GraphsUrl}Ontologia/{OntologyName}.owl";
                }
                else
                {
                    _ontologyUrl = $"{GraphsUrl}Ontologia/{OntologyName}";
                }
            }
        }

        /// <summary>
        /// Read the configuration from a configuration file
        /// </summary>
        /// <param name="docXml">XmlDocument with the configuration</param>
        protected override void ReadConfigFile(XmlDocument docXml)
        {
            XmlNode nodoOntologia = docXml.SelectSingleNode("config/ontologyName");
            if (nodoOntologia != null)
            {
                OntologyName = nodoOntologia.InnerText;
            }

            base.ReadConfigFile(docXml);
        }

        /// <summary>
        /// Read the configuration from the environment variables
        /// </summary>
        protected override void LoadEnvironmentVariables()
        {
            if (Environment.GetEnvironmentVariable("ontologyName") != null)
            {
                OntologyName = Environment.GetEnvironmentVariable("ontologyName");
            }
            else
            {
                throw new GnossAPIException("The environment variables doesn't contains ontologyName");
            }

            base.LoadEnvironmentVariables();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default url for graphs
        /// </summary>
        public string GraphsUrl
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the ontology url
        /// </summary>
        public string OntologyUrl
        {
            get
            {
                return _ontologyUrl;
            }
            set
            {
                Log.Trace(value);
                _ontologyUrl = value;
            }
        }

        /// <summary>
        /// Gets the ontology name without extension
        /// </summary>
        public string OntologyNameWithoutExtension
        {
            get
            {
                _ontologyNameWithoutExtension = OntologyUrl.Substring(OntologyUrl.LastIndexOf("/") + 1).Replace(".owl", "");
                return _ontologyNameWithoutExtension;
            }
        }

        /// <summary>
        /// Gets the ontology name with extension
        /// </summary>
        public string OntologyNameWithExtension
        {
            get
            {
                _ontologyNameWithoutExtension = OntologyUrl.Substring(OntologyUrl.LastIndexOf("/") + 1);
                return _ontologyNameWithoutExtension;
            }
        }

        /// <summary>
        /// Gets the wrapper for community API
        /// </summary>
        public CommunityApi CommunityApiWrapper
        {
            get
            {
                if (_communityApi == null)
                {
                    _communityApi = new CommunityApi(OAuthInstance, Log);
                }
                return _communityApi;
            }
        }

        /// <summary>
        /// Developer's email, who will be informed of any problem during a large load of resources. 
        /// </summary>
        private string DeveloperEmail { get; set; }

        /// <summary>
        /// Gets or sets the ontology name
        /// </summary>
        private string OntologyName { get; set; }

        #endregion
    }
}