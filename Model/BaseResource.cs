using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Abstract class that represents the basic properties of any resource
    /// </summary>
    public abstract class BaseResource
    {
        #region Members

        private string _gnossId;
        private string _title;
        private string _description;
        private string[] _tags;
        private string _automaticTagsTextFromTitle;
        private string _automaticTagsTextFromDescription;
        private DateTime _creationDate;

        private List<Multilanguage> _multilanguageTitle;
        private List<Multilanguage> _multilanguageDescription;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the resource identifier
        /// </summary>
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if the resource has been modified
        /// </summary>
        public bool Modified
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if the resource has been deleted
        /// </summary>
        public bool Deleted
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the gnoss identifier
        /// </summary>
        public virtual string GnossId
        {
            get
            {
                return _gnossId;
            }
            set
            {
                _gnossId = value;

                try
                {
                    if (_gnossId != null && _gnossId.Contains("_"))
                    {
                        string[] result = _gnossId.Split('_');
                        ShortGnossId = new Guid(result[result.Length - 2]);
                    }
                    else
                    {
                        if (_gnossId != null)
                        {
                            string var = _gnossId.Replace("http://gnoss/", "");
                            ShortGnossId = new Guid(var);
                        }
                        else
                        {
                            ShortGnossId = Guid.Empty;
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw new GnossAPIArgumentException("The GnossId is not valid", "GnossId", ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource title
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                if (!string.IsNullOrEmpty(_title) && !string.IsNullOrWhiteSpace(_title) && _title.Contains("\0"))
                {
                    _title = _title.Replace("\0", "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the multilanguage title
        /// Example: 
        ///  resource.MultilanguageTitle.Add(new MultiIdioma("English title", Gnoss.ApiWrapper.Helpers.Languages.English));
        ///  resource.MultilanguageTitle.Add(new MultiIdioma("French title", Gnoss.ApiWrapper.Helpers.Languages.French));
        ///  resource.MultilanguageTitle.Add(new MultiIdioma("Spanish title", Gnoss.ApiWrapper.Helpers.Languages.Spanish));
        /// </summary>
        public List<Multilanguage> MultilanguageTitle
        {
            get { return _multilanguageTitle; }
            set
            {
                _multilanguageTitle = value;
                StringBuilder title = new StringBuilder();
                foreach (Multilanguage languageTitle in _multilanguageTitle)
                {
                    if (!string.IsNullOrEmpty(languageTitle.String))
                    {
                        title.Append($"{languageTitle.String}@{languageTitle.Language}|||");
                    }
                    else
                    {
                        Title = string.Empty;
                    }
                }
                if (title.Length > 0)
                {
                    Title = title.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                if (!string.IsNullOrEmpty(_description) && !string.IsNullOrWhiteSpace(_description) && _description.Contains("\0"))
                {
                    _description = _description.Replace("\0", "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the multilanguage description
        /// Example: 
        ///  resource.MultilanguageDescription.Add(new MultiIdioma("English description", Gnoss.ApiWrapper.Helpers.Languages.English));
        ///  resource.MultilanguageDescription.Add(new MultiIdioma("French description", Gnoss.ApiWrapper.Helpers.Languages.French));
        ///  resource.MultilanguageDescription.Add(new MultiIdioma("Spanish description", Gnoss.ApiWrapper.Helpers.Languages.Spanish));
        /// </summary>
        public List<Multilanguage> MultilanguageDescription
        {
            get { return _multilanguageDescription; }
            set
            {
                _multilanguageDescription = value;
                StringBuilder description = new StringBuilder();
                foreach (Multilanguage languageDescription in _multilanguageDescription)
                {
                    if (!string.IsNullOrEmpty(languageDescription.String))
                    {
                        description.Append($"{languageDescription.String}@{languageDescription.Language}|||");
                    }
                    else
                    {
                        Description = string.Empty;
                    }
                }
                if (description.Length > 0)
                {
                    Description = description.ToString();
                }
            }
        }
        /// <summary>
        /// Gets or sets the resource tags
        /// </summary>
        public string[] Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                if (value != null)
                {
                    _tags = value.ToList().ConvertAll(tag => tag.ToLower()).ToArray();
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource categories names. It can be in hierarchy (setting the parent to childre all the categories names), or without hierarchy. 
        /// <example>If we want to set the category "Madrid", children of the category "Spain", the category text is: <c>Spain|Madrid</c></example>
        /// </summary>
        public List<string> TextCategories
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the categories identifiers. This list is automatic generated by <see cref="TextCategories">TextCategories property</see> in <see cref="ResourceApi">ResourceApi</see>
        /// </summary>
        public List<Guid> CategoriesIds
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the author of the resource. It there is more than one author, thay must be separated by comma (example: Miguel de Cervantes, William Shakespeare, ...)
        /// </summary>
        public string Author
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the GNOSS Guid resource calculated from <see cref="GnossId">GnossId</see>
        /// </summary>
        public Guid ShortGnossId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if the publisher of the resource is the author of it
        /// </summary>
        public bool CreatorIsAuthor
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the text from which the tags are going to be calculated. This tags are going to be generated by the Gnoss Automatic Tag service from the title if this property is filled. 
        /// </summary>
        public string AutomaticTagsTextFromTitle
        {
            get { return _automaticTagsTextFromTitle; }
            set
            {
                _automaticTagsTextFromTitle = value;
                if (!string.IsNullOrEmpty(_automaticTagsTextFromTitle) && !string.IsNullOrWhiteSpace(_automaticTagsTextFromTitle) && _automaticTagsTextFromTitle.Contains("\0"))
                {
                    _automaticTagsTextFromTitle = _automaticTagsTextFromTitle.Replace("\0", "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description from which the tags are going to be calculated. This tags are going to be generated by the Gnoss Automatic Tag service from the description if this property is filled. 
        /// </summary>
        public string AutomaticTagsTextFromDescription
        {
            get { return _automaticTagsTextFromDescription; }
            set
            {
                _automaticTagsTextFromDescription = value;
                if (!string.IsNullOrEmpty(_automaticTagsTextFromDescription) && !string.IsNullOrWhiteSpace(_automaticTagsTextFromDescription) && _automaticTagsTextFromDescription.Contains("\0"))
                {
                    _automaticTagsTextFromDescription = _automaticTagsTextFromDescription.Replace("\0", "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource visibility.
        /// </summary>
        public ResourceVisibility Visibility
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the resource readers groups.
        /// The editors and editors groups are set by his short name. If there is any organization group, the way to set this group is setting the organization_short_name and group_short_name. 
        /// </summary>
        public List<ReaderEditor> ReadersGroups
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the resource editors groups. 
        /// The value of this property must be tha short names of the groups, separated by commas. 
        /// The readers and readers groups are set by his short name. If there is any organization group, the way to set this group is setting the organization_short_name and group_short_name.  
        /// </summary>
        public List<ReaderEditor> EditorsGroups
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the resource creation date. If it's not set, the value would be the upload date. 
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                if (_creationDate == DateTime.MinValue)
                {
                    _creationDate = DateTime.Now;
                }

                return _creationDate;
            }
            set
            {
                _creationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets if this resource must be publised in tho community home
        /// </summary>
        public bool PublishInHome
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if this resource has been uploaded correctly
        /// </summary>
        public bool Uploaded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the canonical Url of the resource
        /// </summary>
        public string CanonicalUrl
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the AumentedReadig of the resource
        /// </summary>
        public AumentedReading AumentedReading
        {
            get; set;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Read a file from the internet or local file system
        /// </summary>
        /// <param name="downloadUrl">Url to download</param>
        /// <returns>The bytes of the file</returns>
        protected virtual byte[] ReadFile(string downloadUrl)
        {
            byte[] file = null;
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                if (Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute))
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(downloadUrl);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        file = ms.ToArray();
                    }

                    client.Dispose();
                    stream.Flush();
                    stream.Close();
                }
                else
                {
                    if (File.Exists(downloadUrl))
                    {
                        FileStream fs = new FileStream(downloadUrl, FileMode.Open);
                        BinaryReader br = new BinaryReader(fs);
                        file = br.ReadBytes((int)fs.Length);
                        fs.Close();
                        fs.Dispose();
                    }
                    else
                    {
                        throw new GnossAPIException($"The file {downloadUrl} doesn't exist or isn't accessible");
                    }
                }
            }
            else
            {
                throw new GnossAPIArgumentException("downloadUrl can't be null or empty", downloadUrl);
            }

            return file;
        }

        #endregion
    }
}
