using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Logs level
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// Write all messages
        /// </summary>
        TRACE = 0,

        /// <summary>
        /// Write fatal, error, warning, information and debug messages
        /// </summary>
        DEBUG = 1,

        /// <summary>
        /// Write fatal, error, warning and information messages
        /// </summary>
        INFO = 2,

        /// <summary>
        /// Write fatal, error and warning messages
        /// </summary>
        WARN = 3,

        /// <summary>
        /// Only write fatal and error messages
        /// </summary>
        ERROR = 4,

        /// <summary>
        /// Only write fatal messages
        /// </summary>
        FATAL = 5,

        /// <summary>
        /// No write any message
        /// </summary>
        OFF = 6
    }

    /// <summary>
    /// 
    /// </summary>
    public static class LogHelper
    {
        #region Static members

        private static string _logDirectory;

        /// <summary>
        /// 
        /// </summary>
        private static ILogHelper instance;

        #endregion

        #region Properties

        static LogHelper()
        {
            // Defult log level: Warning. Writes Fatal, Error and Warning messages. 
            LogHelper.LogLevel = LogLevels.WARN;
        }

        /// <summary>
        /// Gets the ILogHelper instance
        /// </summary>
        public static ILogHelper Instance
        {
            get
            {
                try
                {
                    if (instance == null)
                    {
                        instance = new LogHelperFile();
                    }

                    return instance;
                }
                catch (Log4csharpException logEx)
                {
                    throw logEx;
                }
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public static LogLevels LogLevel
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the log directory
        /// </summary>
        public static string LogDirectory
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
        public static string LogFileName
        {
            get; set;
        }

        #endregion
    }
}
