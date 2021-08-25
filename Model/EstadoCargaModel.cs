
namespace Gnoss.ApiWrapper.Model
{
    public class EstadoCargaModel
    {
        public bool Cerrado { get; set; }
        public EstadoCarga EstadoCarga { get; set; }
        public int NumPaquetesPendientes { get; set; }
        public int NumPaquetesCorrectos { get; set; }
        public int NumPaquetesErroneos { get; set; }
    }
}
