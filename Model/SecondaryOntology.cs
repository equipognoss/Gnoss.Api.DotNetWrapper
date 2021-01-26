using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represent a secondary ontology
    /// </summary>
    public class SecondaryOntology : BaseOntology
    {
        #region Members

        private Guid _resourceId = Guid.Empty;

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
        /// <param name="entityList">List of <see cref="OntologyEntity"> auxiliary entities</see></param>
        /// <param name="secondaryEntityList">Secondary entity list of the ontology</param>
        public SecondaryOntology(string graphsUrl, string ontologyUrl, string rdfType, string rdfsLabel, List<string> prefixList, List<OntologyProperty> propertyList, string identifier, List<SecondaryEntity> secondaryEntityList = null, List<OntologyEntity> entityList = null)
            : base(graphsUrl, ontologyUrl, rdfType, rdfsLabel, prefixList, propertyList, identifier, entityList)
        {
            _resourceId = Guid.NewGuid();
            if (string.IsNullOrEmpty(identifier) || string.IsNullOrWhiteSpace(identifier))
            {
                throw new GnossAPIArgumentException("Required. It can't be null", "identifier");
            }

            SecondaryEntities = secondaryEntityList;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Generate the rdf file with all the components of the ontology
        /// </summary>
        /// <returns>Rdf file</returns>
        public override byte[] GenerateRDF()
        {
            Stream = new MemoryStream();
            File = new StreamWriter(Stream);
            byte[] rdfFile = null;
            WriteRdfHeader();
            Dictionary<Guid, OntologyEntity> entitiesDictionary = EntityListToEntityDictionary();
            Dictionary<string, SecondaryEntity> secondaryEntityDictionary = SecondaryEntityListToSecondaryEntityDictionary();
            if (string.IsNullOrEmpty(RdfType) || string.IsNullOrWhiteSpace(RdfType))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfType");
            }
            else if (string.IsNullOrEmpty(RdfsLabel) || string.IsNullOrWhiteSpace(RdfsLabel))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfsLabel");
            }
            else
            {
                // FirstDescription (Global description)
                File.WriteLine($"<rdf:Description rdf:about=\"{GraphsUrl}items/{Identifier}\">");
                if (!string.IsNullOrEmpty(RdfType) || !string.IsNullOrEmpty(RdfsLabel))
                {
                    Write("rdf:type", RdfType);
                    Write("rdfs:label", RdfsLabel);
                }
                else
                {
                    throw new GnossAPIException("RdfType and RdfLabel are required, they can't be null or empty");
                }

                WritePropertyList(Properties, Guid.Empty);
                WriteEntityFirstDescription(entitiesDictionary, _resourceId);
                WriteSecondaryEntityFirstDescription(secondaryEntityDictionary);
                File.WriteLine("</rdf:Description>");

                // Additional Descriptions
                WriteEntityAdditionalDescription(entitiesDictionary, _resourceId);

                WriteSecondaryEntityAdditionalDescription(secondaryEntityDictionary);

                File.WriteLine("</rdf:RDF>");

                // RDF file
                File.Flush();
                Stream.Position = 0;
                BinaryReader b2 = new BinaryReader(Stream);
                rdfFile = b2.ReadBytes((int)Stream.Length);

                b2.Dispose();
                b2.Close();
                Stream.Dispose();
                Stream.Close();
                File.Dispose();
                File.Close();
            }
            return rdfFile;
        }
        #endregion

        #region Private methods

        internal override void WritePropertyList(List<OntologyProperty> properties, Guid resourceId)
        {
            if (resourceId == Guid.Empty)
            {
                if (properties != null)
                {
                    foreach (OntologyProperty prop in properties)
                    {
                        if (!string.IsNullOrWhiteSpace(prop.Name) && prop.Value != null && !prop.GetType().Equals(DataTypes.OntologyPropertyImage))
                        {
                            Write(prop.Name, prop.Value, prop.Language);
                        }
                    }
                }
            }
        }

        private Dictionary<string, SecondaryEntity> SecondaryEntityListToSecondaryEntityDictionary()
        {
            Dictionary<string, SecondaryEntity> secondaryEntityDictionary = null;
            if (SecondaryEntities != null)
            {
                foreach (SecondaryEntity secondaryEntity in SecondaryEntities)
                {
                    if (secondaryEntityDictionary == null)
                    {
                        secondaryEntityDictionary = new Dictionary<string, SecondaryEntity>();
                    }
                    secondaryEntityDictionary.Add(secondaryEntity.Identifier, secondaryEntity);
                }
            }
            return secondaryEntityDictionary;
        }

        private void WriteSecondaryEntityFirstDescription(Dictionary<string, SecondaryEntity> secondaryEntityDictionary)
        {
            if (secondaryEntityDictionary != null)
            {
                foreach (string id in secondaryEntityDictionary.Keys)
                {
                    if (!secondaryEntityDictionary[id].HasRdfTypeDefined())
                    {
                        throw new GnossAPIArgumentException("Required. It can't be null", "secondaryEntityDictionary[id].RdfType");
                    }
                    else if (!secondaryEntityDictionary[id].HasRdfsLabelDefined())
                    {
                        throw new GnossAPIArgumentException("Required. It can't be null", "secondaryEntityDictionary[id].RdfsLabel");
                    }
                    else
                    {
                        if (secondaryEntityDictionary[id].HasAnyPropertyWithData())
                        {
                            Write(secondaryEntityDictionary[id].Label, secondaryEntityDictionary[id].Identifier);
                        }
                    }
                }
            }
        }

        private void WriteSecondaryEntityAdditionalDescription(Dictionary<string, SecondaryEntity> secondaryEntityDictionary)
        {
            if (secondaryEntityDictionary != null)
            {
                foreach (string id in secondaryEntityDictionary.Keys)
                {
                    if (!secondaryEntityDictionary[id].HasRdfTypeDefined())
                    {
                        throw new GnossAPIArgumentException("Required. It can't be null", "secondaryEntityDictionary[id].RdfType");
                    }
                    else if (!secondaryEntityDictionary[id].HasRdfsLabelDefined())
                    {
                        throw new GnossAPIArgumentException("Required. It can't be null", "secondaryEntityDictionary[id].RdfsLabel");
                    }
                    else
                    {
                        File.WriteLine($"<rdf:Description rdf:about=\"{secondaryEntityDictionary[id].Identifier}\">");
                       
                        Write("rdf:type", secondaryEntityDictionary[id].RdfType);
                        Write("rdfs:label", secondaryEntityDictionary[id].RdfsLabel);
                       
                        if (secondaryEntityDictionary[id].Properties != null)
                        {
                            foreach (OntologyProperty prop in secondaryEntityDictionary[id].Properties)
                            {
                                if (secondaryEntityDictionary[id].HasAnyPropertyWithData())
                                {
                                    Write(prop.Name, prop.Value, prop.Language);
                                }
                            }
                        }
                        File.WriteLine("</rdf:Description>");
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the secondary entity list of the ontology
        /// </summary>
        public List<SecondaryEntity> SecondaryEntities
        {
            get; set;
        }

        #endregion
    }
}
