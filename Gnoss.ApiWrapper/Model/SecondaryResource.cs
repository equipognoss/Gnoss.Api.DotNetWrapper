using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a secondary resource
    /// </summary>
    public class SecondaryResource
    {
        #region Members

        private byte[] _rdfFile;
        private string _stringRdfFile;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the secondary entity identifier
        /// </summary>
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the secondary ontology
        /// </summary>
        public SecondaryOntology SecondaryOntology
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the rdf file
        /// </summary>
        public byte[] RdfFile
        {
            get
            {
                _rdfFile = SecondaryOntology.GenerateRDF();
                return _rdfFile;
            }
            set
            {
                _rdfFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the rdf file as string
        /// </summary>
        public string StringRdfFile
        {
            get
            {
                _rdfFile = SecondaryOntology.GenerateRDF();
                StreamReader sr = new StreamReader(new MemoryStream(_rdfFile));
                _stringRdfFile = sr.ReadToEnd();
                return _stringRdfFile;
            }
        }

        /// <summary>
        /// Gets or sets if this resource has been uploaded correctly
        /// </summary>
        public bool Uploaded
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
        #endregion
    }
}
