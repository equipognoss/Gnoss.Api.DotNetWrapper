using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a secondary entity of the ontology taxonomy.owl
    /// </summary>
    public sealed class ConceptEntity : SecondaryEntity
    {
        #region Members

        private Dictionary<string, string> _prefLabelDictionary = null;
        private string _rootIdentifier;
        private string _nameIdentifier;
        private string _parentNameIdentifier;
        private string _conceptEntityGnossId = null;
        private string _parentGnossId = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="ConceptEntity"/>.
        /// </summary>
        /// <param name="identifierNameRoot">Identifier name of the <see cref="ConceptEntity"/></param>
        /// <param name="identifier">Identifier of the entity that will be concatenated to tha name</param>
        /// <param name="dcSource"><c>dc:source</c></param>
        /// <param name="prefLabelDictionary">Dictionary of prefLabel</param>
        /// <param name="subentities">Subentities of this entity</param>
        /// <param name="parentIdentifier">Identifier of the parent</param>
        /// <param name="level">Level of this entity (0 for the roots elements, 1 for the child...)</param>
        /// <param name="GraphsUrl">The base url of the graphs. (example: http://gnoss.com/) </param>
        public ConceptEntity(string identifierNameRoot, string identifier, string dcSource, Dictionary<string, string> prefLabelDictionary, List<ConceptEntity> subentities, string parentIdentifier, string GraphsUrl, LogHelper logHelper, int level = 1)
        {
            if (string.IsNullOrEmpty(dcSource) || string.IsNullOrWhiteSpace(dcSource) || prefLabelDictionary == null || string.IsNullOrWhiteSpace(identifierNameRoot) || string.IsNullOrEmpty(identifierNameRoot) || string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(identifier))
            {
                logHelper.Instance.Error(ErrorText.RequiredParameterConstructor, this.GetType().Name);
                throw new GnossAPIException(ErrorText.RequiredParameterConstructor);
            }
            _prefLabelDictionary = prefLabelDictionary;

            // rdf:type and rdfs:label are always the same
            RdfType = "http://www.w3.org/2008/05/skos#Concept";
            RdfsLabel = "http://www.w3.org/2008/05/skos#Concept";

            if (Properties == null)
            {
                Properties = new List<OntologyProperty>();
            }
            Subentities = subentities;
            _rootIdentifier = identifierNameRoot;
            _nameIdentifier = identifier.Replace(".", "_");
            _parentNameIdentifier = parentIdentifier;
            Level = level;

            // skos:prefLabel
            foreach (string idioma in _prefLabelDictionary.Keys)
            {
                Properties.Add(new StringOntologyProperty("skos:prefLabel", _prefLabelDictionary[idioma], idioma));
            }

            //dc:identifier
            Properties.Add(new StringOntologyProperty("dc:identifier", identifier));

            // skos:broader
            if (ParentIdentifier != null)
            {
                Properties.Add(new StringOntologyProperty("skos:broader", ParentGnossId));
            }

            //dc:source
            Properties.Add(new StringOntologyProperty("dc:source", dcSource));

            Properties.Add(new StringOntologyProperty("skos:symbol", Level.ToString()));

            this.GraphsUrl = GraphsUrl;

        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the hierarchy level of the entity
        /// </summary>
        public int Level
        {
            get;
        }

        /// <summary>
        /// Gets the identifier URI of the parent (if exists)
        /// </summary>
        public string ParentGnossId
        {
            get
            {
                if (!string.IsNullOrEmpty(_parentNameIdentifier) && !string.IsNullOrWhiteSpace(_parentNameIdentifier))
                {
                    _parentGnossId = !_rootIdentifier.EndsWith("_") ? $"{_rootIdentifier}_{_parentNameIdentifier}" : $"{_rootIdentifier}{_parentNameIdentifier}";
                    _parentGnossId = $"{GraphsUrl}items/{_parentGnossId}";
                }
                else
                {
                    _parentGnossId = null;
                }
                return _parentGnossId;
            }
        }

        /// <summary>
        /// Gets the identifier of the concept entity
        /// </summary>
        public string ConceptEntityGnossId
        {
            get
            {
                _conceptEntityGnossId = $"{GraphsUrl}items/{Identifier}";
                return _conceptEntityGnossId;
            }
        }

        /// <summary>
        /// Gets or sets the base url of the graphs. (example: http://gnoss.com/)
        /// </summary>
        public string GraphsUrl
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the identifier of the parent
        /// </summary>
        public string ParentIdentifier
        {
            get { return _parentNameIdentifier; }
            set
            {
                _parentGnossId = !_rootIdentifier.EndsWith("_") ? $"{_rootIdentifier}_{_parentNameIdentifier}" : $"{_rootIdentifier}{_parentNameIdentifier}";
                _parentGnossId = $"{GraphsUrl}items/{_parentGnossId}";
                _parentNameIdentifier = value;
            }
        }

        /// <summary>
        /// Subentities of this entity
        /// </summary>
        public List<ConceptEntity> Subentities
        {
            get; set;
        }

        /// <summary>
        /// Gets the identifier of this entity
        /// </summary>
        public override string Identifier
        {
            get
            {
                base.Identifier = !_rootIdentifier.EndsWith("_") ? $"{_rootIdentifier}_{_nameIdentifier}" : $"{_rootIdentifier}{_nameIdentifier}";
                _conceptEntityGnossId = $"{GraphsUrl}items/{base.Identifier}";
                return base.Identifier;
            }
        }

        #endregion
    }




}
