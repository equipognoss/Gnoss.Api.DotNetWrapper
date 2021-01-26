﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Abstract class that represents an ontology
    /// </summary>
    public abstract class BaseOntology
    {
        #region Members

        private string _name;
        private string _rdfType;
        private string _rdfsLabel;
        private List<OntologyProperty> _properties;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="BaseOntology"/>
        /// </summary>
        /// <param name="graphsUrl">Graphs url of the enviroment.</param>
        /// <param name="ontologyUrl">Download url of the ontology</param>
        /// <param name="rdfType">The <c>rdf:type</c> property of the ontology</param>
        /// <param name="rdfsLabel">the <c>rdfs:label</c> of the ontology</param>
        /// <param name="prefixList">Prefix list declared in the ontology</param>
        /// <param name="propertyList">Property list of the ontology</param>
        /// <param name="identifier">Ontology identifier</param>
        /// <param name="entityList">list of <see cref="OntologyEntity"> auxiliary entities</see></param>
        public BaseOntology(string graphsUrl, string ontologyUrl, string rdfType, string rdfsLabel, List<string> prefixList, List<OntologyProperty> propertyList, string identifier, List<OntologyEntity> entityList = null)
        {
            if (string.IsNullOrWhiteSpace(rdfType) || string.IsNullOrEmpty(rdfType))
            {
                throw new GnossAPIArgumentException("Required.It can't be null or empty", "rdfType");
            }
            else if (string.IsNullOrWhiteSpace(rdfsLabel) || string.IsNullOrEmpty(rdfsLabel))
            {
                throw new GnossAPIArgumentException("Required.It can't be null or empty", "rdfsLabel");
            }
            else
            {
                GraphsUrl = graphsUrl;
                OntologyUrl = ontologyUrl;
                this._rdfType = rdfType;
                this._rdfsLabel = rdfsLabel;
                PrefixList = prefixList;
                this._properties = propertyList;
                Entities = entityList;
                this.Identifier = identifier;
            }
        }

        /// <summary>
        /// Constructor of <see cref="BaseOntology"/>
        /// </summary>
        /// <param name="graphsUrl">Graphs url of the enviroment.</param>
        /// <param name="ontologyUrl">Download url of the ontology</param>
        /// <param name="rdfType">The <c>rdf:type</c> property of the ontology</param>
        /// <param name="rdfsLabel">the <c>rdfs:label</c> of the ontology</param>
        /// <param name="prefixList">Prefix list declared in the ontology</param>
        /// <param name="propertyList">Property list of the ontology</param>
        /// <param name="entityList">list of <see cref="OntologyEntity"> auxiliary entities</see></param>
        public BaseOntology(string graphsUrl, string ontologyUrl, string rdfType, string rdfsLabel, List<string> prefixList, List<OntologyProperty> propertyList, List<OntologyEntity> entityList = null)
        {
            if (string.IsNullOrWhiteSpace(rdfType) || string.IsNullOrEmpty(rdfType))
            {
                throw new GnossAPIArgumentException("Required.It can't be null or empty", "rdfType");
            }
            else if (string.IsNullOrWhiteSpace(rdfsLabel) || string.IsNullOrEmpty(rdfsLabel))
            {
                throw new GnossAPIArgumentException("Required.It can't be null or empty", "rdfsLabel");
            }
            else
            {
                GraphsUrl = graphsUrl;
                OntologyUrl = ontologyUrl;
                this._rdfType = rdfType;
                this._rdfsLabel = rdfsLabel;
                PrefixList = prefixList;
                this._properties = propertyList;
                Entities = entityList;
            }
        }

        internal BaseOntology()
        {

        }

        #endregion

        #region Internal methods to build the RDF

        internal void WriteRdfHeader()
        {
            string txt = string.Empty;

            txt += "<rdf:RDF ";
            txt += $"xmlns:gnossonto=\"{OntologyUrl}\"";
            foreach (string ontologia in PrefixList)
            {
                txt += $" {ontologia}";
            }
            txt += ">";
            File.WriteLine(txt);
        }

        internal void Write(string label, string value, string languageAttribute = null)
        {
            if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(label))
            {
                if (value.EndsWith("\0"))
                {
                    value = value.Replace("\0", "");
                }
                if (value.Contains("\r\n"))
                {
                    value = value.Replace("\r\n", "<br />");
                }
                if (value.Contains("\n"))
                {
                    value = value.Replace("\n", "<br />");
                }
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value.Contains("&") || value.Contains("<") || value.Contains(">"))
                    {
                        if (languageAttribute == null)
                        {
                            File.WriteLine($"<{label}><![CDATA[{value}]]></{label}>");
                        }
                        else
                        {
                            File.WriteLine($"<{label} xml:lang=\"{languageAttribute}\"><![CDATA[{value}]]></{label}>");
                        }
                    }
                    else
                    {
                        if (languageAttribute == null)
                        {
                            File.WriteLine($"<{label}>{value}</{label}>");
                        }
                        else
                        {
                            File.WriteLine($"<{label} xml:lang=\"{languageAttribute}\">{value}</{label}>");
                        }
                    }
                }
            }
        }

        internal void Write(string label, bool value)
        {
            if (value)
            {
                File.WriteLine($"<{label}>true</{label}>");
            }
            else
            {
                File.WriteLine($"<{label}>false</{label}>");
            }
        }

        internal void Write(string label, List<string> valueList, string languageAttribute = null)
        {
            List<string> listAux = null;
            if (valueList != null && valueList.Count > 0)
            {
                if (listAux == null)
                {
                    listAux = new List<string>();
                }
                foreach (string value in valueList)
                {
                    string newValue = value;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        if (newValue.EndsWith("\0"))
                        {
                            newValue = newValue.Replace("\0", "");
                        }
                        if (newValue.Contains("\r\n"))
                        {
                            newValue = newValue.Replace("\r\n", "<br />");
                        }
                        if (newValue.Contains("\n"))
                        {
                            newValue = newValue.Replace("\n", "<br />");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(newValue) && !listAux.Contains(newValue))
                    {
                        listAux.Add(newValue);
                        if (newValue.Contains("&") || newValue.Contains("<") || newValue.Contains(">"))
                        {
                            if (languageAttribute == null)
                            {
                                File.WriteLine($"<{label}><![CDATA[{newValue}]]></{label}>");
                            }
                            else
                            {
                                File.WriteLine($"<{label} xml:lang=\"{languageAttribute}\"><![CDATA[{newValue}]]></{label}>");
                            }
                        }
                        else
                        {
                            if (languageAttribute == null)
                            {
                                File.WriteLine($"<{label}>{newValue}</{label}>");
                            }
                            else
                            {
                                File.WriteLine($"<{label} xml:lang=\"{languageAttribute}\">{newValue}</{label}>");
                            }
                        }
                    }
                }
            }
        }

        internal void Write(string label, object value, string languageAttribute = null)
        {
            List<string> listString = new List<string>();
            if (label != null && value != null)
            {
                Type dataType = value.GetType();

                if (value.GetType().Equals(DataTypes.String))
                {
                    Write(label, value as string, languageAttribute);
                }
                else if (value.GetType().Equals(DataTypes.ListString))
                {
                    Write(label, value as List<string>, languageAttribute);
                }
                else if (value.GetType().Equals(DataTypes.Bool))
                {
                    Write(label, Convert.ToBoolean(value));
                }
            }
        }

        internal Dictionary<Guid, OntologyEntity> EntityListToEntityDictionary(List<OntologyEntity> entityList = null)
        {
            Dictionary<Guid, OntologyEntity> entityDictionary = null;
            if (entityList == null)
            {
                if (Entities != null)
                {
                    foreach (OntologyEntity entity in Entities)
                    {
                        if (entityDictionary == null)
                        {
                            entityDictionary = new Dictionary<Guid, OntologyEntity>();
                        }

                        if (!entityDictionary.ContainsKey(entity.GuidEntidad))
                        {
                            entityDictionary.Add(entity.GuidEntidad, entity);
                        }
                    }
                }
            }
            else
            {
                foreach (OntologyEntity entity in entityList)
                {
                    if (entityDictionary == null)
                    {
                        entityDictionary = new Dictionary<Guid, OntologyEntity>();
                    }

                    if (!entityDictionary.ContainsKey(entity.GuidEntidad))
                    {
                        entityDictionary.Add(entity.GuidEntidad, entity);
                    }
                }
            }
            return entityDictionary;
        }

        internal abstract void WritePropertyList(List<OntologyProperty> propertyList, Guid resourceId);

        internal void WriteEntityFirstDescription(Dictionary<Guid, OntologyEntity> entityDictionary, Guid resourceId, bool includeCloseDescription = false)
        {
            if (entityDictionary != null)
            {
                foreach (Guid id in entityDictionary.Keys)
                {
                    if ((entityDictionary[id].HasAnyPropertyWithData() || (entityDictionary[id].Entities != null && entityDictionary[id].Entities.Count > 0)) && entityDictionary[id].HasRDFTypeAndRDFLabelDefined())
                    {
                        Write(entityDictionary[id].Label, $"{GraphsUrl}items/{entityDictionary[id].Items}_{resourceId}_{id}");
                    }
                }
                if (includeCloseDescription)
                {
                    File.WriteLine("</rdf:Description>"); // End first description
                }
            }
        }

        internal void WriteEntityAdditionalDescription(Dictionary<Guid, OntologyEntity> entityDictionary, Guid resourceId)
        {
            if (entityDictionary != null)
            {
                foreach (Guid id in entityDictionary.Keys)
                {
                    if ((entityDictionary[id].HasAnyPropertyWithData()) || (entityDictionary[id].Entities != null && entityDictionary[id].Entities.Count > 0) && entityDictionary[id].HasRDFTypeAndRDFLabelDefined())
                    {
                        File.WriteLine($"<rdf:Description rdf:about=\"{GraphsUrl}items/{entityDictionary[id].Items}_{resourceId}_{id}\">");
                        if (string.IsNullOrWhiteSpace(entityDictionary[id].RdfsLabel) || string.IsNullOrWhiteSpace(entityDictionary[id].RdfType))
                        {
                            throw new GnossAPIException("rdfType and rdfLabel are required, they can't be null or empty");
                        }
                        else
                        {
                            Write("rdf:type", entityDictionary[id].RdfType);
                            Write("rdfs:label", entityDictionary[id].RdfsLabel);

                            if (entityDictionary[id].Properties != null)
                            {
                                foreach (OntologyProperty prop in entityDictionary[id].Properties)
                                {
                                    if (prop.GetType().Equals(DataTypes.OntologyPropertyImage))
                                    {
                                        if (prop.Value.ToString().Contains(Constants.IMAGES_PATH_ROOT))
                                        {
                                            prop.Value = prop.Value.ToString().Substring(prop.Value.ToString().LastIndexOf("/") + "/".Length);
                                        }
                                        prop.Value = GnossHelper.GetImagePath(resourceId, prop.Value.ToString());
                                    }
                                    Write(prop.Name, prop.Value,  prop.Language);
                                }
                                if (entityDictionary[id].Entities == null || entityDictionary[id].Entities.Count == 0)
                                {
                                    File.WriteLine("</rdf:Description>");
                                }
                            }
                            if (entityDictionary[id].Entities != null && entityDictionary[id].Entities.Count > 0)
                            {
                                Dictionary<Guid, OntologyEntity> dicSubEnt = EntityListToEntityDictionary(entityDictionary[id].Entities);
                                WriteEntityFirstDescription(dicSubEnt, resourceId, true);
                                WriteEntityAdditionalDescription(dicSubEnt, resourceId);

                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Generate the rdf file with all the components of the ontology
        /// </summary>
        /// <returns>Rdf file</returns>
        public abstract byte[] GenerateRDF();

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name of the ontology
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name) || string.IsNullOrWhiteSpace(_name))
                {
                    _name = OntologyUrl.Substring(OntologyUrl.LastIndexOf(CharDelimiters.Slash) + 1);
                }
                return _name;
            }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the prefix list declared in the ontology
        /// </summary>
        public virtual List<string> PrefixList
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the <c>rdf:type</c> of the ontology
        /// </summary>
        public string RdfType
        {
            get { return _rdfType; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new GnossAPIArgumentException("Required.It can't be null or empty", "RdfType");
                }
                else
                {
                    _rdfType = value;
                }

            }
        }

        /// <summary>
        /// Gets or sets the <c>rdfs:label</c> of the ontology
        /// </summary>
        public string RdfsLabel
        {
            get { return _rdfsLabel; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new GnossAPIArgumentException("Required.It can't be null or empty", "RdfsLabel");
                }
                else
                {
                    _rdfsLabel = value;
                }

            }
        }

        /// <summary>
        /// Gets or sets the ontology identifier
        /// </summary>
        public string Identifier
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the property list of the ontology
        /// </summary>
        public List<OntologyProperty> Properties
        {
            get
            {
                if (_properties != null)
                {
                    _properties = _properties.Distinct().ToList();
                    return _properties;
                }
                else
                {
                    return _properties;
                }
            }
            set
            {
                _properties = value;

            }
        }

        /// <summary>
        /// Gets or sets the list of <see cref="OntologyEntity"> auxiliary entities</see>
        /// </summary>
        public List<OntologyEntity> Entities
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the graphs url of the enviroment. 
        /// </summary>
        public string GraphsUrl
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the download url of the ontology
        /// </summary>
        public string OntologyUrl
        {
            get; set;
        }

        #endregion

        #region Internal properties

        internal MemoryStream Stream
        {
            get; set;
        }

        internal StreamWriter File
        {
            get; set;
        }

        #endregion
    }
}
