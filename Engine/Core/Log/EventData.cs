namespace ClearSight.Core.Log
{
    /// <summary>
    /// Different log event types that a logwriter can receive.
    /// Number values and special entries are for easy log event determination.
    /// </summary>
    /// <see cref="Log.MessageFilter"/>
    /// <see cref="EventData.messageType"/>
    [System.Flags]
    public enum EventType
    {
        /// <summary>
        /// A logging group has been opened.
        /// </summary>
        BeginGroup = 1,
        /// <summary>
        /// A logging group has been closed.
        /// </summary>
        EndGroup = 2,
        /// <summary>
        /// An error message.
        /// </summary>
        ErrorMsg = 4,
        /// <summary>
        /// A warning message.
        /// </summary>
        WarningMsg = 8,
        /// <summary>
        /// A success message.
        /// </summary>
        SuccessMsg = 16,
        /// <summary>
        /// A info message.
        /// </summary>
        InfoMsg = 32,
        /// <summary>
        /// A debug message. Disabled in debug by default.
        /// </summary>
        DebugMsg = 64,

        /// <summary>
        /// All message types flag.
        /// </summary>
        /// <see cref="Log.MessageFilter"/>
        All = BeginGroup | EndGroup | ErrorMsg | WarningMsg | SuccessMsg | InfoMsg | DebugMsg,
    };

    /// <summary>
    /// Data sent through ILog.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// The type of the message to be sent. For log groups this is the group name.
        /// </summary>
        public EventType messageType;
        /// <summary>
        /// In how many levels of groups this messae is. Translates usually to intendation.
        /// </summary>
        public int groupDepth;
        /// <summary>
        /// The actual log message.
        /// </summary>
        public string message;


        /// <summary>
        /// Combines message type and message. Does not take groupDepth into account.
        /// </summary>
        public string GetDefaultFormatedMessage()
        {
            switch(messageType)
            {
                case EventType.BeginGroup:
                    return "<Begin Group> " + message;
                case EventType.EndGroup:
                    return "<End Group> " + message;
                case EventType.ErrorMsg:
                    return "Error: " + message;
                case EventType.SuccessMsg:
                    return "Success: " + message;
                case EventType.InfoMsg:
                    return "Info: " + message;
                case EventType.DebugMsg:
                    return "Debug: " + message;
                default:
                    return message;
            }
        }
    };
}
