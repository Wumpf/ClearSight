using ClearSight.Core.Window;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract;
using System;

namespace SimpleDesktop
{
    class Application : System.Windows.Application, ClearSight.Core.IApplication
    {
        private WPFWindow window;
        private Device renderDevice;

        public Application()
        {
            Log.RegisterLogWriter(new DebuggerLogWriter());
            Log.RegisterLogWriter(new ConsoleLogWriter());

            // Log test.
            using(new ScopedLogGroup("TestGroup"))
            {
                Log.Error("testerror", 0);
                using (new ScopedLogGroup("TestGroupNested"))
                {
                    Log.Debug("testdebug {0}", 0);
                }
            }

            window = new ClearSight.Core.Window.WPFWindow(1024, 786);
        }

        public bool IsRunning => !((ClearSight.Core.Window.IWindow)window).Closed;

        public void AfterEngineInit()
        {
            Device.Descriptor desc = new Device.Descriptor {DebugDevice = true};
            renderDevice = new ClearSight.RendererDX12.Device(ref desc, ClearSight.RendererDX12.Device.FeatureLevel.Level_11_0);
        }

        public void BeforeEngineShutdown()
        {
            renderDevice.Dispose();
        }

        public void AfterEngineShutdown()
        {
        }

        new public void Run()
        {
            Run(window);
        }

        [STAThread]
        static void Main(string[] args)
        {
            ClearSight.Core.Engine.RunApplication<Application>();
        }
    }
}
