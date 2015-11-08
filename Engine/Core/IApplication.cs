using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearSight.Core
{
    /// <summary>
    /// Interface for all ClearSight applications.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Called after the engine initalization is done.
        /// </summary>
        void AfterEngineInit();

        /// <summary>
        /// Application run. After return, the application quits.
        /// </summary>
        void Run();

        /// <summary>
        /// Called after before the engine shuts down.
        /// </summary>
        void BeforeEngineShutdown();

        /// <summary>
        /// Called after before the engine shuts down.
        /// </summary>
        void AfterEngineShutdown();
    }
}
