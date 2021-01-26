using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.Helpers
{
    class DatosRequest
    {
        public string HttpMethod { get; set; }
        public string RawURL { get; set; }
        public string URL { get; set; }
        public string Domain { get; set; }
        public string UserAgent { get; set; }
        public string Ip { get; set; }
        public string CookieSessionId { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
    }
}
