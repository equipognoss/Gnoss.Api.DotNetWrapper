using Gnoss.ApiWrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.Interfaces
{
    /// <summary>
    /// Interface with the methods of the Ontology class
    /// </summary>
    public interface IGnossOCBase
    {
        /// <summary>
        /// Generates the ontology graph triples
        /// </summary>
        /// <param name="pResourceApi">Api resource</param>
        /// <returns>List with the ontology triples</returns>
        List<string> ToOntologyGnossTriples(ResourceApi pResourceApi);
        /// <summary>
        /// Generates the search graph triples
        /// </summary>
        /// <param name="pResourceApi">Api resource</param>
        /// <returns>List with the search graph triples</returns>
        List<string> ToSearchGraphTriples(ResourceApi pResourceApi);
        /// <summary>
        /// Generates the sql server list dates
        /// </summary>
        /// <param name="resourceAPI">Api resource</param>
        /// <returns>The id of the document and the necessary dates for insert in sql server</returns>
        KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI);
        /// <summary>
        /// Generates the object's URI
        /// </summary>
        /// <param name="resourceAPI">Api Resource</param>
        /// <returns>The object´s URI</returns>
        string GetURI(ResourceApi resourceAPI);
        int GetID();

        /// <summary>
        /// Generate a resource to load/modify in GNOSS
        /// </summary>
        /// <param name="resourceAPI">Api Resource</param>
        /// <returns></returns>
        BaseResource ToGnossApiResource(ResourceApi resourceAPI);
    }
}
