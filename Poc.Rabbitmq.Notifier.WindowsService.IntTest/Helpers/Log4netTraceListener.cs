﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using log4net;

namespace Poc.Rabbitmq.Notifier.WindowsService.IntTest.Helpers
{
    /// <summary>
    /// Trace listener that will write all trace messages to log 4 net. This listener makes sure not
    /// to break existing trace functionality.
    /// </summary>
 	public class Log4netTraceListener : TraceListener
    {
        #region Constants
        /// <summary>
        /// Trace message for the ItemsSource timing issue. This line should be ignored.
        /// </summary>
        private const string ItemsSourceTimingIssueTrace = "ContentAlignment; DataItem=null;";
        #endregion

        #region Variables
        private static readonly ILog _log = LogManager.GetLogger("Vueling.Contingency.CrmNotifier.WindowsService.IntTest");
        #endregion

        #region Constructor & destructor
        /// <summary>
        /// Initializes a new ins
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:Do not indirectly expose methods with link demands")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:Do not call overridable methods in constructors")]
        public Log4netTraceListener()
        {
            // Set default values
            Name = "Log4net Trace Listener";
            ActiveTraceLevel = TraceLevel.Verbose;

            // Create additional trace sources list
            TraceSourceCollection = new List<TraceSource>();

            // Add additional trace sources - .NET framework
            //TraceSources.Add(System.Diagnostics.

            // Subscribe to all trace sources
            foreach (TraceSource traceSource in TraceSourceCollection)
            {
                traceSource.Listeners.Add(this);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the trace source collection.
        /// </summary>
        /// <value>The trace source collection.</value>
        private List<TraceSource> TraceSourceCollection { get; set; }

        /// <summary>
        /// Gets or sets the active trace type.
        /// </summary>
        public TraceLevel ActiveTraceLevel { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            // Call overload
            TraceEvent(eventCache, source, eventType, id, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        private string GetMessage(TraceEventCache eventCache, string message)
        {
            string result = string.Empty;
            var lastIndexOf = eventCache?.Callstack?.LastIndexOf("System.Diagnostics.Trace.TraceError(String format, Object[] args)", StringComparison.Ordinal);
            if (lastIndexOf.HasValue)
            {
                result = (lastIndexOf > -1) ? eventCache.Callstack.Substring(lastIndexOf.Value).Replace("System.Diagnostics.Trace.TraceError(String format, Object[] args)\r\n", "") :
                                              eventCache.Callstack;
            }
            return message + result;
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            // Should we ignore this line?
            if (message.Contains(ItemsSourceTimingIssueTrace)) return;

            // Check the type
            switch (eventType)
            {
                case TraceEventType.Error:
                    if (ActiveTraceLevel >= TraceLevel.Error)
                    {
                        _log.Error(GetMessage(eventCache, message));
                    }
                    break;

                case TraceEventType.Warning:
                    if (ActiveTraceLevel >= TraceLevel.Warning)
                    {
                        _log.Warn(GetMessage(eventCache, message));
                    }
                    break;

                case TraceEventType.Information:
                    if (ActiveTraceLevel >= TraceLevel.Info)
                    {
                        _log.Info(message);
                    }
                    break;

                default:
                    if (ActiveTraceLevel == TraceLevel.Verbose)
                    {
                        _log.Debug(message);
                    }
                    break;
            }
        }

        /// <summary>
        /// Writes text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void Write(string message)
        {
            // Call write line
            WriteLine(message);
        }

        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void WriteLine(string message)
        {
            // Invoke the event (but only when output is set to verbose)
            if (ActiveTraceLevel == TraceLevel.Verbose)
            {
                _log.Debug(message);
            }
        }
        #endregion
    }
}
