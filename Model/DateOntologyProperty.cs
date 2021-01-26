using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Represents a date property from an ontology
    /// </summary>
    public class DateOntologyProperty : OntologyProperty
    {
        #region Constructors

        /// <summary>
        /// Constructor of <see cref="DateOntologyProperty"/>.
        /// </summary>
        /// <param name="label">Property predicate</param>
        /// <param name="date">Property value</param>
        public DateOntologyProperty(string label, DateTime date)
        {
            if (date != DateTime.MinValue && date != DateTime.MaxValue)
            {
                Name = label;
                Value = ConvertIntToStringWithLimit(date.Year, 4) + ConvertIntToStringWithLimit(date.Month, 2) + ConvertIntToStringWithLimit(date.Day, 2) + ConvertIntToStringWithLimit(date.Hour, 2) + ConvertIntToStringWithLimit(date.Minute, 2) + ConvertIntToStringWithLimit(date.Second, 2);
            }
        }

        #endregion

        #region Private methods

        private string ConvertIntToStringWithLimit(int number, int limit)
        {
            string num = number.ToString();

            while (num.Length < limit)
            {
                num = $"0{num}";
            }
            return num;
        }

        #endregion

    }
}
