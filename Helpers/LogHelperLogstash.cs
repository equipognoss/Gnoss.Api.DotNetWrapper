using System;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using Serilog;
using Serilog.Core;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Clase LogHelperLogstash.
    /// </summary>
    public class LogHelperLogstash : ILogHelper
    {
        #region Members                

        /// <summary>
        /// Objeto logger.
        /// </summary>
        private static Logger LOG;

        private static DatosRequest request;

        private IHttpContextAccessor _httpContextAccessor;

        #endregion


        public LogHelperLogstash(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Métodos Públicos
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
                //Write(LogLevels.TRACE, className, memberName, message);
                EnviarLog(LogLevels.TRACE, className, memberName, message);
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
                //Write(LogLevels.DEBUG, className, memberName, message);
                EnviarLog(LogLevels.DEBUG, className, memberName, message);
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
                //Write(LogLevels.INFO, className, memberName, message);
                EnviarLog(LogLevels.INFO, className, memberName, message);
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
                //Write(LogLevels.WARN, className, memberName, message);
                EnviarLog(LogLevels.WARN, className, memberName, message);
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
                //Write(LogLevels.ERROR, className, memberName, message);
                EnviarLog(LogLevels.ERROR, className, memberName, message);
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
                //Write(LogLevels.FATAL, className, memberName, message);
                EnviarLog(LogLevels.FATAL, className, memberName, message);
            }
        }
        #endregion

        #region Métodos Privados
        private void EnviarLog(LogLevels logLevel, string className, string memberName, string message, int numberWriteErrors = 3)
        {
            try
            {
                string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                string completeMessage = $"{currentDate}\t{currentTime}\t{Thread.CurrentThread.ManagedThreadId}\t{logLevel.ToString()}\t{className}\t{memberName}\t{message}";

                request = new DatosRequest();

                //if (HttpContext.Current != null && HttpContext.Current.Request != null)
                if (_httpContextAccessor.HttpContext.Request != null)
                {
                    // Rellenar objeto con los datos (Request)
                    HttpRequest currentRequest = _httpContextAccessor.HttpContext.Request;

                    IRequestCookieCollection listaCookies = currentRequest.Cookies;
                    request.CookieSessionId = currentRequest.Cookies["ASP.NET_SessionId"];

                    string headers = "";
                    string[] arrayHeaders = currentRequest.Headers.Keys.ToArray();
                    foreach (string clave in arrayHeaders)
                    {
                        currentRequest.Headers.TryGetValue(clave, out StringValues values);
                        headers += $"{clave} : {string.Join(" ", values)}\n";
                    }
                    request.Headers = headers.TrimEnd();

                    request.HttpMethod = currentRequest.Method;
                    request.RawURL = currentRequest.GetEncodedUrl();
                    request.URL = currentRequest.GetDisplayUrl();
                    request.UserAgent = currentRequest.Headers["User-Agent"];
                    request.Domain = currentRequest.Host.Value;

                    if (currentRequest.Body != null && currentRequest.Body.Length > 0)
                    {
                        currentRequest.Body.Position = 0;
                        StreamReader streamInputStream = new StreamReader(currentRequest.Body);
                        request.Body = streamInputStream.ReadToEnd();
                    }

                    string ip = currentRequest.Headers["X-FORWARDED-FOR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        request.Ip = ip;
                    }
                }


                // Objeto Anónimo
                var data = new { CurrentDate = currentDate, CurrentTime = currentTime, ThreadId = Thread.CurrentThread.ManagedThreadId, LogLevel = logLevel.ToString(), ClassName = className, MemberName = memberName, Message = message };


                LOG = new LoggerConfiguration()
                    .WriteTo.Http(EndPoint)
                    .CreateLogger();

                switch (logLevel)
                {
                    case LogLevels.TRACE:
                        LOG = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.Http(EndPoint)
                            .CreateLogger();

                        LOG.Verbose("{@data}, {@request}", data, request);
                        break;
                    case LogLevels.DEBUG:
                        LOG = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Http(EndPoint)
                            .CreateLogger();

                        LOG.Debug("{@data}, {@request}", data, request);
                        break;
                    case LogLevels.INFO:
                        LOG.Information("{@data}, {@request}", data, request);
                        break;
                    case LogLevels.WARN:
                        LOG.Warning("{@data}, {@request}", data, request);
                        break;
                    case LogLevels.ERROR:
                        LOG.Error("{@data}, {@request}", data, request);
                        break;
                    case LogLevels.FATAL:
                        LOG.Fatal("{@data}, {@request}", data, request);
                        break;
                }

                Console.WriteLine($"Mensaje Enviado: {completeMessage}");
            }
            catch
            {
                Thread.Sleep(500);
                if (numberWriteErrors > 0)
                {
                    numberWriteErrors--;
                    EnviarLog(logLevel, className, memberName, message, numberWriteErrors);
                }
            }

        }
        #endregion

        #region Propiedades

        public static string EndPoint { get; set; }

        #endregion
    }
}
