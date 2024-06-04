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
    /// Triples to be included
    /// </summary>
    public class TriplesToInclude
    {

        #region Members

        private string _predicate;

        #endregion

        #region Constructors

        /// <summary>
        /// Empry constructor
        /// </summary>
        public TriplesToInclude()
        {
            Title = false;
            Description = false;
        }

        /// <summary>
        /// Creates a new object to insert a new property in a resource
        /// </summary>
        /// <param name="value">Value of the new property</param>
        /// <param name="predicate">Predicate of the property to be inserted, with namespace without prefix. If it's an auxiliary entity property, you must set the predicate as firstLevelPredicate|secondLevelPredicate</param>
        /// <param name="title">If the property inserted is the title, set this property to TRUE</param>
        /// <param name="description">If the property inserted is the description, set this property to TRUE</param>
        public TriplesToInclude(string value, string predicate, bool title = false, bool description = false)
        {
            NewValue = value;
            Predicate = predicate;

            Title = title;
            Description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the new value of the property
        /// </summary>
        public string NewValue
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the predicate of the property to be inserted, with namespace without prefix. If it's an auxiliary entity property, you must set the predicate as firstLevelPredicate|secondLevelPredicate
        /// </summary>
        public string Predicate
        {
            get { return _predicate; }
            set
            {
                _predicate = value;
            }
        }

        /// <summary>
        /// If the property inserted is the title, set this property to TRUE
        /// </summary>
        public bool Title
        {
            get; set;
        }

        /// <summary>
        /// If the property inserted is the description, set this property to TRUE
        /// </summary>
        public bool Description
        {
            get; set;
        }

        #endregion
    }
}
