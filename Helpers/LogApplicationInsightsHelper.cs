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
    public class LogApplicationInsightsHelper : ILogHelper
    {

        #region Static members

        private static string _logDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        public LogApplicationInsightsHelper()
        {
            if (string.IsNullOrEmpty(LogHelper.LogDirectory))
            {
                Console.WriteLine("Warning: The log has not been configured");
            }
            else
            {
                if (string.IsNullOrEmpty(LogHelper.LogFileName))
                {
                    LogHelper.LogFileName = "gnoss_api.log";
                }
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
            if (Enum.IsDefined(typeof(LogsAndTracesLocation), pLocation))
            {
                LogHelper.LogLocation = (LogsAndTracesLocation)pLocation;
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
        public static void LeerConfiguracionApplicationInsights(string pImplementationKey)
        {
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = pImplementationKey;
            ImplementationKey = pImplementationKey;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        private void Write(LogLevels logLevel, string className, string memberName, string message, int numberWriteErrors = 3)
        {
            string fileNameWithDate = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{LogHelper.LogFileName}";
            string absolutePath = $"{LogHelper.LogDirectory}/{fileNameWithDate}";
            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            string completeMessage = $"{currentDate}\t{currentTime}\t{Thread.CurrentThread.ManagedThreadId}\t{logLevel.ToString()}\t{className}\t{memberName}\t{message}";

            if (!LogHelper.LogLocation.Equals(LogsAndTracesLocation.ApplicationInsights))
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

            if (UtilTelemetry.EstaConfiguradaTelemetria && !LogHelper.LogLocation.Equals(LogsAndTracesLocation.File))
            {
                try
                {
                    if (logLevel < LogLevels.ERROR)
                    {
                        UtilTelemetry.EnviarTelemetriaTraza(completeMessage);
                    }
                    else
                    {
                        Exception ex = new Exception(completeMessage);
                        UtilTelemetry.EnviarTelemetriaExcepcion(ex, completeMessage);
                    }
                }
                catch
                { }
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
