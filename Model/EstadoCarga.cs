namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Estado del paquete de la carga
    /// </summary>
    public enum EstadoCarga
    {
        /// <summary>
        /// Aun no se ha comenzado la carga
        /// </summary>
        Pendiente = 0,

        /// <summary>
        /// La carga ha sido correcta
        /// </summary>
        Correcto = 1,

        /// <summary>
        /// Ha fallado algún paquete durante la carga
        /// </summary>
        Erroneo = 2,

        /// <summary>
        /// La carga esta siendo procesada
        /// </summary>
        EnProceso = 3
    }
}