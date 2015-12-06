using ClearSight.Core.Window;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract;
using System;
using System.Windows.Interop;
using System.Windows.Threading;
using ClearSight.RendererAbstract.CommandSubmission;

namespace SimpleDesktop
{
    class Application : System.Windows.Application, ClearSight.Core.IApplication
    {
        private WPFWindow window;

        private Device renderDevice;
        private SwapChain swapChain;
        private CommandQueue commandQueue;
        private CommandList commandList;

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
            var deviceDesc = new Device.Descriptor {DebugDevice = true};
            renderDevice = new ClearSight.RendererDX12.Device(ref deviceDesc, ClearSight.RendererDX12.Device.FeatureLevel.Level_11_0);

            var descCQ = new CommandQueue.Descriptor() { Type = CommandListType.Graphics };
            commandQueue = renderDevice.Create(ref descCQ);

            var wih = new WindowInteropHelper(window);
            var swapChainDesc = new SwapChain.Descriptor()
            {
                AssociatedGraphicsQueue = commandQueue,

                MaxFramesInFlight = 3,
                BufferCount = 3,

                Width = (uint)window.Width,
                Height = (uint)window.Height,

                SampleCount = 1,
                SampleQuality = 0,

                WindowHandle = wih.Handle,
                Fullscreen = false
            };
            swapChain = renderDevice.Create(ref swapChainDesc);

            var commandListDesc = new CommandList.Descriptor()
            {
                Type = CommandListType.Graphics,
                AllocationPolicy = new CommandListInFlightFrameAllocationPolicy(CommandListType.Graphics, swapChain)
            };
            commandList = renderDevice.Create(ref commandListDesc);
        }


        public void BeforeEngineShutdown()
        {
            commandList.Dispose();
            commandQueue.Dispose();
            swapChain.Dispose();
            renderDevice.Dispose();
        }

        public void AfterEngineShutdown()
        {
        }

        new public void Run()
        {
            ComponentDispatcher.ThreadIdle += new EventHandler(Update);
            Run(window);
        }

        private void Update(object sender, EventArgs e)
        {
            swapChain.BeginFrame();
            commandList.StartRecording();

            commandList.EndRecording();
            commandQueue.ExecuteCommandList(commandList);
            swapChain.EndFrame();
        }

        [STAThread]
        static void Main(string[] args)
        {
            ClearSight.Core.Engine.RunApplication<Application>();
        }
    }
}
