using System;
using System.IO;
using Gnoss.ApiWrapper.Helpers;
using SixLabors.ImageSharp;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a resource based in the GNOSS ontology
    /// </summary>
    public class BasicOntologyResource : BaseResource
    {
        #region Constructor

        public BasicOntologyResource()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the properties name
        /// </summary>
        public string PropertiesName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the resource download url 
        /// </summary>
        public string DownloadUrl
        {
            get; set;
        }

        /// <summary>
        /// Gets the file or image to attach to the resource
        /// </summary>
        /// <remarks>
        /// To set this property, use <see cref="AttachImage">AttachImage</see> method or <see cref="AttachFile">AttachFile</see> method
        /// </remarks>
        public byte[] AttachedFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the snapshot sizes
        /// </summary>
        public int[] SnapshotSizes
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if the snapshot must be generated or not
        /// </summary>
        public bool GenerateSnapshot
        {
            get; set;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Read an image from the local file system or the internet and asigns it to the <see cref="AttachedFile"/> property
        /// </summary>
        /// <param name="downloadUrl">Absolute local path or url of the image</param>
        /// <param name="size">Size to resize the image after download it</param>
        public void AttachImage(string downloadUrl, int size)
        {
            byte[] image = ReadFile(downloadUrl);

            Image resizedImage = null;
            GenerateSnapshot = true;

            if (size != 0)
            {
                resizedImage = ImageHelper.ByteArrayToImage(image);
                ImageHelper.ResizeImageToWidth(resizedImage, size);
                AttachedFile = ImageHelper.ImageToByteArray(resizedImage);
            }
        }

        /// <summary>
        /// Read a file (not an image, to attach an image use <see cref="AttachImage"/>) from the local file system or the internet and asigns it to the <see cref="AttachedFile"/> property
        /// </summary>
        /// <param name="downloadUrl">Absolute local path or url of the image</param>
        public void AttachFile(string downloadUrl)
        {
            ReadFile(downloadUrl);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Read a file from the internet or local file system
        /// </summary>
        /// <param name="downloadUrl">Url to download</param>
        /// <returns>The bytes of the file</returns>
        protected override byte[] ReadFile(string downloadUrl)
        {
            AttachedFile = base.ReadFile(downloadUrl);

            if (!Uri.IsWellFormedUriString(downloadUrl, UriKind.Absolute) && File.Exists(downloadUrl))
            {
                if (downloadUrl.Contains("/"))
                {
                    PropertiesName = downloadUrl.Substring(downloadUrl.LastIndexOf("/") + 1);
                }
                else if (downloadUrl.Contains("\\"))
                {
                    PropertiesName = downloadUrl.Substring(downloadUrl.LastIndexOf("\\") + 1);
                }
            }

            return AttachedFile;
        }

        #endregion
    }
}
