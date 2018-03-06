using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gnoss.ApiWrapper.Model;
using System.Web;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Utilities to use strings
    /// </summary>
    public class StringHelper
    {

        #region Members

        private static Regex mReplace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
        private static Regex mReplace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
        private static Regex mReplace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
        private static Regex mReplace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
        private static Regex mReplace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
        private static Regex mReplace_A_Accents = new Regex("[Á|À|Ä|Â]", RegexOptions.Compiled);
        private static Regex mReplace_E_Accents = new Regex("[É|È|Ë|Ê]", RegexOptions.Compiled);
        private static Regex mReplace_I_Accents = new Regex("[Í|Ì|Ï|Î]", RegexOptions.Compiled);
        private static Regex mReplace_O_Accents = new Regex("[Ó|Ò|Ö|Ô]", RegexOptions.Compiled);
        private static Regex mReplace_U_Accents = new Regex("[Ú|Ù|Ü|Û]", RegexOptions.Compiled);
        private static Regex mRegexQuitarHtml = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);

        #endregion

        #region Methods

        /// <summary>
        /// Remove reserved caracters for urls in a string
        /// </summary>
        /// <param name="inputString">String to remove reserved characters</param>
        /// <returns>The inputString without reserved characters</returns>
        public static string RemoveReserverCharactersForUrl(string inputString)
        {
            int languageSeparatorIndex = inputString.IndexOf("|||");

            if (languageSeparatorIndex != -1 && inputString[languageSeparatorIndex - 3] == '@')
            {
                inputString = inputString.Substring(0, languageSeparatorIndex);
                inputString = inputString.Substring(0, inputString.LastIndexOf("@"));
            }

            inputString = RemoveAccentsWithRegEx(inputString);

            Regex notAlfanumeric = new Regex("[^a-zA-Z0-9 ]");

            inputString = notAlfanumeric.Replace(inputString, "");

            while (inputString.IndexOf("  ") > 0)
            {
                inputString = inputString.Replace("  ", " ");
            }

            if (inputString.Length > 50)
            {
                inputString = inputString.Substring(0, 50);

                int ultimoEspacio = inputString.LastIndexOf(" ");
                if (ultimoEspacio > 0)
                {
                    inputString = inputString.Substring(0, ultimoEspacio);
                }
            }

            Regex space = new Regex("[ ]");
            inputString = space.Replace(inputString, "-");

            return inputString.ToLower();
        }

        /// <summary>
        /// Remove accents from the input string
        /// </summary>
        /// <param name="pInputString">String to remove accents</param>
        /// <returns>InputString without accents</returns>
        public static string RemoveAccentsWithRegEx(string pInputString)
        {
            pInputString = mReplace_a_Accents.Replace(pInputString, "a");
            pInputString = mReplace_e_Accents.Replace(pInputString, "e");
            pInputString = mReplace_i_Accents.Replace(pInputString, "i");
            pInputString = mReplace_o_Accents.Replace(pInputString, "o");
            pInputString = mReplace_u_Accents.Replace(pInputString, "u");
            pInputString = mReplace_A_Accents.Replace(pInputString, "A");
            pInputString = mReplace_E_Accents.Replace(pInputString, "E");
            pInputString = mReplace_I_Accents.Replace(pInputString, "I");
            pInputString = mReplace_O_Accents.Replace(pInputString, "O");
            pInputString = mReplace_U_Accents.Replace(pInputString, "U");

            return pInputString;
        }

        /// <summary>
        /// Gets a url encoded as UTF-8
        /// </summary>
        /// <param name="url">Url to encode</param>
        /// <returns></returns>
        public static string UrlEncoderUTF8(string url)
        {
            return (HttpUtility.UrlEncode(url, UTF8Encoding.UTF8));
        }

        #endregion
    }
}
