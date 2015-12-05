using System.Diagnostics;

namespace ClearSight.Core
{
    /// <summary>
    /// Provides assertion methods. Use these instead of the .net assertions for better control.
    /// Asserts should only be used for critical errors in the programming. They are well suited to document invarients.
    /// </summary>
    static public class Assert
    {
        /// <summary>
        /// Assertion that is only executed if DEBUG is defined.
        /// </summary>
        /// <param name="condition">Condition that should be met.</param>
        /// <param name="message">Message if it condition is false.</param>
        [Conditional("DEBUG")]
        [DebuggerHidden]
        static public void Debug(bool condition, string message)
        {
            if (!condition)
            {
                Log.Log.Error(message);
            }

            // Todo? Own assert implementation?
            System.Diagnostics.Debug.Assert(condition, message);
        }

        /// <summary>
        /// Assertion that is executed no matter of the compilation settings.
        /// </summary>
        /// <param name="condition">Condition that should be met.</param>
        /// <param name="message">Message if it condition is false.</param>
        [DebuggerHidden]
        static public void Always(bool condition, string message)
        {
            if (!condition)
            {
                Log.Log.Error(message);
            }

            // Todo? Own assert implementation?
            System.Diagnostics.Trace.Assert(condition, message);
        }
    }
}
