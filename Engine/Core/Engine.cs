namespace ClearSight.Core
{
    /// <summary>
    /// Static class for central init and deinit of the engine.
    /// </summary>
    static public class Engine
    {
        /// <summary>
        /// Currently active application.
        /// </summary>
        static public IApplication CurrentApplication { private set; get; }

        /// <summary>
        /// Creates a new application of the given type and inits engine then.
        /// This method returns after the application stopped (CurrentApplication.IsRunning is false) and the engine was shut down.
        /// There can always be only one active application.
        /// </summary>
        static public void RunApplication<T>() where T : IApplication, new()
        {
            Assert.Always(CurrentApplication == null, "There is already an activate application.");

            CurrentApplication = new T();

            // Todo: Engine init.

            CurrentApplication.AfterEngineInit();

            CurrentApplication.Run();

            CurrentApplication.BeforeEngineShutdown();

            // Todo: Engine shutdown.

            CurrentApplication.AfterEngineShutdown();

            CurrentApplication = null;
        }
    }
}
