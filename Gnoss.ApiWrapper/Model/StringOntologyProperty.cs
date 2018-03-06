using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents string property from an ontology
    /// </summary>
    public class StringOntologyProperty : OntologyProperty
    {
        /// <summary>
        /// Constructor of <see cref="StringOntologyProperty"/>.
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="value">Property value</param>
        public StringOntologyProperty(string label, string value)
        {
            Name = label;
            Value = value;
        }

        /// <summary>
        /// Constructor of <see cref="ListStringOntologyProperty"/>.
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="value">Property value</param>
        /// <param name="language">Property language</param>
        public StringOntologyProperty(string label, string value, string language)
        {
            Name = label;
            Value = value;
            Language = language;
        }

    }
}
