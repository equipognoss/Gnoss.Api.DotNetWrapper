using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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
    public class LogHelper
    {
        #region Static members

        

        /// <summary>
        /// 
        /// </summary>
        private static ILogHelper instance;
        private string mDirectory;
        private string mFileName;
        #endregion

        #region Properties

        private IHttpContextAccessor _httpContextAccessor;

        public LogHelper(IHttpContextAccessor httpContextAccessor)
        {
            // Defult log level: Warning. Writes Fatal, Error and Warning messages. 
            LogHelper.LogLevel = LogLevels.WARN;
            _httpContextAccessor = httpContextAccessor;
        }

        public LogHelper(string directory, string fileName, IHttpContextAccessor httpContextAccessor = null)
        {
            // Defult log level: Warning. Writes Fatal, Error and Warning messages. 
            LogHelper.LogLevel = LogLevels.WARN;
            _httpContextAccessor = httpContextAccessor;
            mDirectory = directory;
            mFileName = fileName;
        }

        /// <summary>
        /// Get or set the Logs location. Possible values: 
        /// 0 (log file only), 1 (ApplicationInsights only), 2 (both, file and App. Insights log recorder)
        /// </summary>
        public static LogsAndTracesLocation LogLocation
        {
            get; set;
        }

        /// <summary>
        /// Gets the ILogHelper instance
        /// </summary>
        public ILogHelper Instance
        {
            get
            {
                try
                {
                    if (instance == null)
                    {
                        if (!string.IsNullOrEmpty(LogApplicationInsightsHelper.ImplementationKey) && (LogHelper.LogLocation == LogsAndTracesLocation.ApplicationInsights || LogHelper.LogLocation == LogsAndTracesLocation.FileAndAppInsights))
                        {
                            instance = new LogApplicationInsightsHelper();
                        }
                        else if (_httpContextAccessor != null && !string.IsNullOrEmpty(LogHelperLogstash.EndPoint) && (LogHelper.LogLocation == LogsAndTracesLocation.Logstash || LogHelper.LogLocation == LogsAndTracesLocation.FileAndLogstash))
                        {
                            instance = new LogHelperLogstash(_httpContextAccessor);
                        }
                        else
                        {
                            instance = new LogHelperFile(mDirectory, mFileName);
                        }
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

        
        #endregion
    }
}
