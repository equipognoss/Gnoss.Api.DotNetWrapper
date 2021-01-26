using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Utilities to use RDF 
    /// </summary>
    public class RDFHelper
    {
        
        /// <summary>
        /// Returns a string with the value of the label localName in xelm
        /// </summary>
        /// <param name="xelt">RDF elements</param>
        /// <param name="localName">Label to get the value</param>
        /// <param name="nameSpaceName">(optional) the namespace of the label, if necesary</param>
        /// <param name="filterAttribute">(optional) XName of the attribute to filter by<example>For the attribute xml:lang, XName.Get("{http://www.w3.org/XML/1998/namespace}lang")</example></param>
        /// <param name="filterAttributeValue">(optional) Value of the filterAttribute</param>
        /// <returns>The value of the label localName</returns>
        public static string getElementValue(IEnumerable<XElement> xelt, string localName, string nameSpaceName = null, XName filterAttribute = null, string filterAttributeValue = "")
        {
            try
            {
                if (nameSpaceName != null)
                {
                    if (xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName) && elto.Name.NamespaceName.Equals(nameSpaceName)).Count() > 0)
                    {
                        if (filterAttribute == null && filterAttributeValue == "")
                        {
                            return xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName) && elto.Name.NamespaceName.Equals(nameSpaceName)).ToList()[0].Value.ToString();
                        }
                        else if (filterAttribute != null && filterAttributeValue != "")
                        {

                            return xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName) && elto.Name.NamespaceName.Equals(nameSpaceName)).ToList().Where(lt => lt.Attribute(filterAttribute).Value == filterAttributeValue).Select(s => s.Value).ToList()[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName)) != null && xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName)).Count() > 0)
                    {
                        if (filterAttribute == null && filterAttributeValue == "")
                        {
                            return xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName)).ToList()[0].Value.ToString();
                        }
                        else if (filterAttribute != null && filterAttributeValue != "")
                        {
                            return xelt.Elements().Where(elto => elto.Name.LocalName.Equals(localName)).ToList().Where(lt => lt.Attribute(filterAttribute).Value == filterAttributeValue).Select(s => s.Value).ToList()[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a string with the value of the label localName in xelm
        /// </summary>
        /// <param name="elto">RDF element</param>
        /// <param name="localName">Label to get the value</param>
        /// <param name="nameSpaceName">(optional) the namespace of the label, if necesary</param>
        /// <returns></returns>
        public static string getAttributeValue(XElement elto, string localName, string nameSpaceName = null)
        {
            if (elto.HasAttributes)
            {
                try
                {
                    return elto.Attributes().Where(atributo => atributo.Name.LocalName.Equals(localName) && atributo.Name.NamespaceName.Equals(nameSpaceName)).First().Value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
