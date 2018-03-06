using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a boolean property from an ontology
    /// </summary>
    public class BoolOntologyProperty : OntologyProperty
    {

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="BoolOntologyProperty"/>
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="value">Value of the predicate</param>
        public BoolOntologyProperty(string label, bool? value)
        {
            Name = label;
            Value = value;
        }

        #endregion

    }
}
