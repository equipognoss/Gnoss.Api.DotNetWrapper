using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using Gnoss.ApiWrapper.Exceptions;
using System.Threading;
using System.Runtime.CompilerServices;
using static Es.Riam.Util.UtilTelemetry;
using Es.Riam.Util;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Helper to save logs an traces
    /// 
    /// The file generated will always have the creation date before the name. Example: 2015_05_17_error.log
    /// 
    /// There are seven posible levels: <br />
    ///  <br />   
    ///     - <c>FATAL</c>: Critical system messages. The application must be aborted after a fatal error<br />
    ///     - <c>ERROR</c>: Errors produced by the API or an unexpected API response<br />
    ///     - <c>WARN</c>: Something goes wrong, but it's not too important<br />
    ///     - <c>INFO</c>: Verbose mode. Information message about the application state<br />
    ///     - <c>DEBUG</c>: Only for development enviroments. Generate message for every API comunication<br />
    ///     - <c>TRACE</c>: Adds more message to DEBUG mode, with information about the request and response data<br />
    ///     - <c>OFF</c>: Log disabled<br />
    /// <br />
    /// 
    /// Class without constructor, following the singleton pattern. To use it, use the static property Instance. 
    /// </summary>
    public class LogApplicationInsightsHelper: ILogHelper
    {

        #region Static members
        
        private static string _logDirectory;

        #endregion

        #region Members

        private bool _isActivated;

        #endregion

        #region Constructors

        static LogApplicationInsightsHelper()
        {
            // Defult log level: Warning. Writes Fatal, Error and Warning messages. 
            LogHelper.LogLevel = LogLevels.WARN;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        public LogApplicationInsightsHelper()
        {
            if (string.IsNullOrEmpty(LogHelper.LogDirectory))
            {
                _isActivated = false;
                Console.WriteLine("Warning: The log has not been configured");
            }
            else
            {
                if (string.IsNullOrEmpty(LogHelper.LogFileName))
                {
                    LogHelper.LogFileName = "gnoss_api.log";
                }
                _isActivated = true;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Write a trace log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Trace(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.TRACE)
            {
                Write(LogLevels.TRACE, className, memberName, message);
            }
        }

        /// <summary>
        /// Write a debug log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Debug(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.DEBUG)
            {
                Write(LogLevels.DEBUG, className, memberName, message);
            }
        }

        /// <summary>
        /// Write a information log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Info(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.INFO)
            {
                Write(LogLevels.INFO, className, memberName, message);
            }
        }

        /// <summary>
        /// Write a warning log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Warn(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.WARN)
            {
                Write(LogLevels.WARN, className, memberName, message);
            }
        }

        /// <summary>
        /// Write a error log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Error(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.ERROR)
            {
                Write(LogLevels.ERROR, className, memberName, message);
            }
        }

        /// <summary>
        /// Write a fatal log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        public void Fatal(string message, string className = "", [CallerMemberName] string memberName = "")
        {
            if (LogHelper.LogLevel <= LogLevels.FATAL)
            {
                Write(LogLevels.FATAL, className, memberName, message);
            }
        }

        /// <summary>
        /// Set the log location
        /// </summary>
        /// <param name="pLocation"></param>
        public static void SetLogLocation(int pLocation)
        {
            if (Enum.IsDefined(typeof(UtilTelemetry.LogsAndTracesLocation), pLocation))
            {
                LogLocation = (UtilTelemetry.LogsAndTracesLocation)pLocation;
            }
        }

        /// <summary>
        /// Set the application insights implementation key
        /// </summary>
        /// <param name="pImplementationKey"></param>
        public static void SetImplementationKey(string pImplementationKey)
        {
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = pImplementationKey;
            ImplementationKey = pImplementationKey;
        }

        /// <summary>
        /// Obtains the Application Insights configuration
        /// </summary>
        public static void LeerConfiguracionApplicationInsights(string pFicheroOAuth)
        {
            if (UsarVariablesEntorno && UsarApplicationInsights)
            {
                string implementationKey = Environment.GetEnvironmentVariable("implementationKey");
                string logLocation = Environment.GetEnvironmentVariable("logLocation");

                if (!string.IsNullOrEmpty(implementationKey))
                {
                    Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = implementationKey;
                    ImplementationKey = implementationKey;
                }
                else
                {
                    throw new Exception("The specified environment variable doesn't exist: implementationKey");
                }

                if (!string.IsNullOrEmpty(logLocation))
                {
                    int valorInt = 0;
                    if (int.TryParse(logLocation, out valorInt))
                    {
                        if (Enum.IsDefined(typeof(LogsAndTracesLocation), valorInt))
                        {
                            LogLocation = (LogsAndTracesLocation)valorInt;
                        }
                    }
                    else
                    {
                        throw new Exception("The specified environment variable doesn't properly configured: logLocation");
                    }
                }
                else
                {
                    throw new Exception("The specified environment variable doesn't exist: logLocation");
                }
            }
            else
            {
                if (File.Exists(pFicheroOAuth))
                {
                    XmlDocument docXml = new XmlDocument();
                    docXml.Load(pFicheroOAuth);

                    XmlNode implementationKeyNode = docXml.SelectSingleNode("config/applicationInsights/implementationKey");

                    if (implementationKeyNode != null)
                    {
                        string implementationKey = implementationKeyNode.InnerText.ToLower();

                        if (!string.IsNullOrEmpty(implementationKey))
                        {
                            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = implementationKey;
                            ImplementationKey = implementationKey;
                        }
                        else
                        {
                            throw new Exception("El nodo implementationKey no puede ser vacío");
                        }
                    }
                    else
                    {
                        throw new Exception("No se ha Configurado el nodo implementationKey");
                    }

                    XmlNode logsLocationNode = docXml.SelectSingleNode("config/applicationInsights/logLocation");

                    if (logsLocationNode != null)
                    {
                        string logsLocation = logsLocationNode.InnerText;

                        int valorInt = 0;
                        if (int.TryParse(logsLocation, out valorInt))
                        {
                            if (Enum.IsDefined(typeof(LogsAndTracesLocation), valorInt))
                            {
                                LogLocation = (LogsAndTracesLocation)valorInt;
                            }
                        }
                        else
                        {
                            throw new Exception("El nodo logLocation no puede ser vacío");
                        }
                    }
                    else
                    {
                        throw new Exception("No se ha Configurado el nodo logLocation");
                    }
                }
                else
                {
                    throw new Exception("No se ha encontrado el Oauth.config en la ruta " + pFicheroOAuth);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        private void Write(LogLevels logLevel, string className, string memberName, string message, int numberWriteErrors = 3)
        {
            if (_isActivated)
            {
                string fileNameWithDate = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{LogHelper.LogFileName}";
                string absolutePath = $"{LogHelper.LogDirectory}/{fileNameWithDate}";
                string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                string completeMessage = $"{currentDate}\t{currentTime}\t{Thread.CurrentThread.ManagedThreadId}\t{logLevel.ToString()}\t{className}\t{memberName}\t{message}";

                if (!LogLocation.Equals(UtilTelemetry.LogsAndTracesLocation.ApplicationInsights))
                {
                    StreamWriter sw = null;
                    try
                    {                        
                        sw = new StreamWriter(absolutePath, true);
                        Console.WriteLine(completeMessage);
                        sw.WriteLine(completeMessage);
                        sw.Flush();
                        sw.Dispose();
                        sw.Close();
                    }
                    catch
                    {
                        Thread.Sleep(500);
                        if (numberWriteErrors > 0)
                        {
                            numberWriteErrors--;
                            Write(logLevel, className, memberName, message, numberWriteErrors);
                        }
                    }
                }

                if (UtilTelemetry.EstaConfiguradaTelemetria && !LogLocation.Equals(UtilTelemetry.LogsAndTracesLocation.File))
                {
                    try
                    {
                        if (LogHelper.LogLevel <= LogLevels.ERROR)
                        {
                            Exception ex = new Exception(completeMessage);
                            UtilTelemetry.EnviarTelemetriaExcepcion(ex, completeMessage);
                        }
                        else
                        {
                            UtilTelemetry.EnviarTelemetriaTraza(completeMessage);
                        }
                    }
                    catch
                    { }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the ApplicationInsights ImplementationKey
        /// </summary>
        public static string ImplementationKey
        {
            get; set;
        }

        /// <summary>
        /// Get or set the Logs location. Possible values: 
        /// 0 (log file only), 1 (ApplicationInsights only), 2 (both, file and App. Insights log recorder)
        /// </summary>
        public static LogsAndTracesLocation LogLocation
        {
            get; set;
        }

        public static bool UsarVariablesEntorno
        {
            get
            {
                return (Environment.GetEnvironmentVariable("useEnvironmentVariables") != null && Environment.GetEnvironmentVariable("useEnvironmentVariables").ToLower().Equals("true"));
            }
        }

        public static bool UsarApplicationInsights
        {
            get
            {
                return (Environment.GetEnvironmentVariable("useApplicationInsights") != null && Environment.GetEnvironmentVariable("useApplicationInsights").ToLower().Equals("true"));
            }
        }

        #endregion
    }

}
