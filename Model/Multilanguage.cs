using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// It's used to represent the language in the title and description of the MultilanguageTitle of a resource
    /// </summary>
    public class Multilanguage
    {
        /// <summary>
        /// Gets or sets the multilanguage string
        /// </summary>
        public string String
        {
            get; set;
        }        

        /// <summary>
        /// Gets or sets the laguage of the string
        /// </summary>
        public string Language
        {
            get; set;
        }

        /// <summary>
        /// Constructor of <see cref="Multilanguage"/>
        /// </summary>
        /// <param name="multiLanguageString">Multilanguage string</param>
        /// <param name="language">Language of the string</param>
        public Multilanguage(string multiLanguageString, string language)
        {
            String = multiLanguageString;
            Language = language;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Multilanguage(){}
        
    }
}
