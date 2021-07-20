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
{    /// <summary>
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
    public class LogHelperFile: ILogHelper
    {

        #region Members

        private bool _isActivated;

        private static string _logDirectory;

        #endregion

        /// <summary>
        /// Gets or sets the log directory
        /// </summary>
        public string LogDirectory
        {
            get
            {
                return _logDirectory;
            }
            set
            {
                _logDirectory = value;

                if (!string.IsNullOrEmpty(_logDirectory))
                {
                    if (!Path.IsPathRooted(_logDirectory))
                    {
                        _logDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _logDirectory;
                    }

                    if (!Directory.Exists(_logDirectory))
                    {
                        Directory.CreateDirectory(_logDirectory);
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the log file name. (Default: gnoss_api.log)
        /// </summary>
        public string LogFileName
        {
            get; set;
        }

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        public LogHelperFile(string directory, string logFileName)
        {


            if (string.IsNullOrEmpty(directory))
            {
                _isActivated = false;
                Console.WriteLine("Warning: The log has not been configured");
            }
            else
            {
                LogDirectory = directory;
                if (string.IsNullOrEmpty(logFileName))
                {
                    LogFileName = "gnoss_api.log";
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
        
        #endregion

        #region Private methods

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        private void Write(LogLevels logLevel, string className, string memberName, string message, int numberWriteErrors = 3)
        {
            if (_isActivated)
            {
                StreamWriter sw = null;
                try
                {
                    string fileNameWithDate = $"{DateTime.Now.ToString("yyyy_MM_dd")}_{LogFileName}";
                    string absolutePath = $"{LogDirectory}/{fileNameWithDate}";
                    string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");
                    string completeMessage = $"{currentDate}\t{currentTime}\t{Thread.CurrentThread.ManagedThreadId}\t{logLevel.ToString()}\t{className}\t{memberName}\t{message}";

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
        }

        #endregion

        
    }

}
