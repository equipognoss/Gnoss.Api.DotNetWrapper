using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.ApiModel
{
    /// <summary>
    /// Class that represents a SparqlObject
    /// </summary>
    public class SparqlObject
    {

        /// <summary>
        /// Head of the sparql query
        /// </summary>
        public Head head { get; set; }
        /// <summary>
        /// Results of the sparql query
        /// </summary>
        public Results results { get; set; }

        /// <summary>
        /// Head of the query
        /// </summary>
        public class Head
        {
            /// <summary>
            /// Links of the query
            /// </summary>
            public List<object> link { get; set; }
            /// <summary>
            /// Vars used in the query
            /// </summary>
            public List<string> vars { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Data
        {
            public string type { get; set; }
            public string value { get; set; }
            public string datatype { get; set; }
        }

        /// <summary>
        /// Results of the query
        /// </summary>
        public class Results
        {
            /// <summary>
            /// If the distinct param is applied or no
            /// </summary>
            public bool distinct { get; set; }
            /// <summary>
            /// If the results will be ordered
            /// </summary>
            public bool ordered { get; set; }
            /// <summary>
            /// Results of the query
            /// </summary>
            public List<Dictionary<string, Data>> bindings { get; set; }
        }

    }
}
