using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.Apiwrapper.ApiModel
{
    public partial class Rol
    {
        public Rol()
        {
        }
        public Guid RolID { get; set; }
        public string Nombre { get; set; }
        public List<PermisoModel> ListaPermisos { get; set; }
    }

    public partial class PermisoModel
    {
        public string Nombre { get; set; }

        public string Seccion { get; set; }

        public bool Concedido { get; set; }
    }
}
