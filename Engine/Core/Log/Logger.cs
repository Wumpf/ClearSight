using System.Collections.Generic;
using System.Diagnostics;

namespace ClearSight.Core.Log
{
    /// <summary>
    /// The main logger class. You can use either the global log 
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Global log instance.
        /// </summary>
        static public Logger Global { get; private set; } = new Logger();

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
        /// Current message intendation. Rises with opening log groups, falls with closing them.
        /// </summary>
        private int groupDepth = 0;

        /// <summary>
        /// Name of the last opened log group.
        /// </summary>
        private string lastGroupName;

        #endregion

        /// <summary>
        /// Registers a new log-writer. Will be ignored if already registered.
        /// </summary>
        /// <param name="newLogWriter">The log writer to register.</param>
        /// <param name="sendHistory">If true, all saved log messages will be sent to the new logwriter.</param>
        public void RegisterLogWriter(ILogWriter logWriter, bool sendHistory = true)
        {
            if(registeredLogWriter.Add(logWriter) && sendHistory)
            {
                foreach(var evt in logHistory)
                {
                    logWriter.OnLogEvent(evt);
                }
            }
        }

        /// <summary>
        /// Unregisters a existing log-writer.
        /// </summary>
        /// <param name="logWriter">Logwriter to unregister</param>
        public void UnregisterLogWriter(ILogWriter logWriter)
        {
            registeredLogWriter.Remove(logWriter);
        }

        /// <summary>
        /// Clears log record and informs log-writers.
        /// Will not reset the current group depth and group name.
        /// </summary>
        public void FlushLogHistory()
        {
            logHistory.Clear();
            foreach (var writer in registeredLogWriter)
            {
                writer.OnLogHistoryFlushed();
            }
        }

        #region logging functions

        public void BeginGroup(string groupName)
        {
            lastGroupName = groupName;
            HandleLogMessage(groupName, EventType.BeginGroup);
            ++groupDepth;
        }

        public void EndGroup()
        {
            HandleLogMessage(lastGroupName, EventType.EndGroup);
            --groupDepth;
        }

        public void Error(string message)
        {
            HandleLogMessage(message, EventType.ErrorMsg);
        }

        public void Warning(string message)
        {
            HandleLogMessage(message, EventType.WarningMsg);
        }

        public void Success(string message)
        {
            HandleLogMessage(message, EventType.SuccessMsg);
        }

        public void Info(string message)
        {
            HandleLogMessage(message, EventType.InfoMsg);
        }

        /// <summary>
        /// Logs a debug message. Only active in debug mode regardless of MessageFilter.
        /// </summary>
        /// <param name="message">Content of the message to be logged.</param>
        [Conditional("DEBUG")]
        public void Debug(string message)
        {
            HandleLogMessage(message, EventType.DebugMsg);
        }

        #endregion

        private void HandleLogMessage(string message, EventType messageType)
        {
            EventData logEvent = new EventData
            {
                messageType = messageType,
                message = message,
                groupDepth = groupDepth
            };
            logHistory.Add(logEvent);

            foreach (var writer in registeredLogWriter)
            {
                writer.OnLogEvent(logEvent);
            }
        }
    };
}
