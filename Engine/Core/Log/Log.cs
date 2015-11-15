using System.Collections.Generic;
using System.Diagnostics;

namespace ClearSight.Core.Log
{
    /// <summary>
    /// The main logger class. All functions are implemented statically with either a variable log or using the default logger.
    /// The logger by itself does not perform any writes to disk or elsewhere. You need to register a ILogWriter for that.
    /// </summary>
    public sealed class Log
    {
        /// <summary>
        /// Default log instance. May be changed.
        /// </summary>
        static public Log Default
        {
            get
            {
                return defaultLog;
            }
            set
            {
                Assert.Debug(value != null, "It is not possible to set the default logger to null.");
            }
        }
        static private Log defaultLog = new Log();

        /// <summary>
        /// Determines which messages should be send and which not.
        /// Note that calls to Logger.Debug will not be compiled into release builds!
        /// </summary>
        public EventType MessageFilter { get; set; } = EventType.All;

        #region private state

        /// <summary>
        /// Set of registered logwriters.
        /// </summary>
        private HashSet<ILogWriter> registeredLogWriter = new HashSet<ILogWriter>();

        /// <summary>
        /// Record of log messages so far.
        /// </summary>
        private List<EventData> logHistory = new List<EventData>();

        /// <summary>
        /// Stack of log groups.
        /// </summary>
        /// <see cref="BeginGroup(string)"/>
        /// <see cref="EndGroup(string)"/>
        private Stack<string> logGroups = new Stack<string>();

        #endregion

        /// <see cref="RegisterLogWriter(Log, ILogWriter, bool)"/>
        static public void RegisterLogWriter(ILogWriter logWriter, bool sendHistory = true)
        {
            RegisterLogWriter(defaultLog, logWriter, sendHistory);
        }
        /// <summary>
        /// Registers a new log-writer. Will be ignored if already registered.
        /// </summary>
        /// <param name="newLogWriter">The log writer to register.</param>
        /// <param name="sendHistory">If true, all saved log messages will be sent to the new logwriter.</param>
        static public void RegisterLogWriter(Log targetedLog, ILogWriter logWriter, bool sendHistory = true)
        {
            if (targetedLog.registeredLogWriter.Add(logWriter) && sendHistory)
            {
                foreach (var evt in targetedLog.logHistory)
                {
                    logWriter.OnLogEvent(evt);
                }
            }
        }


        /// <see cref="UnregisterLogWriter(Log, ILogWriter)"/>
        static public void UnregisterLogWriter(ILogWriter logWriter)
        {
            UnregisterLogWriter(defaultLog, logWriter);
        }
        /// <summary>
        /// Unregisters a existing log-writer.
        /// </summary>
        /// <param name="logWriter">Logwriter to unregister</param>
        static public void UnregisterLogWriter(Log targetedLog, ILogWriter logWriter)
        {
            targetedLog.registeredLogWriter.Remove(logWriter);
        }


        /// <see cref="FlushLogHistory(Log)"/>
        static public void FlushLogHistory()
        {
            FlushLogHistory(defaultLog);
        }
        /// <summary>
        /// Clears log record and informs log-writers.
        /// Will not reset the current group depth and group name.
        /// </summary>
        static public void FlushLogHistory(Log targetedLog)
        {
            targetedLog.logHistory.Clear();
            foreach (var writer in targetedLog.registeredLogWriter)
            {
                writer.OnLogHistoryFlushed();
            }
        }

        #region logging functions

        /// <see cref="BeginGroup(Log, string)"/>
        static public void BeginGroup(string groupName)
        {
            BeginGroup(defaultLog, groupName);
        }
        /// <summary>
        /// Starts a logging group.
        /// </summary>
        /// <param name="groupName">Name of the new group.</param>
        static public void BeginGroup(Log targetedLog, string groupName)
        {
            targetedLog.HandleLogMessage(groupName, EventType.BeginGroup);
            targetedLog.logGroups.Push(groupName);
        }

        /// <see cref="BeginGroup(Log, string)"/>
        static public void EndGroup()
        {
            EndGroup(defaultLog);
        }
        /// <summary>
        /// Closes a previous logging group. Will be ignored if no group is open.
        /// </summary>
        static public void EndGroup(Log targetedLog)
        {
            if (targetedLog.logGroups.Count > 0)
            {
                targetedLog.HandleLogMessage(targetedLog.logGroups.Pop(), EventType.EndGroup);
            }
        }

        static public void Error(string message, params object[] args)
        {
            defaultLog.HandleLogMessage(string.Format(message, args), EventType.ErrorMsg);
        }
        static public void Error(Log targetedLog, string message, params object[] args)
        {
            targetedLog.HandleLogMessage(string.Format(message, args), EventType.ErrorMsg);
        }

        static public void Warning(string message, params object[] args)
        {
            defaultLog.HandleLogMessage(string.Format(message, args), EventType.WarningMsg);
        }
        static public void Warning(Log targetedLog, string message, params object[] args)
        {
            targetedLog.HandleLogMessage(string.Format(message, args), EventType.WarningMsg);
        }

        static public void Success(string message, params object[] args)
        {
            defaultLog.HandleLogMessage(string.Format(message, args), EventType.SuccessMsg);
        }
        static public void Success(Log targetedLog, string message, params object[] args)
        {
            targetedLog.HandleLogMessage(string.Format(message, args), EventType.SuccessMsg);
        }

        static public void Info(string message, params object[] args)
        {
            defaultLog.HandleLogMessage(string.Format(message, args), EventType.InfoMsg);
        }
        static public void Info(Log targetedLog, string message, params object[] args)
        {
            targetedLog.HandleLogMessage(string.Format(message, args), EventType.InfoMsg);
        }

        /// <see cref="Debug(Log, string, object[])"/>
        [Conditional("DEBUG")]
        static public void Debug(string message, params object[] args)
        {
            defaultLog.HandleLogMessage(string.Format(message, args), EventType.DebugMsg);
        }
        /// <summary>
        /// Logs a debug message. Only active in debug mode regardless of MessageFilter.
        /// </summary>
        /// <param name="message">Content of the message to be logged.</param>
        [Conditional("DEBUG")]
        public void Debug(Log targetedLog, string message, params object[] args)
        {
            targetedLog.HandleLogMessage(string.Format(message, args), EventType.DebugMsg);
        }

        #endregion

        private void HandleLogMessage(string message, EventType messageType)
        {
            EventData logEvent = new EventData
            {
                messageType = messageType,
                message = message,
                groupDepth = logGroups.Count
            };
            logHistory.Add(logEvent);

            foreach (var writer in registeredLogWriter)
            {
                writer.OnLogEvent(logEvent);
            }
        }
    };
}
