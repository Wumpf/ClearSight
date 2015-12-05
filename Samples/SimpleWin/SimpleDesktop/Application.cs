using ClearSight.Core.Window;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract;
using System;
using System.Windows.Interop;
using ClearSight.RendererAbstract.CommandSubmission;

namespace SimpleDesktop
{
    class Application : System.Windows.Application, ClearSight.Core.IApplication
    {
        private WPFWindow window;

        private Device renderDevice;
        private SwapChain swapChain;
        private CommandQueue commandQueue;

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

            CommandQueue.Descriptor descCQ = new CommandQueue.Descriptor() { Type = CommandQueue.Descriptor.Types.Graphics };
            commandQueue = renderDevice.Create(ref descCQ);

            var wih = new WindowInteropHelper(window);
            var swapChainDesc = new SwapChain.Descriptor()
            {
                AssociatedGraphicsQueue = commandQueue,
                BufferCount = 3,
                Width = (uint)window.Width,
                Height = (uint)window.Height,

                SampleCount = 1,
                SampleQuality = 0,

                WindowHandle = wih.Handle,
                Fullscreen = false
            };
            swapChain = new ClearSight.RendererDX12.SwapChain(ref swapChainDesc);
        }


        public void BeforeEngineShutdown()
        {
            commandQueue.Dispose();
            swapChain.Dispose();
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
