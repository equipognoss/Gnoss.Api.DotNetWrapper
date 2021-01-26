using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a String List property from an ontology
    /// </summary>
    public class ListStringOntologyProperty : OntologyProperty
    {
        /// <summary>
        /// Constructor of <see cref="ListStringOntologyProperty"/>.
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="value">Property value</param>
        public ListStringOntologyProperty(string label, List<string> value)
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
        public ListStringOntologyProperty(string label, List<string> value, string language)
        {
            Name = label;
            Value = value;
            Language = language;
        }

    }
}
