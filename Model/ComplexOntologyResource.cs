using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;
using Gnoss.ApiWrapper.ApiModel;
using SixLabors.ImageSharp;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a resource based on a complex ontology
    /// </summary>
    public class ComplexOntologyResource : BaseResource
    {

        #region Members

        private byte[] _rdfFile;
        private string _stringRdfFile;
        private Ontology _ontology;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the GNOSS Identifier
        /// </summary>
        public override string GnossId
        {
            get
            {
                return base.GnossId;
            }
            set
            {
                base.GnossId = value;
            }
        }

        /// <summary>
        /// Gets or sets the rdf file
        /// </summary>
        public byte[] RdfFile
        {
            get
            {
                if (_rdfFile == null || _rdfFile.Length == 0)
                {
                    _rdfFile = this._ontology.GenerateRDF();
                }
                return _rdfFile;
            }
            set
            {
                _rdfFile = value;
            }
        }

        /// <summary>
        /// Gets the rdf file as string
        /// </summary>
        public string StringRdfFile
        {
            get
            {
                if (_rdfFile == null || _rdfFile.Length == 0)
                {
                    _rdfFile = this._ontology.GenerateRDF();
                }
                StreamReader sr = new StreamReader(new MemoryStream(_rdfFile));
                _stringRdfFile = sr.ReadToEnd();
                return _stringRdfFile;
            }
        }

        /// <summary>
        /// Gets or sets the publisher email
        /// </summary>
        public string PublisherEmail
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the main image
        /// </summary>
        public string MainImage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the ontolgy which the resource is based on
        /// </summary>
        public Ontology Ontology
        {
            get { return _ontology; }
            set
            {
                _ontology = value;
                _rdfFile = _ontology.GenerateRDF();
                ShortGnossId = _ontology.ResourceId;
                GnossId = _ontology.Identifier;
            }
        }

        #region Attached files

        /// <summary>
        /// Gets the attached files name
        /// </summary>
        /// <remarks>
        /// To add an attached file, use the AttachFile or AttachImage methods
        /// </remarks>
        public List<string> AttachedFilesName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the attached files types. The possible types are listed in: <see cref="AttachedResourceFilePropertyTypes">AttachedResourceFilePropertyTypes</see> class
        /// </summary>
        /// <remarks>
        /// To add an attached file, use the AttachFile or AttachImage methods
        /// </remarks>
        public List<AttachedResourceFilePropertyTypes> AttachedFilesType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the attached files. The possible types are listed in: <see cref="AttachedResourceFilePropertyTypes">AttachedResourceFilePropertyTypes</see> class
        /// </summary>
        /// <remarks>
        /// To add an attached file, use the AttachFile or AttachImage methods
        /// </remarks>
        public List<byte[]> AttachedFiles
        {
            get;
            private set;
        }

        #endregion

        #region Screenshots

        /// <summary>
        /// Gets if the screen shot of the resource must be generated
        /// </summary>
        /// <remarks>To set this property to true, use the <see cref="GenerateScreenshot">GenerateScreenshot</see> method</remarks>
        public bool MustGenerateScreenshot
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the screenshot url
        /// </summary>
        /// <remarks>To set this property, use the <see cref="GenerateScreenshot">GenerateScreenshot</see> method</remarks>
        public string ScreenshotUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the predicate which will have a reference to the screenshot
        /// </summary>
        /// <remarks>To set this property, use the <see cref="GenerateScreenshot">GenerateScreenshot</see> method</remarks>
        public string ScreenshotPredicate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the screenshot sizes. It will generate as many screenshots as sizes in this array
        /// </summary>
        /// <remarks>To set this property, use the <see cref="GenerateScreenshot">GenerateScreenshot</see> method</remarks>
        public int[] ScreenshotSizes
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ComplexOntologyResource()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor of <see cref="ComplexOntologyResource"/>, asigning only the GnossId property
        /// </summary>
        /// <param name="largeGnossId">Gnoss identifier used as subject in the ontology graph</param>
        public ComplexOntologyResource(string largeGnossId)
        {
            Initialize();
            if (largeGnossId != null)
            {
                GnossId = largeGnossId;
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "largeGnossId");
            }
        }

        /// <summary>
        /// Constructor of <see cref="ComplexOntologyResource"/>, asigning only the ShortGnossId property
        /// </summary>
        /// <param name="shortGnossId">Internal GNOSS identifier used as subject in the shearch graph</param>
        public ComplexOntologyResource(Guid shortGnossId)
        {

            Initialize();
            ShortGnossId = shortGnossId;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Prepares the screenshot of a url with the specified sizes. The screenshot will be asigned to imagePredicate property
        /// </summary>
        /// <param name="screenshotUrl">Url to generate the screenshot</param>
        /// <param name="imagePredicate">Predicate where the screenshot url will be saved</param>
        /// <param name="screenshotSizes">Screenshot sizes. It will generate as many screenshots as sizes in this array</param>
        /// <example>Generate a screenshot of 'http://www.gnoss.com/home' with sizes 240 y 435 in the predicate 'blog:image'
        ///     <code>
        ///         ComplexOntologyResource resource = new ComplexOntologyResource();
        ///         resource.GenerateScreenshot("http://www.gnoss.com/home", "http://ontology.gnoss.com/blogontology.owl/2013-11#image", new int[] { 240, 435 });
        ///     </code>
        /// </example>
        public void GenerateScreenshot(string screenshotUrl, string imagePredicate, int[] screenshotSizes)
        {
            MustGenerateScreenshot = true;
            ScreenshotUrl = screenshotUrl;
            ScreenshotPredicate = imagePredicate;
            ScreenshotSizes = screenshotSizes;

        }

        /// <summary>
        /// Gets a <see cref="ComplexOntologyResource"/> as a string
        /// </summary>
        /// <returns>String with the information of this <see cref="ComplexOntologyResource"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-------------------------------------------------------------");
            sb.AppendLine("Resource: ");
            sb.AppendLine($"\t\tId: {Id}");
            sb.AppendLine($"\t\tGnossId: {GnossId}");
            sb.AppendLine($"\t\tDescription: {Description}");
            sb.AppendLine($"\t\tTags: {Tags.ToString()}");
            sb.AppendLine($"\t\tTextCategories: {TextCategories.ToString()}");
            sb.AppendLine($"\t\tAuthor: {Author}");
            sb.AppendLine("-------------------------------------------------------------");

            return sb.ToString();
        }

        /// <summary>
        /// Attach a file (not an image, to attach an image use AttachImage method) to the resource. 
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="filePredicate">Predicate of the ontological property where the file reference will be inserted</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the file</param>
        /// <param name="fileIdentifier">(Optional) Unique identifier of the file. Only neccesary if there is more than one file with the same name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachFile(string downloadUrl, string filePredicate, OntologyEntity entity = null, string fileIdentifier = null, string language = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                byte[] file = ReadFile(downloadUrl);
                string fileName = string.Empty;
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    fileName = ((!string.IsNullOrEmpty(fileIdentifier)) ? $"{fileIdentifier}_" : "") + downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        if (downloadUrl.Contains("/"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                        }
                        else if (downloadUrl.Contains("\\"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                        }
                    }
                    else
                    {
                        throw new GnossAPIException($"The file: {downloadUrl} doesn't exist or is inaccessible");
                    }
                }

                AttachFileInternal(file, filePredicate, fileName, AttachedResourceFilePropertyTypes.file, entity, false, language);
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "downloadUrl");
            }
        }

        /// <summary>
        /// Attach a file (not an image, to attach an image use AttachImage method) to the resource. 
        /// </summary>
        /// <param name="file">The file bytes</param>
        /// <param name="filePredicate">Predicate of the ontological property where the file reference will be inserted</param>
        /// <param name="fileName">The file name</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the file</param>
        public void AttachFile(byte[] file, string filePredicate, string fileName, OntologyEntity entity = null)
        {
            AttachFileInternal(file, filePredicate, fileName, AttachedResourceFilePropertyTypes.file, entity);
        }

        /// <summary>
        /// Attach a file (not an image, to attach an image use AttachImage method) to the resource that can be accessed from the Web. It means that the file won't be encripted in the server
        /// </summary>
        /// <param name="file">Archivo a adjuntar (no imagen).</param>
        /// <param name="filePredicate">Predicate of the ontological property where the file reference will be inserted</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the file</param>
        /// <param name="fileName">The file name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachDownloadableFile(byte[] file, string filePredicate, string fileName, OntologyEntity entity = null, string language = null)
        {
            AttachFileInternal(file, filePredicate, fileName, AttachedResourceFilePropertyTypes.downloadableFile, entity, false, language);
        }

        /// <summary>
        /// Attach a reference to a file (not an image, to attach an image use AttachImageWithoutReference method) previusly uploaded using the <see cref="AttachFileWithoutReference"/> method 
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="filePredicate">Predicate of the ontological property where the file reference will be inserted</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the file</param>
        /// <param name="fileIdentifier">(Optional) Unique identifier of the file. Only neccesary if there is more than one file with the same name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachReferenceToFile(string downloadUrl, string filePredicate, OntologyEntity entity = null, string fileIdentifier = null, string language = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                string fileName = string.Empty;
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    fileName = ((!string.IsNullOrEmpty(fileIdentifier)) ? $"{fileIdentifier}_" : "") + downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        if (downloadUrl.Contains("/"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                        }
                        else if (downloadUrl.Contains("\\"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                        }
                    }
                    else
                    {
                        throw new GnossAPIException($"The file: {downloadUrl} doesn't exist or is inaccessible");
                    }
                }

                if (language != null)
                {
                    fileName = $"{fileName}@{language}";
                }

                AttachFileInternal(null, filePredicate, fileName, AttachedResourceFilePropertyTypes.file, entity, true);
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "downloadUrl");
            }
        }

        /// <summary>
        /// Attach a reference to a file (not an image, to attach an image use AttachImageWithoutReference method) previusly uploaded using the <see cref="AttachFileWithoutReference"/> method 
        /// A downloadable file it's a file that can be accessed from the Web. It means that the file won't be encripted in the server
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="filePredicate">Predicate of the ontological property where the file reference will be inserted</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the file</param>
        /// <param name="fileIdentifier">(Optional) Unique identifier of the file. Only neccesary if there is more than one file with the same name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachReferenceToDownloadableFile(string downloadUrl, string filePredicate, OntologyEntity entity = null, string fileIdentifier = null, string language = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                string fileName = string.Empty;
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    fileName = ((!string.IsNullOrEmpty(fileIdentifier)) ? $"{fileIdentifier}_" : "") + downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        if (downloadUrl.Contains("/"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                        }
                        else if (downloadUrl.Contains("\\"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                        }
                    }
                    else
                    {
                        throw new GnossAPIException($"The file: {downloadUrl} doesn't exist or is inaccessible");
                    }
                }

                if (language != null)
                {
                    fileName = $"{fileName}@{language}";
                }
                AttachFileInternal(null, filePredicate, fileName, AttachedResourceFilePropertyTypes.downloadableFile, entity, true);
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "downloadUrl");
            }
        }

        /// <summary>
        /// Uploads a file to the server, but it's not referenced by the resource
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="fileIdentifier">(Optional) Unique identifier of the file. Only neccesary if there is more than one file with the same name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachFileWithoutReference(string downloadUrl, string fileIdentifier = null, string language = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                byte[] file = ReadFile(downloadUrl);
                string fileName = string.Empty;
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    fileName = ((!string.IsNullOrEmpty(fileIdentifier)) ? $"{fileIdentifier}_" : "") + downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        if (downloadUrl.Contains("/"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                        }
                        else if (downloadUrl.Contains("\\"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                        }
                    }
                    else
                    {
                        throw new GnossAPIException($"The file: {downloadUrl} doesn't exist or is inaccessible");
                    }
                }

                if (language != null)
                {
                    fileName = $"{fileName}@{language}";
                }
                AttachFileInternal(file, null, fileName, AttachedResourceFilePropertyTypes.file, null);
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "downloadUrl");
            }
        }

        /// <summary>
        /// Uploads a file to the server, but it's not referenced by the resource. 
        /// A downloadable file it's a file that can be accessed from the Web. It means that the file won't be encripted in the server
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="fileIdentifier">(Optional) Unique identifier of the file. Only neccesary if there is more than one file with the same name</param>
        /// <param name="language">(Optional) The file language</param>
        public void AttachDownloadableFileWithoutReference(string downloadUrl, string fileIdentifier = null, string language = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                byte[] file = ReadFile(downloadUrl);
                string fileName = string.Empty;
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    fileName = ((!string.IsNullOrEmpty(fileIdentifier)) ? $"{fileIdentifier}_" : "") + downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        if (downloadUrl.Contains("/"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                        }
                        else if (downloadUrl.Contains("\\"))
                        {
                            fileName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                        }
                    }
                    else
                    {
                        throw new GnossAPIException($"The file: {downloadUrl} doesn't exist or is inaccessible");
                    }
                }

                if (language != null)
                {
                    fileName = $"{fileName}@{language}";
                }
                AttachFileInternal(file, null, fileName, AttachedResourceFilePropertyTypes.downloadableFile, null);
            }
            else
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "downloadUrl");
            }
        }


        /// <summary>
        /// Attach an image to the resource. It generates as many images as actions contains the actions parameter
        /// </summary>
        /// <param name="downloadUrl">Download url of the image. It can be a local path or a internet url</param>
        /// <param name="actions">List of transformations to do over the image</param>
        /// <param name="predicate">Predicate of the ontological property where the image reference will be inserted</param>
        /// <param name="mainImage">True if this image is the resource main image</param>
        /// <param name="extension">Image extension</param>
        /// <param name="saveOriginalImage">True if the original file must be saved</param>
        /// <param name="saveMaxSizedImage">True if the max sized image must be saved</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the image</param>
        /// <remarks>If a main image has lower size than the minimum permited (240px), acions must be null</remarks>
        /// <returns>True if the image has been attached successfully</returns>
        public bool AttachImage(string downloadUrl, List<ImageAction> actions, string predicate, bool mainImage, string extension, OntologyEntity entity = null, bool saveOriginalImage = true, bool saveMaxSizedImage = false)
        {
            Image imagenOriginal = ImageHelper.ReadImageFromUrlOrLocalPath(downloadUrl);
            bool imagenAdjuntada = false;
            if (imagenOriginal != null)
            {
                imagenAdjuntada = AttachImageInternal(ImageHelper.ImageToByteArray(imagenOriginal), actions, predicate, mainImage, Guid.Empty, false, entity, extension, saveOriginalImage, saveMaxSizedImage);
            }
            return imagenAdjuntada;
        }

        /// <summary>
        /// Attach an image to the resource previusly uploaded using the AttachImageWithoutReference method 
        /// </summary>
        /// <param name="actions">List of transformations to do over the image</param>
        /// <param name="predicate">Predicate of the ontological property where the image reference will be inserted</param>
        /// <param name="mainImage">True if this image is the resource main image</param>
        /// <param name="entity">(Optional) Auxiliary entity which would have the reference to the image</param>
        /// <param name="imageId">Image identifier. If it's Guid.Empty, it would be automatically generated</param>
        /// <param name="extension">Image extension</param>
        /// <remarks>If a main image has lower size than the minimum permited (240px), acions must be null</remarks>
        /// <returns>True if the image reference has been attached successfully</returns>
        public bool AttachReferenceToImage(List<ImageAction> actions, string predicate, bool mainImage, Guid imageId, string extension, OntologyEntity entity = null)
        {
            return AttachImageInternal(null, actions, predicate, mainImage, imageId, true, entity, extension, false, false, false);
        }

        /// <summary>
        /// Uploads an image to the server, but it's not referenced by the resource. It generates as many images as actions contains the actions parameter
        /// </summary>
        /// <param name="downloadUrl">Download Url. It can be a local path or a internet url</param>
        /// <param name="actions">List of transformations to do over the image</param>
        /// <param name="mainImage">True if this image is the resource main image</param>
        /// <param name="imageId">Image identifier. If it's Guid.Empty, it would be automatically generated</param>
        /// <param name="extension">Image extension</param>
        /// <param name="saveOriginalImage">True if the original file must be saved</param>
        /// <param name="saveMaxSizedImage">True if the max sized image must be saved</param>
        /// <param name="saveMainImage">True if the main image must be saved</param>
        /// <returns>True if the image has been uploaded successfully</returns>
        public bool AttachImageWithoutReference(string downloadUrl, List<ImageAction> actions, bool mainImage, Guid imageId, string extension, bool saveOriginalImage = true, bool saveMaxSizedImage = false, bool saveMainImage = true)
        {
            Image originalImage = ImageHelper.ReadImageFromUrlOrLocalPath(downloadUrl);

            bool success = false;
            if (originalImage != null)
            {
                try
                {
                    success = AttachImageInternal(ImageHelper.ImageToByteArray(originalImage), actions, string.Empty, mainImage, imageId, false, null, extension, saveOriginalImage, saveMaxSizedImage, saveMainImage);
                    originalImage.Dispose();
                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"Error attaching image {downloadUrl}: {ex.Message}");

                }
            }
            return success;
        }

        /// <summary>
        /// Uploads an image to the server, but it's not referenced by the resource. It generates as many images as actions contains the actions parameter
        /// </summary>
        /// <param name="originalImage">Image to upload</param>
        /// <param name="actions">List of transformations to do over the image</param>
        /// <param name="mainImage">True if this image is the resource main image</param>
        /// <param name="imageId">Image identifier. If it's Guid.Empty, it would be automatically generated</param>
        /// <param name="extension">Image extension</param>
        /// <param name="saveOriginalImage">True if the original file must be saved</param>
        /// <param name="saveMaxSizedImage">True if the max sized image must be saved</param>
        /// <returns>True if the image has been uploaded successfully</returns>
        public bool AttachImageWithoutReference(byte[] originalImage, List<ImageAction> actions, bool mainImage, Guid imageId, string extension, bool saveOriginalImage = true, bool saveMaxSizedImage = false)
        {
            bool imagenAdjuntada = false;
            if (originalImage != null)
            {
                imagenAdjuntada = AttachImageInternal(originalImage, actions, string.Empty, mainImage, imageId, false, null, extension, saveOriginalImage, saveMaxSizedImage);
            }

            return imagenAdjuntada;
        }

        /// <summary>
        /// Attach an image to the resource. It generates as many images as actions contains the actions parameter
        /// </summary>
        /// <param name="originalImage">Image to upload,  null if it would be only a reference</param>
        /// <param name="actions">List of <see cref="ImageAction"/></param>
        /// <param name="predicate">Ontology predicate</param>
        /// <param name="mainImage">True if the image is the resource main image</param>
        /// <param name="extension">Image extension</param>
        /// <param name="entity">(Optional) The auxiliary entity that owns the image property</param>
        /// <param name="saveOriginalImage">True if the original file must be saved</param>
        /// <param name="saveMaxSizedImage">True if the max sized image must be saved</param>
        /// <remarks>If a main image has lower size than the minimum permited (240px), acions must be null</remarks>
        /// <returns>True if the image reference has been attached successfully</returns>
        public bool AttachImage(byte[] originalImage, List<ImageAction> actions, string predicate, bool mainImage, string extension, OntologyEntity entity = null, bool saveOriginalImage = true, bool saveMaxSizedImage = false)
        {
            bool success = false;
            if (originalImage != null)
            {
                success = AttachImageInternal(originalImage, actions, predicate, mainImage, Guid.Empty, false, entity, extension, saveOriginalImage, saveMaxSizedImage);
            }
            return success;
        }

        #endregion

        #region Private methods

        private void Initialize()
        {
            Author = null;
            MustGenerateScreenshot = false;
            ScreenshotUrl = string.Empty;
            ScreenshotSizes = null;
            ScreenshotPredicate = string.Empty;
            AttachedFilesName = new List<string>();
            AttachedFilesType = new List<AttachedResourceFilePropertyTypes>();
            AttachedFiles = new List<byte[]>();
            _ontology = null;
            AutomaticTagsTextFromTitle = string.Empty;
            AutomaticTagsTextFromDescription = string.Empty;
            Description = string.Empty;
            PublishInHome = false;
            Uploaded = false;
            PublisherEmail = string.Empty;
        }


        /// <summary>
        /// Attach a file
        /// </summary>
        /// <param name="file">File to attach</param>
        /// <param name="filePredicate">Predicate of the ontological property</param>
        /// <param name="fileName">File name</param>
        /// <param name="fileType">File type</param>
        /// <param name="onlyReference">True if the file mustn't be saved, only as a reference</param>
        /// <param name="entity">(Optional) The auxiliary entity that owns the file property</param>
        /// <param name="language">language of the file</param>
        private void AttachFileInternal(byte[] file, string filePredicate, string fileName, AttachedResourceFilePropertyTypes fileType, OntologyEntity entity = null, bool onlyReference = false, string language = null)
        {
            if (file != null)
            {
                if (onlyReference == false)
                {
                    AttachedFiles.Add(file);

                    string temporalFileName = fileName;
                    if (!string.IsNullOrEmpty(language))
                    {
                        temporalFileName += $"@{language}";
                    }

                    AttachedFilesName.Add(temporalFileName);

                    if (fileType == AttachedResourceFilePropertyTypes.file)
                    {
                        AttachedFilesType.Add(AttachedResourceFilePropertyTypes.file);
                    }
                    else
                    {
                        AttachedFilesType.Add(AttachedResourceFilePropertyTypes.downloadableFile);
                    }
                }
            }

            if (entity != null)
            {
                if (Ontology.Entities == null)
                {
                    Ontology.Entities = new List<OntologyEntity>();
                    Ontology.Entities.Add(entity);
                }
                if (AttachedFilesType.Count > 0 && AttachedFiles.Count > 0 && AttachedFilesType.Count > 0)
                {
                    entity.Properties.Add(new StringOntologyProperty(filePredicate, fileName, language));
                }
            }
            else
            {
                if (filePredicate != null)
                {
                    if (Ontology.Properties == null)
                    {
                        Ontology.Properties = new List<OntologyProperty>();
                    }
                    Ontology.Properties.Add(new StringOntologyProperty(filePredicate, fileName, language));
                }
            }

            if(Ontology != null)
            {
                _rdfFile = Ontology.GenerateRDF();
            }
        }

        /// <summary>
        /// Attach an image
        /// </summary>
        /// <param name="originalImage">Image to upload,  null if it would be only a reference</param>
        /// <param name="actions">List of <see cref="ImageAction"/></param>
        /// <param name="predicate">Ontology predicate</param>
        /// <param name="mainImage">True if the image is the resource main image</param>
        /// <param name="imageId">Image identifier. If it's Guid.Empty, it would be automatically generated</param>
        /// <param name="onlyReference">True if the image mustn't be saved, only as a reference</param>
        /// <param name="entity">(Optional) The auxiliary entity that owns the image property</param>
        /// <param name="extension">Image extension</param>
        /// <param name="saveOriginalImage">True if the original file must be saved</param>
        /// <param name="saveMaxSizedImage">True if the max sized image must be saved</param>
        /// <param name="saveMainImage">True if the main image must be saved</param>
        /// <returns>True if the image reference has been attached successfully</returns>
        private bool AttachImageInternal(byte[] originalImage, List<ImageAction> actions, string predicate, bool mainImage, Guid imageId, bool onlyReference, OntologyEntity entity, string extension, bool saveOriginalImage = true, bool saveMaxSizedImage = false, bool saveMainImage = true)
        {
            bool attachedImage = false;
            if (extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".gif"))
            {
                if (string.IsNullOrEmpty(extension))
                {
                    extension = ".jpg";
                }
                else
                {
                    if (!extension.StartsWith("."))
                    {
                        extension = "." + extension;
                    }
                }

                if (imageId.Equals(Guid.Empty))
                {
                    imageId = Guid.NewGuid();
                }

                

                try
                {
                    List<string> errorList = new List<string>();

                    if (actions == null && actions.Count == 0)
                    {
                        // Without actions
                        if (mainImage && saveOriginalImage)
                        {
                            // The image to upload has lower size than 240 pixels. The image quality would be 100%
                            MainImage = string.Format("[IMGPrincipal][240,]{0}" + extension, imageId);

                            AttachedFilesName.Add($"{imageId}" + extension);
                            AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);

                            AttachedFilesName.Add($"{imageId}_240" + extension);
                            AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);

                            if (!onlyReference)
                            {
                                AttachedFiles.Add(originalImage);
                                AttachedFiles.Add(originalImage);
                                AttachedFiles.Add(originalImage);
                            }
                            attachedImage = true;
                        }
                        else if (saveOriginalImage)
                        {
                            AttachedFilesName.Add($"{imageId}" + extension);
                            AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);

                            if (!onlyReference)
                            {
                                AttachedFiles.Add(originalImage);
                            }
                            attachedImage = true;
                        }
                        else
                        {
                            throw new GnossAPIException("Actions can be null only when the image is a main image and the original image is set to be saved");
                        }
                    }
                    else
                    {
                        // With actions

                        bool originalImageSaved = false;
                        Image maxSizeImage = null;
                        bool imageModificationError = false;

                        foreach (ImageAction action in actions)
                        {
                            imageModificationError = false;
                            Image resizedImage = null;

                            if (!onlyReference)
                            {
                                // Modify the image
                                switch (action.ImageTransformationType)
                                {
                                    case ImageTransformationType.Crop:
                                        try
                                        {
                                            resizedImage = ImageHelper.ByteArrayToImage(originalImage);
                                            ImageHelper.CropImageToSquare(resizedImage, action.Size);
                                        }
                                        catch (GnossAPIException gaex)
                                        {
                                            imageModificationError = true;
                                            errorList.Add(gaex.Message);
                                        }
                                        break;
                                    case ImageTransformationType.ResizeToHeight:
                                        try
                                        {
                                            resizedImage = ImageHelper.ByteArrayToImage(originalImage);
                                            ImageHelper.ResizeImageToHeight(resizedImage, (int)action.Height);
                                        }
                                        catch (GnossAPIException gaex)
                                        {
                                            imageModificationError = true;
                                            errorList.Add(gaex.Message);
                                        }
                                        break;
                                    case ImageTransformationType.ResizeToWidth:
                                        try
                                        {
                                            resizedImage = ImageHelper.ByteArrayToImage(originalImage);
                                            ImageHelper.ResizeImageToWidth(resizedImage, (int)action.Width);
                                        }
                                        catch (GnossAPIException gaex)
                                        {
                                            imageModificationError = true;
                                            errorList.Add(gaex.Message);
                                        }
                                        break;
                                    case ImageTransformationType.ResizeToHeightAndWidth:
                                        try
                                        {
                                            resizedImage = ImageHelper.ByteArrayToImage(originalImage);
                                            ImageHelper.ResizeImageToHeightAndWidth(resizedImage, (int)action.Width, (int)action.Height);
                                        }
                                        catch (GnossAPIException gaex)
                                        {
                                            imageModificationError = true;
                                            errorList.Add(gaex.Message);
                                        }
                                        break;
                                    case ImageTransformationType.CropToHeightAndWidth:
                                        try
                                        {
                                            resizedImage = ImageHelper.ByteArrayToImage(originalImage);
                                            ImageHelper.CropImageToHeightAndWidth(resizedImage, (int)action.Height, (int)action.Width);
                                        }
                                        catch (GnossAPIException gaex)
                                        {
                                            imageModificationError = true;
                                            errorList.Add(gaex.Message);
                                        }
                                        break;

                                    default:
                                        throw new GnossAPIException("The ImageTransformationType is not valid");

                                }
                            }

                            if (mainImage && imageModificationError)
                            {
                                mainImage = false;
                            }

                            if (!imageModificationError && !onlyReference)
                            {
                                if (mainImage)
                                {
                                    MainImage = string.Format("[IMGPrincipal][{0},]{1}" + extension, action.Size, imageId);
                                    mainImage = false;
                                }

                                AttachedFilesName.Add($"{imageId}_{action.Size}{extension}");
                                AttachedFiles.Add(ImageHelper.ImageToByteArray(resizedImage, (int)action.ImageQualityPercentage));
                                AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);
                                attachedImage = true;

                                if (action.EmbedsRGB)
                                {
                                    ImageHelper.AssignEXIFPropertyColorSpaceSRGB(resizedImage);
                                }

                                // Save original image
                                if (saveOriginalImage && !originalImageSaved)
                                {
                                    if (saveMainImage)
                                    {
                                        AttachedFilesName.Add($"{imageId}" + extension);
                                        AttachedFiles.Add(ImageHelper.ImageToByteArray(ImageHelper.ByteArrayToImage(originalImage), (int)actions.Max(z => z.ImageQualityPercentage)));
                                        AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);
                                    }

                                    originalImageSaved = true;
                                    attachedImage = true;
                                }
                            }
                            else
                            {
                                if (mainImage)
                                {
                                    MainImage = string.Format("[IMGPrincipal][{0},]{1}" + extension, action.Size, imageId);
                                    mainImage = false;
                                }
                                attachedImage = true;

                            }
                        }

                        // Save the image at the max size allowed
                        if (saveMaxSizedImage)
                        {
                            try
                            {
                                // The quality percentage of the max size image is the highest quality percentage  
                                if (ImageHelper.ByteArrayToImage(originalImage).Width > Constants.MAXIMUM_WIDTH_GNOSS_IMAGE)
                                {
                                    maxSizeImage = ImageHelper.ByteArrayToImage(originalImage);
                                    ImageHelper.ResizeImageToWidth(maxSizeImage, Constants.MAXIMUM_WIDTH_GNOSS_IMAGE);

                                    AttachedFilesName.Add($"{imageId}_{Constants.MAXIMUM_WIDTH_GNOSS_IMAGE}" + extension);
                                    AttachedFiles.Add(ImageHelper.ImageToByteArray(maxSizeImage, (int)actions.Max(z => z.ImageQualityPercentage)));
                                    AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);

                                    attachedImage = true;
                                }
                                else
                                {
                                    AttachedFilesName.Add($"{imageId}_{Constants.MAXIMUM_WIDTH_GNOSS_IMAGE}" + extension);
                                    AttachedFiles.Add(ImageHelper.ImageToByteArray(ImageHelper.ByteArrayToImage(originalImage), (int)actions.Max(z => z.ImageQualityPercentage)));
                                    AttachedFilesType.Add(AttachedResourceFilePropertyTypes.image);

                                    attachedImage = true;
                                }
                            }
                            catch (GnossAPIException gaex)
                            {
                                imageModificationError = true;
                                errorList.Add(gaex.Message);
                            }
                        }
                    }

                    if (entity != null && !string.IsNullOrWhiteSpace(predicate) && !string.IsNullOrEmpty(predicate))
                    {
                        // The image is from an auxiliary entity
                        if (Ontology.Entities == null)
                        {
                            Ontology.Entities = new List<OntologyEntity>();
                            Ontology.Entities.Add(entity);
                        }

                        if (!onlyReference)
                        {
                            if (AttachedFiles.Count > 0 && AttachedFilesType.Count > 0)
                            {
                                if (entity.Properties == null)
                                {
                                    entity.Properties = new List<OntologyProperty>();
                                }
                                entity.Properties.Add(new ImageOntologyProperty(predicate, $"{imageId}" + extension));

                            }
                        }
                        else
                        {
                            if (attachedImage)
                            {
                                if (entity.Properties == null)
                                {
                                    entity.Properties = new List<OntologyProperty>();
                                }
                                entity.Properties.Add(new ImageOntologyProperty(predicate, $"{imageId}" + extension));

                            }
                        }
                    }
                    else
                    {
                        // The image is an ontology property
                        if (!string.IsNullOrWhiteSpace(predicate) && !string.IsNullOrEmpty(predicate))
                        {
                            if (Ontology.Properties == null)
                            {
                                Ontology.Properties = new List<OntologyProperty>();
                            }

                            if (!onlyReference)
                            {
                                if (AttachedFilesType.Count > 0 && AttachedFiles.Count > 0)
                                {
                                    ImageOntologyProperty pOntImg = new ImageOntologyProperty(predicate, $"{imageId}" + extension);
                                    Ontology.Properties.Add(pOntImg);
                                }
                            }
                            else
                            {
                                if (attachedImage)
                                {
                                    Ontology.Properties.Add(new ImageOntologyProperty(predicate, $"{imageId}" + extension));
                                }
                            }

                        }
                    }

                    if (errorList != null && errorList.Count > 0)
                    {
                        string message = null;
                        foreach (string error in errorList)
                        {
                            if (message == null)
                            {
                                message = error;
                            }
                            else
                            {
                                message = $"{message}\n{error}";
                            }
                        }
                        throw new GnossAPIException(message);
                    }
                }
                catch (GnossAPIException)
                {
                    throw;
                }
                catch (FileNotFoundException ex)
                {
                    throw new GnossAPIException($"The image: {imageId} doesn't exist or is inaccessible", ex);
                }

                if(Ontology != null)
                {
                    _rdfFile = Ontology.GenerateRDF();
                }                

            }
            else
            {
                throw new Exception($"Los formatos permitidos son png, jpg, jpeg y gif. La extensión recibida es {extension}.");
            }
            
            return attachedImage;
        }

        #endregion
    }
}
