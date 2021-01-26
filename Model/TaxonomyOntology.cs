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
    /// Represents the ontology taxonomy.owl
    /// </summary>
    public class TaxonomyOntology : BaseOntology
    {
        #region Constants

        private const string SKOS_NARROWER_LABEL = "skos:narrower";
        private const string SKOS_MEMBER_LABEL = "skos:member";
        private const string XSD = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema#\"";
        private const string DC = "xmlns:dc=\"http://purl.org/dc/elements/1.1/\"";
        private const string RDFS = "xmlns:rdfs=\"http://www.w3.org/2000/01/rdf-schema#\"";
        private const string RDF = "xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"";
        private const string OWL = "xmlns:owl=\"http://www.w3.org/2002/07/owl#\"";
        private const string TAXO = "xmlns:taxo=\"http://www.taxonomy-ontology/2013-10#\"";
        private const string SKOS = "xmlns:skos=\"http://www.w3.org/2004/02/skos/core#\"";
        private const string RDF_TYPE_COLLECTION = "http://www.w3.org/2008/05/skos#Collection";
        private const string RDFS_LABEL_COLLECTION = "http://www.w3.org/2008/05/skos#Collection";

        #endregion

        #region Members

        private byte[] _rdfFile;
        private string _stringRdfFile;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="TaxonomyOntology"/>.
        /// </summary>
        /// <param name="collectionNameIdentifier">Collection identifier</param>
        /// <param name="dcSource"><c>dc:source</c></param>
        /// <param name="scopeNote"><c>skos:scopeNote</c></param>
        /// <param name="conceptEntityList">Concept entity list</param>
        public TaxonomyOntology(string collectionNameIdentifier, string dcSource, string scopeNote, List<ConceptEntity> conceptEntityList)
        {
            Identifier = $"{GraphsUrl}items/{collectionNameIdentifier}";
            RdfType = RDF_TYPE_COLLECTION;
            RdfsLabel = RDFS_LABEL_COLLECTION;

            OntologyUrl = $"{GraphsUrl}Ontologia/Taxonomy.owl";
            PrefixList = new List<string>() { DC, RDFS, OWL, XSD, RDF, TAXO, SKOS };

            if (Properties == null)
            {
                Properties = new List<OntologyProperty>();
            }

            // dc:source
            Properties.Add(new StringOntologyProperty("dc:source", dcSource));

            // skos:scopeNote
            Properties.Add(new StringOntologyProperty("skos:scopeNote", scopeNote));

            ConceptEntities = conceptEntityList;

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

                File.WriteLine($"<rdf:Description rdf:about=\"{Identifier}\">");
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
                WriteConceptEntitiesFirstDescription(ConceptEntities);
                File.WriteLine("</rdf:Description>");

                // Additional Descriptions

                WriteConceptEntitiesAdditionalDescription(ConceptEntities);

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

        internal override void WritePropertyList(List<OntologyProperty> propertyList, Guid resourceId)
        {
            if (resourceId == Guid.Empty)
            {
                if (propertyList != null)
                {
                    foreach (OntologyProperty prop in propertyList)
                    {
                        if (!string.IsNullOrWhiteSpace(prop.Name) && prop.Value != null && !prop.GetType().Equals(DataTypes.OntologyPropertyImage))
                        {
                            Write(prop.Name, prop.Value, prop.Language);
                        }
                    }
                }
            }
        }

        private void WriteConceptEntitiesFirstDescription(List<ConceptEntity> conceptEntityList)
        {
            if (conceptEntityList != null)
            {
                foreach (ConceptEntity ec in conceptEntityList)
                {
                    WriteConceptEntityFirstDescription(ec);
                }
            }
        }

        private void WriteConceptEntityFirstDescription(ConceptEntity conceptEntity)
        {
            if (!conceptEntity.HasRdfTypeDefined())
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfType");
            }
            else if (!conceptEntity.HasRdfsLabelDefined())
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfsLabel");
            }
            else
            {
                if (conceptEntity.HasAnyPropertyWithData())
                {
                    if (conceptEntity.ParentIdentifier == null)
                    {
                        Write(SKOS_MEMBER_LABEL, conceptEntity.ConceptEntityGnossId);
                    }
                    else
                    {
                        Write(SKOS_NARROWER_LABEL, conceptEntity.ConceptEntityGnossId);
                    }
                }
            }
        }

        private void WriteConceptEntitiesAdditionalDescription(List<ConceptEntity> conceptEntityList)
        {
            if (conceptEntityList != null)
            {
                foreach (ConceptEntity ec in conceptEntityList)
                {
                    WriteConceptEntityAdditionalDescription(ec);
                }
            }
        }

        private void WriteConceptEntityAdditionalDescription(ConceptEntity conceptEntity)
        {
            if (!conceptEntity.HasRdfTypeDefined())
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfType");
            }
            else if (!conceptEntity.HasRdfsLabelDefined())
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfsLabel");
            }
            else
            {
                if (conceptEntity.HasAnyPropertyWithData())
                {
                    File.WriteLine($"<rdf:Description rdf:about=\"{conceptEntity.ConceptEntityGnossId}\">");

                    Write("rdf:type", conceptEntity.RdfType);
                    Write("rdfs:label", conceptEntity.RdfsLabel);

                    if (conceptEntity.Properties != null)
                    {
                        foreach (OntologyProperty prop in conceptEntity.Properties)
                        {
                            if (conceptEntity.HasAnyPropertyWithData())
                            {
                                Write(prop.Name, prop.Value, prop.Language);
                            }
                        }
                    }
                    if (conceptEntity.Subentities != null && conceptEntity.Subentities.Count > 0)
                    {
                        WriteConceptEntitiesFirstDescription(conceptEntity.Subentities);
                    }
                    File.WriteLine("</rdf:Description>");
                    if (conceptEntity.Subentities != null && conceptEntity.Subentities.Count > 0)
                    {
                        foreach (ConceptEntity newEc in conceptEntity.Subentities)
                        {
                            WriteConceptEntityAdditionalDescription(newEc);
                        }
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the concept entity list of the ontology
        /// </summary>
        public List<ConceptEntity> ConceptEntities
        {
            get;
        }

        /// <summary>
        /// Gets or sets the text file with the rdf content of the ontology
        /// </summary>
        public byte[] RdfFile
        {
            get
            {
                _rdfFile = GenerateRDF();
                return _rdfFile;
            }
            set
            {
                _rdfFile = value;
            }
        }

        /// <summary>
        /// Gets the content of the <see cref="RdfFile">RdfFile</see> as string
        /// </summary>
        public string StringRdfFile
        {
            get
            {
                StreamReader sr = new StreamReader(new MemoryStream(RdfFile));
                _stringRdfFile = sr.ReadToEnd();
                return _stringRdfFile;
            }
        }
        #endregion
    }
}
