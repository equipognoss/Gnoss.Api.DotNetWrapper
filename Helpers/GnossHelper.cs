using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnoss.ApiWrapper.OAuth;

namespace Gnoss.ApiWrapper.Helpers
{

    /// <summary>
    /// Helper for GNOSS utilities
    /// </summary>
    public class GnossHelper
    {

        /// <summary>
        /// Gets the relative path of a image 
        /// </summary>
        /// <param name="resourceID">Identifier of the document</param>
        /// <param name="imageName">Name of the image</param>
        /// <returns></returns>
        public static string GetImagePath(Guid resourceID, string imageName)
        {
            return $"{Constants.IMAGES_PATH_ROOT}{resourceID.ToString().Substring(0, 2)}/{resourceID.ToString().Substring(0, 4)}/{resourceID}/{imageName}";
        }

        /// <summary>
        /// Gets the short id of a large id
        /// </summary>
        /// <param name="largeID">Large id of a resource (ej: http://gnoss.com/items/article_591df24d-4161-4fc0-933a-00fa0e91503c_5653cca6-b987-45ae-a33c-911c87aef051</param>
        /// <returns></returns>
        public static Guid GetResourceID(string largeID)
        {
            string[] splittedID = largeID.Split(CharArrayDelimiters.Underscore);
            try
            {
                return new Guid(splittedID[splittedID.Length - 2]);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        
    }
}
