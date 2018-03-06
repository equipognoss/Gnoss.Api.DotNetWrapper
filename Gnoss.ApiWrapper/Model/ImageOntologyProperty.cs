using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a image property from an ontology
    /// </summary>
    public class ImageOntologyProperty : OntologyProperty
    {
        #region Constructors

        /// <summary>
        /// Constructor of <see cref="ImageOntologyProperty"/>.
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="value">Property value</param>
        public ImageOntologyProperty(string label, string value)
        {
            Name = label;
            Value = value;
        }

        #endregion
    }
}
