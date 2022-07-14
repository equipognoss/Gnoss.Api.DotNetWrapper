using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a property from an ontology
    /// </summary>
    public class OntologyProperty
    {
        #region Const

        private const string _languagePattern = @"^[a-z]{2,2}$";

        #endregion

        #region Members 

        private string _language;
        

        #endregion

        #region Public methods
               
        /// <summary>
        /// Gets the HashCode of the property
        /// </summary>
        /// <returns>HasCode</returns>
        public override int GetHashCode()
        {
            int hashOntoloyPropertyName = Name == null ? 0 : Name.GetHashCode();
            int hashOntologyPropertyValue = Value == null ? 0 : Value.GetHashCode();
            int hashOntologyPropertyLanguage = Language == null ? 0 : Language.GetHashCode();

            return hashOntoloyPropertyName ^ hashOntologyPropertyValue ^ hashOntologyPropertyLanguage;
        }

        /// <summary>
        /// Returns true if <paramref name="pProperty"/> is equals to the current property
        /// </summary>
        /// <param name="pProperty"><see cref="OntologyProperty"/> property to compare</param>
        /// <returns><c>true</c> if <paramref name="pProperty"/> is equals to the current property, <c>false</c> in another case</returns>
        public override bool Equals(object pProperty)
        {
            if ((pProperty == null) || !GetType().Equals(pProperty.GetType()))
            {
                return false;
            }
            else
            {
                OntologyProperty ontologyPropertyParam = (OntologyProperty) pProperty;
                return ontologyPropertyParam.Name == Name && ontologyPropertyParam.Value.ToString() == Value.ToString() && ontologyPropertyParam.Language == Language;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value
        /// </summary>
        public object Value
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the language property (en, es...)
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != null)
                {
                    if (Regex.IsMatch(_language, _languagePattern, RegexOptions.None))
                    {
                        _language = value;
                    }
                    else
                    {
                        throw new GnossAPIArgumentException($"The language sets in the property {Name} is not valid. It must have only two letters (example: en, es...)", "Language");
                    }
                }
                else
                {
                    _language = value;
                }
            }
        }

        #endregion
    }
}
