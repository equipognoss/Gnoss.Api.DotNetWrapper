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
    /// Represents a date in GNOSS
    /// </summary>
    public class GnossDate
    {
        #region Members

        private string _millenniumRoman;
        private string _centuryRoman;
        private string _day;
        private string _month;
        private string _year;

        #region Patterns

        private string _patternCenturyStartByS = @"^S[\.]*"; // century or century range
        private string _patternContainsRomano = @"^[IVXLCDMivxlcdm]+";
        private string _patternDateTypeAC = @"a[\.]*C[\.]*";
        private string _patternDateTypeDC = @"d[\.]*C[\.]*";
        private string _patternDateTypeBP = @"B[\.]*P[\.]*";
        private string _patternDateAccurancyTypeAprox = @"aprox\s*\.*";
        private string _patternDateAccurancyCA = "(\\[)+ca(\\])+";
        private string _patternDateAccurancyDoubtful = @"\¿|\?";
        private string _patternMillennium = @"milenio";
        private string _patternDDMMYYYY = @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{3,4})$";
        private string _patternMMDDYYYY = @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0-9]{3,4})$";
        private string _patternMMYYYY = @"^([0]?[1-9]|[1][0-2])[./-]([0-9]{3,4})$";
        private string _patternYYYY = @"^([0-9]{1,4})$";
        private string _patternYYYYMMDD = @"^([0-9]{3,4})[./-]([0]?[1-9]|[1][0-2])[./-]([0]?[1-9]|[1|2][0-9]|[3][0|1])$";
        private string _patternYYYYMM = @"^([0-9]{3,4})[./-]([0]?[1-9]|[1][0-2])$";
        private string _patternDDLettersMonthYYYY = @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[\s]*[de]*[\s]*(Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre)[\s]*[de|del]*[\s]*([0-9]{3,4})$";

        private string _patternLettersMonthDDYYYY = @"^((Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre)*[0]?[1-9]|[1|2][0-9]|[3][0|1])[\s][de]*[\s]*[\s]*[de|del]*[\s]*([0-9]{3,4})$";

        private string _patternLettersMonthYYYY = @"^(Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre)[\s]*[de|del]*[\s]*([0-9]{3,4})$";
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="GnossDate"/>.
        /// </summary>
        public GnossDate()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor of <see cref="GnossDate"/>
        /// </summary>
        /// <param name="date">A date</param>
        /// <param name="americanFormat">(Optional) True if the date is in American format</param>
        public GnossDate(string date, bool americanFormat = false)
        {
            Initialize();

            this.TypeDate = GetDateType(date);
            date = CleanDateType(date);

            // Date accurancy
            this.PrecisionDate = GetDateAccurancy(date);
            date = CleanDateAccurancy(date);


            NormalizeDate(date, americanFormat);

        }
        #endregion

        #region Private methods

        private void NormalizeDate(string date, bool americanFormat)
        {
            Regex regex = null;
            if (americanFormat)
            {
                _patternDDMMYYYY = _patternMMDDYYYY;
                _patternDDLettersMonthYYYY = _patternLettersMonthDDYYYY;
            }
            regex = new Regex(_patternDDMMYYYY, RegexOptions.IgnoreCase);
            if (Regex.IsMatch(date, _patternDDMMYYYY, RegexOptions.IgnoreCase))
            {
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    if (americanFormat)
                    {
                        Month = m.Groups[1].Value;
                        Day = m.Groups[2].Value;
                        Year = m.Groups[3].Value;
                    }
                    else
                    {
                        Day = m.Groups[1].Value;
                        Month = m.Groups[2].Value;
                        Year = m.Groups[3].Value;
                    }
                }
            }
            else if (Regex.IsMatch(date, _patternMMYYYY, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternMMYYYY, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    Month = m.Groups[1].Value;
                    Year = m.Groups[2].Value;
                }
            }
            else if (Regex.IsMatch(date, _patternYYYY, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternYYYY, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    Year = m.Groups[1].Value;
                }
            }
            else if (Regex.IsMatch(date, _patternYYYYMM, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternYYYYMM, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    Year = m.Groups[1].Value;
                    Month = m.Groups[2].Value;
                }
            }
            else if (Regex.IsMatch(date, _patternYYYYMMDD, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternYYYYMMDD, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    Year = m.Groups[1].Value;
                    Month = m.Groups[2].Value;
                    Day = m.Groups[3].Value;
                }
            }
            else if (Regex.IsMatch(date, _patternDDLettersMonthYYYY, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternDDLettersMonthYYYY, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    if (americanFormat)
                    {
                        Month = m.Groups[1].Value;
                        Day = m.Groups[2].Value;
                        Year = m.Groups[3].Value;
                    }
                    else
                    {
                        Day = m.Groups[1].Value;
                        Month = m.Groups[2].Value;
                        Year = m.Groups[3].Value;
                    }
                }
            }
            else if (Regex.IsMatch(date, _patternLettersMonthYYYY, RegexOptions.IgnoreCase))
            {
                regex = new Regex(_patternLettersMonthYYYY, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(date);
                foreach (Match m in mc)
                {
                    Month = m.Groups[1].Value;
                    Year = m.Groups[2].Value;
                }
            }
            else if (Regex.IsMatch(date, _patternMillennium, RegexOptions.IgnoreCase))
            {
                date = CleanMillennium(date);
                MillenniumRoman = date;
            }
            else if (Regex.IsMatch(date, _patternCenturyStartByS, RegexOptions.IgnoreCase) || Regex.IsMatch(date, _patternContainsRomano, RegexOptions.IgnoreCase))
            {
                date = CleanCentury(date);
                CenturyRoman = date;
            }
            else
            {
                throw new GnossAPIDateException($"Couldn't normalize date: {date}");
            }
        }

        private void Initialize()
        {
            _day = "00";
            _month = "00";
            _year = "0000";
            Hours = "00";
            Minutes = "00";
            Seconds = "00";

            Century = 0;
            Millennium = 0;

            _millenniumRoman = null;
            NormDate = null;
            PrecisionDate = null;
            TypeDate = null;
            _centuryRoman = null;
        }

        /// <author>Lorena Ruiz Elósegui</author>
        /// <datecreated>2014-01-08</datecreated>
        /// <summary>
        /// Converts a roman number to decimal number
        /// 
        /// Code gets from http://my.opera.com/RAJUDASA/blog/2013/04/08/c-convert-roman-numbers-to-decimal-numbers
        /// 
        /// Explicación:
        /// code process:
        /// convert string to char array.
        /// convert each char (roman number) to its equivalent decimal number and stores them in array.
        /// Iterating over array, check whether low number is left side next to high number if yes, remove that low number from high number else add both numbers.
        /// Assumes only one low number comes left side and max 3 low numbers right side at a time of high number.(ex: IV and VIII)
        /// 
        /// Try invoking the above method with these sample Roman Numbers:
        /// "MMXIII" -> 2013; DCCCXLI -> 841; MCMLXVII -> 1967; XIX -> 19
        /// </summary>
        /// <param name="romanNum"></param>
        /// <returns></returns>
        private int RomanToDecimal(string romanNum)
        {
            int[] ints = romanNum.ToCharArray().Select(x => { return x == 'I' ? 1 : x == 'V' ? 5 : x == 'X' ? 10 : x == 'L' ? 50 : x == 'C' ? 100 : x == 'D' ? 500 : x == 'M' ? 1000 : 0; }).ToArray();
            int calc = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                int mult = (i + 1 < ints.Length && ints[i + 1] > ints[i]) ? -1 : 1;
                calc += mult * ints[i];
            }
            return calc;
        }

        private string CompleteStringZerosAtLeftWithLimit(string number, int limit)
        {
            while (number.Length < limit)
            {
                number = $"0{number}";
            }
            return number;
        }

        private void UpdateNormalizeDate()
        {
            NormDate = Year + Month + Day + Hours + Minutes + Seconds;
            if (this.TypeDate != null && this.TypeDate.Equals(DateTypes.AC))
            {
                NormDate = $"-{NormDate}";
            }
        }

        #region Extraction and cleaning methods

        private string GetDateType(string text)
        {
            string dateType = null;
            bool ac = Regex.IsMatch(text, _patternDateTypeAC, RegexOptions.IgnoreCase);
            bool dc = Regex.IsMatch(text, _patternDateTypeDC, RegexOptions.IgnoreCase);
            bool bp = Regex.IsMatch(text, _patternDateTypeBP, RegexOptions.IgnoreCase);
            if (ac)
            {
                dateType = DateTypes.AC;
            }
            else if (dc)
            {
                dateType = DateTypes.DC;
            }
            else if (bp)
            {
                dateType = DateTypes.BP;
            }
            else if (text.StartsWith("-"))
            {
                dateType = DateTypes.AC;
            }
            return dateType;
        }

        private string GetDateAccurancy(string text)
        {
            string accurancy = null;
            bool aprox = Regex.IsMatch(text, _patternDateAccurancyTypeAprox, RegexOptions.IgnoreCase) || Regex.IsMatch(text, _patternDateAccurancyCA, RegexOptions.IgnoreCase);
            bool dudosa = Regex.IsMatch(text, _patternDateAccurancyDoubtful, RegexOptions.IgnoreCase);

            if (aprox)
            {
                accurancy = "ca";
            }
            else if (dudosa)
            {
                accurancy = "?";
            }
            return accurancy;
        }

        private string CleanMillennium(string text)
        {
            Regex regex = new Regex(_patternMillennium, RegexOptions.IgnoreCase);
            while (Regex.IsMatch(text, _patternMillennium, RegexOptions.IgnoreCase))
            {
                MatchCollection mc = regex.Matches(text);

                int mIdx = 0;
                foreach (Match m in mc)
                {
                    for (int gIdx = 0; gIdx < m.Groups.Count; gIdx++)
                    {
                        text = text.Replace(m.Groups[gIdx].Value.Trim(), "").Trim();
                    }
                    mIdx++;
                }
            }
            return text;
        }

        private string CleanCentury(string text)
        {
            Regex regex = new Regex(_patternCenturyStartByS, RegexOptions.IgnoreCase);
            while (Regex.IsMatch(text, _patternCenturyStartByS, RegexOptions.IgnoreCase))
            {
                MatchCollection mc = regex.Matches(text);

                int mIdx = 0;
                foreach (Match m in mc)
                {
                    for (int gIdx = 0; gIdx < m.Groups.Count; gIdx++)
                    {
                        text = text.Replace(m.Groups[gIdx].Value.Trim(), "").Trim();
                    }
                    mIdx++;
                }
            }
            return text;
        }

        private string CleanDateType(string text)
        {
            List<string> dateTypePatterns = new List<string>() { _patternDateTypeAC, _patternDateTypeBP, _patternDateTypeDC };
            foreach (string dateTypePattern in dateTypePatterns)
            {
                Regex regex = new Regex(dateTypePattern, RegexOptions.IgnoreCase);
                if (Regex.IsMatch(text, dateTypePattern, RegexOptions.IgnoreCase))
                {
                    MatchCollection mc = regex.Matches(text);

                    int mIdx = 0;
                    foreach (Match m in mc)
                    {
                        for (int gIdx = 0; gIdx < m.Groups.Count; gIdx++)
                        {
                            text = text.Replace(m.Groups[gIdx].Value.Trim(), "").Trim();
                        }
                        mIdx++;
                    }
                }
            }
            if (text.StartsWith("-"))
            {
                text = text.Substring(1);
            }
            return text;
        }

        private string CleanDateAccurancy(string text)
        {
            List<string> dateAccurancyPatterns = new List<string>() { _patternDateAccurancyTypeAprox, _patternDateAccurancyDoubtful, _patternDateAccurancyCA };
            foreach (string dateAccurancyPattern in dateAccurancyPatterns)
            {
                Regex regex = new Regex(dateAccurancyPattern, RegexOptions.IgnoreCase);
                if (Regex.IsMatch(text, dateAccurancyPattern, RegexOptions.IgnoreCase))
                {
                    MatchCollection mc = regex.Matches(text);

                    int mIdx = 0;
                    foreach (Match m in mc)
                    {
                        for (int gIdx = 0; gIdx < m.Groups.Count; gIdx++)
                        {
                            text = text.Replace(m.Groups[gIdx].Value.Trim(), "").Trim();
                        }
                        mIdx++;
                    }
                }
            }
            return text;
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Gets a <see cref="GnossDate"/> as a string
        /// </summary>
        /// <returns>String with the information of this <see cref="GnossDate"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("---------- GnossDate ------------");
            sb.AppendLine($"Millennium: {Millennium}");
            sb.AppendLine($"Century: {Century}");
            sb.AppendLine($"Date type: {TypeDate}");
            sb.AppendLine($"Accurancy: {PrecisionDate}");
            sb.AppendLine($"Normalized date: {NormDate}");
            sb.AppendLine("----------------------------------");

            return sb.ToString();
        }

        /// <summary>
        /// Gets if this Date is not initialized
        /// </summary>
        /// <returns><c>true</c> if the date is empty, <c>false</c> in another case</returns>
        public bool IsEmpty()
        {
            return Millennium == 0 && Century == 0 && NormDate == null && Day.Equals("00") && Month.Equals("00") && Year.Equals("0000") && Hours.Equals("00") && Minutes.Equals("00") && Seconds.Equals("00") && PrecisionDate == null && TypeDate == null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the millennium
        /// </summary>
        public int Millennium
        {
            get; set;
        }

        /// <summary>
        ///Gets or sets the century
        /// </summary>
        public int Century
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the millennium in roman numbers
        /// </summary>
        public string MillenniumRoman
        {
            get { return _millenniumRoman; }
            set
            {
                _millenniumRoman = value;
                Millennium = RomanToDecimal(_millenniumRoman);
            }
        }

        /// <summary>
        /// Gets or sets the normalized date with the pattern yyyyMMddhhmmss.
        /// </summary>
        public string NormDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the accurancy
        /// </summary>
        public string PrecisionDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the type of date
        /// </summary>
        public string TypeDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the century in roman numbers
        /// </summary>
        public string CenturyRoman
        {
            get { return _centuryRoman; }
            set
            {
                _centuryRoman = value;
                Century = RomanToDecimal(_centuryRoman);

            }
        }

        /// <summary>
        /// Gets or sets the day (2 digits)
        /// </summary>
        public string Day
        {
            get { return _day; }
            set
            {
                _day = CompleteStringZerosAtLeftWithLimit(value, 2);
                UpdateNormalizeDate();
            }
        }

        /// <summary>
        /// Gets or sets the month (2 digits)
        /// </summary>
        public string Month
        {
            get { return _month; }
            set
            {
                if (value.ToLower().Equals("enero"))
                {
                    _month = "01";
                }
                else if (value.ToLower().Equals("febrero"))
                {
                    _month = "02";
                }
                else if (value.ToLower().Equals("marzo"))
                {
                    _month = "03";
                }
                else if (value.ToLower().Equals("abril"))
                {
                    _month = "04";
                }
                else if (value.ToLower().Equals("mayo"))
                {
                    _month = "05";
                }
                else if (value.ToLower().Equals("junio"))
                {
                    _month = "06";
                }
                else if (value.ToLower().Equals("julio"))
                {
                    _month = "07";
                }
                else if (value.ToLower().Equals("agosto"))
                {
                    _month = "08";
                }
                else if (value.ToLower().Equals("septiembre"))
                {
                    _month = "09";
                }
                else if (value.ToLower().Equals("octubre"))
                {
                    _month = "10";
                }
                else if (value.ToLower().Equals("noviembre"))
                {
                    _month = "11";
                }
                else if (value.ToLower().Equals("diciembre"))
                {
                    _month = "12";
                }
                else
                {
                    // No es un mes escrito en letra
                    _month = CompleteStringZerosAtLeftWithLimit(value, 2);
                }

                UpdateNormalizeDate();
            }
        }

        /// <summary>
        /// Gets or sets the year (4 digits)
        /// </summary>
        public string Year
        {
            get { return _year; }
            set
            {
                _year = CompleteStringZerosAtLeftWithLimit(value, 4);
                UpdateNormalizeDate();
            }
        }

        /// <summary>
        /// Gets or sets the hour (2 digits)
        /// </summary>
        public string Hours
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the minutes (2 digits)
        /// </summary>
        public string Minutes
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the seconds (2 digits)
        /// </summary>
        public string Seconds
        {
            get; set;
        }

        #endregion
    }
}