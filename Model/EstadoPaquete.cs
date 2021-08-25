namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    /// Estado del paquete de la carga
    /// </summary>
    public enum EstadoPaquete
    {
        /// <summary>
        /// Pendiente de procesar
        /// </summary>
        Pendiente = 0,

        /// <summary>
        /// El paquete se ha procesado correctamente
        /// </summary>
        Correcto = 1,

        /// <summary>
        /// El paquete ha fallado durante la carga
        /// </summary>
        Erroneo = 2
    }
}
