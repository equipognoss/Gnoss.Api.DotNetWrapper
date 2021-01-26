using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents an auxiliary entity
    /// </summary>
    public class OntologyEntity
    {
        #region Members

        private string _rdfType;
        private string _rdfsLabel;
        private List<OntologyProperty> _properties;

        #endregion        

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="OntologyEntity"/>
        /// </summary>
        /// <param name="rdfType">rdf:type of the auxiliary entity</param>
        /// <param name="rdfsLabel">rdfs:label of the auxiliary entity</param>
        /// <param name="label">Predicate that contains a reference to the auxiliary entity</param>
        public OntologyEntity(string rdfType, string rdfsLabel, string label)
        {
            if (string.IsNullOrEmpty(rdfType) || string.IsNullOrWhiteSpace(rdfType))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "rdfType");
            }
            else if (string.IsNullOrEmpty(rdfsLabel) || string.IsNullOrWhiteSpace(rdfsLabel))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "rdfsLabel");
            }
            else
            {
                RdfType = rdfType;
                RdfsLabel = rdfsLabel;
                Label = label;
                _properties = new List<OntologyProperty>();
                Entities = new List<OntologyEntity>();
                GuidEntidad = Guid.NewGuid();
            }
        }

        /// <summary>
        /// Constructor of <see cref="OntologyEntity"/>
        /// </summary>
        /// <param name="rdfType">rdf:type of the auxiliary entity</param>
        /// <param name="rdfsLabel">rdfs:label of the auxiliary entity</param>
        /// <param name="label">Predicate that contains a reference to the auxiliary entity</param>
        /// <param name="properties">Properties of the entity</param>
        /// <param name="entities">(Optional) Auxiliary entity list of the entity</param>
        public OntologyEntity(string rdfType, string rdfsLabel, string label, List<OntologyProperty> properties, List<OntologyEntity> entities = null)
        {
            if (string.IsNullOrEmpty(rdfType) || string.IsNullOrWhiteSpace(rdfType))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "rdfType");
            }
            else if (string.IsNullOrEmpty(rdfsLabel) || string.IsNullOrWhiteSpace(rdfsLabel))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "rdfsLabel");
            }
            else
            {
                RdfType = rdfType;
                RdfsLabel = rdfsLabel;
                Label = label;
                Properties = properties;
                Entities = entities;
                GuidEntidad = Guid.NewGuid();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Validate if the properties of the entity has any property with no empty data
        /// </summary>
        /// <returns><c>true</c> if the entity has any property with no empty data or <c>false</c> if the entity has eny property with empty or null data</returns>
        public bool HasAnyPropertyWithData()
        {
            bool hasProperties = false;
            foreach (OntologyProperty prop in Properties)
            {
                if (prop.GetType() == DataTypes.OntologyPropertyString)
                {
                    if (prop.Value != null && !string.IsNullOrEmpty(prop.Value.ToString()) && !string.IsNullOrWhiteSpace(prop.Value.ToString()))
                    {
                        hasProperties = true;
                        break;
                    }
                }
                else if (prop.GetType() == DataTypes.OntologyPropertyListString)
                {
                    if (prop.Value != null)
                    {
                        foreach (string valor in prop.Value as List<string>)
                        {
                            if (!string.IsNullOrEmpty(valor) && !string.IsNullOrWhiteSpace(valor))
                            {
                                hasProperties = true;
                                break;
                            }
                        }
                    }
                    if (hasProperties)
                    {
                        break;
                    }
                }
                else if (prop.GetType() == DataTypes.OntologyPropertyDate)
                {
                    if (prop.Value != null && !string.IsNullOrEmpty(prop.Value.ToString()) && !string.IsNullOrWhiteSpace(prop.Value.ToString()) && !prop.Value.ToString().Equals("00000000000000"))
                    {
                        hasProperties = true;
                        break;
                    }
                }
                else
                {
                    hasProperties = true;
                }
            }

            return hasProperties;
        }

        /// <summary>
        /// Validates if the auxiliary entity has <c>rdf:type</c> defined
        /// </summary>
        /// <returns><c>true</c> if <see cref="SecondaryEntity"/> has defined the property <c>rdf:type</c>. <c>false</c> in another case</returns>
        public bool HasRdfTypeDefined()
        {
            return !string.IsNullOrEmpty(RdfType) && !string.IsNullOrWhiteSpace(RdfType);
        }

        /// <summary>
        /// Validates if the auxiliary entity has <c>rdfs:label</c> defined
        /// </summary>
        /// <returns><c>true</c> if <see cref="SecondaryEntity"/> has defined the property <c>rdfs:label</c>. <c>false</c> in another case</returns>
        public bool HasRdfsLabelDefined()
        {
            return !string.IsNullOrEmpty(RdfType) && !string.IsNullOrEmpty(RdfsLabel) && !string.IsNullOrEmpty(RdfsLabel) && !string.IsNullOrEmpty(RdfsLabel);
        }

        /// <summary>
        /// Validates if the auxiliary entity has <c>rdf:type</c> and <c>rdfs:label</c> defined
        /// </summary>
        /// <returns><c>true</c> if <see cref="OntologyEntity"/> has defined the properties <c>rdf:type</c> and <c>rdfs:label</c>. <c>false</c> in another case</returns>
        public bool HasRDFTypeAndRDFLabelDefined()
        {
            return HasRdfsLabelDefined();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rdf:type property
        /// </summary>
        public string RdfType
        {
            get { return _rdfType; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfType");
                }
                else
                {
                    _rdfType = value;
                    if (RdfType.Contains("#"))
                    {
                        Items = RdfType.Substring(RdfType.LastIndexOf("#") + 1);
                    }
                    else if (RdfType.Contains("/"))
                    {
                        Items = RdfType.Substring(RdfType.LastIndexOf("/") + 1);
                    }
                    else
                    {
                        Items = RdfType;
                    }
                }

            }
        }

        /// <summary>
        /// Gets or sets the rdfs:label property
        /// </summary>
        public string RdfsLabel
        {
            get { return _rdfsLabel; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfsLabel");
                }
                else
                {
                    _rdfsLabel = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the properties of the entity
        /// </summary>
        public List<OntologyProperty> Properties
        {
            get { return _properties; }
            set
            {
                _properties = value.Distinct().ToList();
            }
        }

        /// <summary>
        /// Gets or sets the entity list that <see cref="OntologyEntity"/> contains
        /// </summary>
        public List<OntologyEntity> Entities
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the predicate that contains a reference to the auxiliary entity
        /// </summary>
        public string Label
        {
            get; set;
        }

        /// <summary>
        /// Gets the part of the identifier of the auxiliary entity
        /// </summary>
        public string Items
        {
            get; set;
        }

        /// <summary>
        /// Gets the entity identifier
        /// </summary>
        public Guid GuidEntidad
        {
            get;
        }
        #endregion
    }
}
