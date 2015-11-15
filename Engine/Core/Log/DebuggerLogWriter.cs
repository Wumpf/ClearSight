using System.Diagnostics;

namespace ClearSight.Core.Log
{
    /// <summary>
    /// Log writer for writing to the debug console.
    /// </summary>
    public class DebuggerLogWriter : ILogWriter
    {
        public void OnLogEvent(EventData logEvent)
        {
            if (logEvent.messageType == EventType.BeginGroup)
            {
                Trace.Indent();
            }
            else if (logEvent.messageType == EventType.EndGroup)
            {
                Trace.Unindent();
            }

            Trace.WriteLine(logEvent.GetDefaultFormatedMessage());
        }

        public void OnLogHistoryFlushed()
        {
        }
    }
}
