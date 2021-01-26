using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gnoss.ApiWrapper.Helpers;

namespace Gnoss.ApiWrapper.Model
{

    /// <summary>
    /// Triples to be modified
    /// </summary>
    public class TriplesToModify : TriplesToInclude
    {

        #region Constructors 

        /// <summary>
        /// Empry constructor
        /// </summary>
        public TriplesToModify()
        {
        }

        /// <summary>
        /// Creates a new object to modify a property in a resource
        /// </summary>
        /// <param name="newValue">New value of the property</param>
        /// <param name="predicate">Predicate of the property to be modified, with namespace without prefix. If it's an auxiliary entity property, you must set the predicate as firstLevelPredicate|secondLevelPredicate</param>
        /// <param name="title">If the property to be modified is the title, set this property to TRUE</param>
        /// <param name="description">If the property to be modified is the description, set this property to TRUE</param>
        /// <param name="oldValue">Current value of the property</param>
        public TriplesToModify(string newValue, string oldValue, string predicate, bool title=false, bool description= false) : base(newValue, predicate, title, description)
        {
            OldValue = oldValue;
        }

        #endregion

        #region Properties 

        /// <summary>
        /// Gets or sets the old value of the property
        /// </summary>
        public string OldValue
        {
            get; set;
        }

        #endregion
    }
}
