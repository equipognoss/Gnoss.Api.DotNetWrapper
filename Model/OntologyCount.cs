using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Counts for each ontology the number of resources and files
    /// </summary>
    public class OntologyCount
    {
        private int resourcesCount;
        private int fileCount;
        /// <summary>
        /// Constructor
        /// </summary>
        public OntologyCount()
        {

        }
        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="resourcesCount">Number of resources in a file</param>
        /// <param name="fileCount">Number of files of a ontology</param>
        public OntologyCount(int resourcesCount, int fileCount)
        {
            this.fileCount = fileCount;
            this.resourcesCount = resourcesCount;
        }
        /// <summary>
        /// Number of resources in a file
        /// </summary>
        public int ResourcesCount
        {
            get { return resourcesCount; }
            set { resourcesCount = value; }
        }
        /// <summary>
        /// Number of files of a ontology
        /// </summary>
        public int FileCount
        {
            get { return fileCount; }
            set { fileCount = value; }
        }
    }
}
