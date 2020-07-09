using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{

    /// <summary>
    /// Auxiliary entities triples
    /// </summary>
    public class AuxiliaryEntitiesTriplesToInclude
    {

        #region Members

        private string _predicate;

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public AuxiliaryEntitiesTriplesToInclude()
        {

        }

        /// <summary>
        /// Constructor of <see cref="AuxiliaryEntitiesTriplesToInclude"/>
        /// </summary>
        /// <param name="value">Value of the property</param>
        /// <param name="predicate">[Parent predicate]|[children property]</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityIdentifier">Entity identifier</param>
        public AuxiliaryEntitiesTriplesToInclude(string value, string predicate, string entityName, Guid entityIdentifier)
        {
            Value = value;
            Predicate = predicate;
            Name = entityName;

            Identifier = entityIdentifier;

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entity identifier
        /// For example, in http://gnoss.com/items/article_223b30c1-2552-4ed0-ba5f-e257585b08bf_9c126c3a-7850-4cdc-b176-95ae6fd0bb78
        /// the identifier is: 9c126c3a-7850-4cdc-b176-95ae6fd0bb78
        /// </summary>
        public Guid Identifier
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the entity name. 
        /// For example, in http://gnoss.com/items/article_223b30c1-2552-4ed0-ba5f-e257585b08bf_9c126c3a-7850-4cdc-b176-95ae6fd0bb78
        /// the entity name is: article
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value of the predicate
        /// </summary>
        public string Value
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the predicate of the auxiliary entity
        /// </summary>
        public string Predicate
        {
            get { return _predicate; }
            set
            {
                if (value.Contains("|"))
                {
                    _predicate = value;
                }
                else
                {
                    throw new GnossAPIArgumentException("The label must be complete, with complete namespace of the auxiliary entity property + | + complete namespace of the property to load");
                }
                _predicate = value;
            }
        }
    }

    #endregion
}

