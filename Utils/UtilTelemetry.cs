using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Utils
{

    #region Enumerations
    /// <summary>
    /// Location of the information stored by logs and traces
    /// </summary>
    public enum LogsAndTracesLocation
    {
        /// <summary>
        /// Indicates the logs and traces information will be saved in a file
        /// </summary>
        File = 0,
        /// <summary>
        /// Indicates the logs and traces information will be sent to ApplicationInsights
        /// </summary>
        ApplicationInsights = 1,
        /// <summary>
        /// Indicates the logs and traces information will be saved in a file and sent to ApplicationInsights
        /// </summary>
        FileAndAppInsights = 2,
        /// <summary>
        /// 
        /// </summary>
        Logstash = 3,
        /// <summary>
        /// 
        /// </summary>
        FileAndLogstash = 4,
    }
    #endregion

    public class UtilTelemetry
    {
        #region Public Methods
        public static TelemetryClient Telemetry
        {
            get
            {
                TelemetryClient telemetryClient = null;

                if (!string.IsNullOrEmpty(Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey))
                {
                    telemetryClient = new TelemetryClient();
                    //TODO: la versión deberíamos obtenerla de otro ensamblado (algún AD pej.)
                    telemetryClient.Context.Component.Version = typeof(Utils.UtilTelemetry).Assembly.GetName().Version.ToString();
                }
                return telemetryClient;
            }
        }

        //enviar telemetria -> método que compruebe si está configurada la telemetría y que la envíe usando la propiedad Telemetry
        public static void EnviarTelemetriaEvento(string pNombreEvento, Dictionary<string, string> pPropiedades, Dictionary<string, double> pMetricas)
        {
            if (EstaConfiguradaTelemetria)
            {
                if (EstaConfiguradaTelemetria)
                {
                    Telemetry.TrackEvent(pNombreEvento, pPropiedades, pMetricas);
                }
            }
        }

        public static void EnviarTelemetriaTraza(string pMensajeTraza, string pNombreDependencia = null, Stopwatch pReloj = null, bool pExito = false)
        {
            if (EstaConfiguradaTelemetria)
            {
                DateTime horaActual = DateTime.Now;
                TimeSpan duracion = new TimeSpan();

                if (pReloj != null)
                {
                    duracion = pReloj.Elapsed;
                }

                if (string.IsNullOrEmpty(pNombreDependencia))
                {
                    Telemetry.TrackTrace(pMensajeTraza, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
                }
                else
                {
                    Telemetry.TrackDependency(pNombreDependencia, pMensajeTraza, horaActual, duracion, pExito);
                }
            }
        }

        public static void EnviarTelemetriaExcepcion(Exception pExcepcion, string pMensajeExtra, bool pErrorCritico = false)
        {
            if (EstaConfiguradaTelemetria)
            {
                Telemetry.TrackException(pExcepcion);
                SeverityLevel nivelError = SeverityLevel.Information;
                if (pErrorCritico)
                {
                    nivelError = SeverityLevel.Critical;
                }
                Telemetry.TrackTrace(pMensajeExtra, nivelError);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Indica si está configurado el envío de métricas con Application Insights
        /// </summary>
        /// <returns></returns>
        public static bool EstaConfiguradaTelemetria
        {
            get
            {
                return !string.IsNullOrEmpty(UtilTelemetry.InstrumentationKey);
            }
        }

        public static bool ModoDepuracion
        {
            get
            {
                if (TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode.HasValue)
                {
                    return TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode.Value;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = value;
            }
        }

        public static bool EstaActiva
        {
            get
            {
                return TelemetryConfiguration.Active.DisableTelemetry;
            }
            set
            {
                TelemetryConfiguration.Active.DisableTelemetry = value;
            }
        }

        private static string InstrumentationKey
        {
            get
            {
                return Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey;
            }
        }

        #endregion
    }
}
