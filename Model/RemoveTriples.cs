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
    /// Class to remove triples from resources
    /// </summary>
    public class RemoveTriples
    {

        #region Members

        private string _predicate;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public RemoveTriples()
        {
            Title = false;
            Description = false;
        }

        /// <summary>
        /// Remove the triples from a resource without attached files
        /// </summary>
        /// <param name="value">Current value of the property to delete</param>
        /// <param name="predicate">Predicate of the property to delete. If it's a property of an auxiliary entity, the correct syntax is firstLevelPredicate|secondLevelPredicate</param>
        public RemoveTriples(string value, string predicate)
        {
            Value = value;
            Predicate = predicate;
        }


        /// <summary>
        /// Remove the triples from a resource with attached files
        /// </summary>
        /// <param name="value">Current value of the property to delete</param>
        /// <param name="predicate">Predicate of the property to delete. If it's a property of an auxiliary entity, the correct syntax is firstLevelPredicate|secondLevelPredicate</param>
        /// <param name="objectType">Objet type. It can be image or attached file</param>
        public RemoveTriples(string value, string predicate, short objectType)
        {
            Predicate = predicate;
            Value = value;
            ObjectType = objectType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current value of the property to delete
        /// </summary>
        public string Value
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the predicate of the property to delete. If it's a property of an auxiliary entity, the correct syntax is firstLevelPredicate|secondLevelPredicate
        /// </summary>
        public string Predicate
        {
            get { return _predicate; }
            set
            {
                if (value.Contains("http:"))
                {
                    _predicate = value;
                }
                else
                {
                    throw new GnossAPIException("the predicate must be a complete uri, not a property with namespace");
                }
            }
        }

        /// <summary>
        /// Gets or sets the resource object type
        /// </summary>
        public short ObjectType
        {
            get; set;
        }


        /// <summary>
        /// Gets or sets the resource title
        /// </summary>
        public bool Title
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the resource description
        /// </summary>
        public bool Description
        {
            get; set;
        }

        #endregion

    }
}
