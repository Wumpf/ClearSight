using System;
using System.Diagnostics;

namespace ClearSight.Core.Log
{
    /// <summary>
    /// Log writer for writing to the system console.
    /// </summary>
    public class ConsoleLogWriter : ILogWriter
    {
        public void OnLogEvent(EventData logEvent)
        {
            string tabs = new String('\t', logEvent.groupDepth);
            Console.WriteLine(tabs + logEvent.GetDefaultFormatedMessage());
        }

        public void OnLogHistoryFlushed()
        {
        }
    }
}
