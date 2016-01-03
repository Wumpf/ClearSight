using ClearSight.Core.Window;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract;
using System;
using System.Numerics;
using System.Windows.Interop;
using System.Windows.Threading;
using ClearSight.RendererAbstract.Binding;
using ClearSight.RendererAbstract.CommandSubmission;
using ClearSight.RendererAbstract.Memory;

namespace SimpleDesktop
{
    class Application : System.Windows.Application, ClearSight.Core.IApplication
    {
        private WPFWindow window;

        private Device renderDevice;
        private SwapChain swapChain;
        private CommandQueue commandQueue;
        private CommandList commandList;
        private DescriptorHeap descHeapRenderTargets;

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
            // Basics.
            {
                var deviceDesc = new Device.Descriptor {DebugDevice = true};
                renderDevice = new ClearSight.RendererDX12.Device(ref deviceDesc,
                    ClearSight.RendererDX12.Device.FeatureLevel.Level_11_0);

                var descCQ = new CommandQueue.Descriptor() {Type = CommandListType.Graphics};
                commandQueue = renderDevice.Create(ref descCQ);

                var wih = new WindowInteropHelper(window);
                var swapChainDesc = new SwapChain.Descriptor()
                {
                    AssociatedGraphicsQueue = commandQueue,

                    MaxFramesInFlight = 3,
                    BufferCount = 3,

                    Width = (uint) window.Width,
                    Height = (uint) window.Height,
                    Format = Format.R8G8B8A8_UNorm,

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

            // Render targets.
            {
                var descHeapDesc = new DescriptorHeap.Descriptor()
                {
                    Type = DescriptorHeap.Descriptor.ResourceDescriptorType.RenderTarget,
                    NumResourceDescriptors = swapChain.Desc.BufferCount
                };
                descHeapRenderTargets = renderDevice.Create(ref descHeapDesc);

                var rtvViewDesc = new RenderTargetViewDescription()
                {
                    Format = swapChain.Desc.Format,
                    Dimension = Dimension.Texture2D,
                    Texture = new TextureSubresourceDesc(mipSlice: 0)
                };
                for (uint i = 0; i < swapChain.Desc.BufferCount; ++i)
                {
                    renderDevice.CreateRenderTargetView(descHeapRenderTargets, i, swapChain.BackbufferResources[i], ref rtvViewDesc);   
                }
            }
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

            commandList.PerformResourceTransition(CommandList.BarrierType.Immediate, swapChain.ActiveBackBuffer, ResourceUsage.ReadWriteUsage.Present, ResourceUsage.ReadWriteUsage.RenderTargetColor);
            commandList.SetRenderTargets(descHeapRenderTargets, swapChain.ActiveSwapChainBufferIndex, null, 0);
            commandList.ClearRenderTargetView(CommandList.ClearTarget.Color, new Vector4(1.0f, 0.0f, 1.0f, 1.0f));

            commandList.PerformResourceTransition(CommandList.BarrierType.Immediate, swapChain.ActiveBackBuffer, ResourceUsage.ReadWriteUsage.RenderTargetColor, ResourceUsage.ReadWriteUsage.Present);
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
