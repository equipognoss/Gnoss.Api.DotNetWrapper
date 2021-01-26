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
    /// Represent an ontology
    /// </summary>
    public sealed class Ontology : BaseOntology
    {
        #region Members

        private string _items;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the first part of the resource identifier
        /// 
        /// </summary>
        public Guid ResourceId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the second part of the resource identifier
        /// </summary>
        public Guid ArticleId
        {
            get; set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="Ontology"/>
        /// </summary>
        /// <param name="graphsUrl">Graphs url of the enviroment.</param>
        /// <param name="ontologyUrl">Download url of the ontology</param>
        /// <param name="rdfType">The <c>rdf:type</c> property of the ontology</param>
        /// <param name="rdfsLabel">the <c>rdfs:label</c> of the ontology</param>
        /// <param name="prefixList">Prefix list declared in the ontology</param>
        /// <param name="propertyList">Property list of the ontology</param>
        /// <param name="entityList">list of <see cref="OntologyEntity"> auxiliary entities</see></param>
        public Ontology(string graphsUrl, string ontologyUrl, string rdfType, string rdfsLabel, List<string> prefixList, List<OntologyProperty> propertyList, List<OntologyEntity> entityList = null)
            : base(graphsUrl, ontologyUrl, rdfType, rdfsLabel, prefixList, propertyList, entityList)
        {
            ResourceId = Guid.NewGuid();
            ArticleId = Guid.NewGuid();

            if (RdfType != null && RdfType.Contains("#"))
            {
                _items = RdfType.Substring(RdfType.LastIndexOf("#") + 1);
            }
            else if (RdfType != null && RdfType.Contains("/"))
            {
                _items = RdfType.Substring(RdfType.LastIndexOf("/") + 1);
            }
            else
            {
                _items = RdfType;
            }

            Identifier = $"{GraphsUrl}items/{_items}_{ResourceId}_{ArticleId}";
        }

        /// <summary>
        /// Constructor of <see cref="Ontology"/>
        /// </summary>
        /// <param name="graphsUrl">Graphs url of the enviroment.</param>
        /// <param name="ontologyUrl">Download url of the ontology</param>
        /// <param name="rdfType">The <c>rdf:type</c> property of the ontology</param>
        /// <param name="rdfsLabel">the <c>rdfs:label</c> of the ontology</param>
        /// <param name="prefixList">Prefix list declared in the ontology</param>
        /// <param name="propertyList">Property list of the ontology</param>
        /// <param name="entityList">list of <see cref="OntologyEntity"> auxiliary entities</see></param>
        /// <param name="resourceId">First part of the resource identifier</param>
        /// <param name="articleId">Second part of the resource identifier</param>
        public Ontology(string graphsUrl, string ontologyUrl, string rdfType, string rdfsLabel, List<string> prefixList, List<OntologyProperty> propertyList, List<OntologyEntity> entityList, Guid resourceId, Guid articleId)
            : base(graphsUrl, ontologyUrl, rdfType, rdfsLabel, prefixList, propertyList, entityList)
        {
            if (resourceId != Guid.Empty)
            {
                ResourceId = resourceId;
            }
            else
            {
                ResourceId = Guid.NewGuid();
            }

            if (articleId != Guid.Empty)
            {
                ArticleId = articleId;
            }
            else
            {
                ArticleId = Guid.NewGuid();
            }

            if (RdfType != null && RdfType.Contains("#"))
            {
                _items = RdfType.Substring(RdfType.LastIndexOf("#") + 1);
            }
            else if (RdfType != null && RdfType.Contains("/"))
            {
                _items = RdfType.Substring(RdfType.LastIndexOf("/") + 1);
            }
            else
            {
                _items = RdfType;
            }

            Identifier = $"{GraphsUrl}items/{_items}_{ResourceId}_{ArticleId}";
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

            WriteRdfHeader();
            Dictionary<Guid, OntologyEntity> entitiesDictionary = EntityListToEntityDictionary();
            byte[] rdfFile = null;

            if (string.IsNullOrWhiteSpace(RdfType) || string.IsNullOrEmpty(RdfType))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfType");
            }
            else if (string.IsNullOrWhiteSpace(RdfsLabel) || string.IsNullOrEmpty(RdfsLabel))
            {
                throw new GnossAPIArgumentException("Required. It can't be null or empty", "RdfsLabel");
            }
            else
            {
                // FirstDescription(Global description)
                File.WriteLine($"<rdf:Description rdf:about=\"{Identifier}\">");
                Write("rdf:type", RdfType);
                Write("rdfs:label", RdfsLabel);
                WritePropertyList(Properties, ResourceId);
                if (entitiesDictionary == null || entitiesDictionary.Count == 0)
                {
                    File.WriteLine("</rdf:Description>");
                }
                WriteEntityFirstDescription(entitiesDictionary, ResourceId, true);

                // Additional Descriptions
                WriteEntityAdditionalDescription(entitiesDictionary, ResourceId);

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

        #region Internal methods

        internal override void WritePropertyList(List<OntologyProperty> propertyList, Guid resourceId)
        {
            if (propertyList != null)
            {
                foreach (OntologyProperty prop in propertyList)
                {
                    if (!string.IsNullOrWhiteSpace(prop.Name) && prop.Value != null)
                    {
                        if (prop.GetType().Equals(DataTypes.OntologyPropertyImage))
                        {
                            if (prop.Value.ToString().Contains(Constants.IMAGES_PATH_ROOT))
                            {
                                prop.Value = prop.Value.ToString().Substring(prop.Value.ToString().LastIndexOf("/") + "/".Length);
                            }
                            prop.Value = GnossHelper.GetImagePath(resourceId, prop.Value.ToString());
                        }
                        Write(prop.Name, prop.Value, prop.Language);
                    }
                }
            }
        }

        #endregion
    }
}
