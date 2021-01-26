using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a secondary entity
    /// </summary>
    public class SecondaryEntity : OntologyEntity
    {

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="SecondaryEntity"/>
        /// </summary>
        /// <param name="rdfType">rdf:type of the secondary entity</param>
        /// <param name="rdfsLabel">rdfs:label of the secondary entity</param>
        /// <param name="label">Predicate that contains a reference to the auxiliary entity</param>
        /// <param name="identifier">The identifier of the secondary entity. If the identifier is an URI, the identifier would be the last part of the URI after the last slash (/)</param>
        public SecondaryEntity(string rdfType, string rdfsLabel, string label, string identifier) : base(rdfType, rdfsLabel, label)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Constructor of <see cref="SecondaryEntity"/>
        /// </summary>
        /// <param name="rdfType">rdf:type of the secondary entity</param>
        /// <param name="rdfsLabel">rdfs:label of the secondary entity</param>
        /// <param name="label">Predicate that contains a reference to the auxiliary entity</param>
        /// <param name="identifier">The identifier of the secondary entity. If the identifier is an URI, the identifier would be the last part of the URI after the last slash (/)</param>
        /// <param name="properties">Properties of the secondary entity</param>
        public SecondaryEntity(string rdfType, string rdfsLabel, string label, string identifier, List<OntologyProperty> properties) : base(rdfType, rdfsLabel, label, properties)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Empty constructor, only for ConceptEntity use
        /// </summary>
        protected SecondaryEntity() : base("", "", "") { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the secondary entity
        /// <remarks>If the identifier is an URI, the identifier would be the last part of the URI after the last slash (/)</remarks>
        /// </summary>
        public virtual string Identifier
        {
            get; set;
        }

        #endregion
    }
}
