using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Interface for log helper
    /// </summary>
    public interface ILogHelper
    {
        /// <summary>
        /// Write a trace log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Trace(string message, string className = "", [CallerMemberName] string memberName = "");

        /// <summary>
        /// Write a debug log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Debug(string message, string className = "", [CallerMemberName] string memberName = "");

        /// <summary>
        /// Write a information log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Info(string message, string className = "", [CallerMemberName] string memberName = "");

        /// <summary>
        /// Write a warning log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Warn(string message, string className = "", [CallerMemberName] string memberName = "");

        /// <summary>
        /// Write a error log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Error(string message, string className = "", [CallerMemberName] string memberName = "");

        /// <summary>
        /// Write a fatal log message
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="className">(Optional) Class name who invokes this method</param>
        /// <param name="memberName">(Optional) Method name who infoes this</param>
        void Fatal(string message, string className = "", [CallerMemberName] string memberName = "");
    }
}
