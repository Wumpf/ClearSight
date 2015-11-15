using System;

namespace ClearSight.Core.Log
{
    /// <summary>
    /// A IDisposable object for automatic closing of loggroups.
    /// </summary>
    /// <remarks>
    /// Usage example:
    /// 
    /// using(new ScopedLogGroup("TestGroup"))
    /// {
    ///     Log.Info("Info from within the group.");
    /// }
    /// </remarks>
    public class ScopedLogGroup : IDisposable
    {
        private Log targetedLogger;

        /// <summary>
        /// Begins a new group on the targeted logger.
        /// </summary>
        /// <param name="groupName">Name of the group to open.</param>
        /// <param name="targetedLogger">Logger on which the group should be created and later ended. 
        /// Defaults to Logger.Default when null is passed.</param>
        public ScopedLogGroup(string groupName, Log targetedLogger = null)
        {
            this.targetedLogger = targetedLogger ?? Log.Default;
            Log.BeginGroup(this.targetedLogger, groupName);
        }

        public void Dispose()
        {
            Log.EndGroup(targetedLogger);
        }
    }
}
