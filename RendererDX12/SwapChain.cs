using System;
using ClearSight.Core.Log;
using SharpDX;
using SharpDX.DXGI;
using CommandQueue = ClearSight.RendererDX12.CommandSubmission.CommandQueue;

namespace ClearSight.RendererDX12
{
    public class SwapChain : RendererAbstract.SwapChain
    {
        public SwapChain3 SwapChainDXGI { get; private set; }

        internal SwapChain(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = (int)Desc.BufferCount,
                ModeDescription = new ModeDescription((int) Desc.Width, (int)Desc.Height, new Rational(0,0), Format.R8G8B8A8_UNorm),
                Usage = Usage.RenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                OutputHandle = Desc.WindowHandle,
                SampleDescription = new SampleDescription
                {
                    Count = (int) Desc.SampleCount,
                    Quality = (int) Desc.SampleQuality
                },
                IsWindowed = !Desc.Fullscreen
            };
            using (var factory = new Factory4())
            {
                var tempSwapChain = new SharpDX.DXGI.SwapChain(factory, ((CommandQueue)Desc.AssociatedGraphicsQueue).CommandQueueD3D12, swapChainDesc);
                SwapChainDXGI = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                CurrentBackBufferIndex = (uint)SwapChainDXGI.CurrentBackBufferIndex;
            }

            SwapChainDXGI.DebugName = Label;

            Log.Info("Created SwapChain");
        }

        protected override void DestroyImpl()
        {
            SwapChainDXGI.Dispose();
        }

        protected override void Present(uint syncInterval = 1)
        {
            SwapChainDXGI.Present((int)syncInterval, PresentFlags.None);
        }

        public override uint ActiveSwapChainBufferIndex => (uint)SwapChainDXGI.CurrentBackBufferIndex;

        
    }
}
