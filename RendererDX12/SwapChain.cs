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

        /// <summary>
        /// Creates a new DXGI swap chain.
        /// </summary>
        /// <exception cref="SharpDX.SharpDXException"></exception>
        public SwapChain(ref Descriptor desc) : base(ref desc)
        {
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = (int)Desc.BufferCount,
                ModeDescription = new ModeDescription((int) desc.Width, (int) desc.Height, new Rational(0,0), Format.R8G8B8A8_UNorm),
                Usage = Usage.RenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                OutputHandle = desc.WindowHandle,
                SampleDescription = new SampleDescription
                {
                    Count = (int) Desc.SampleCount,
                    Quality = (int) Desc.SampleQuality
                },
                IsWindowed = !Desc.Fullscreen
            };
            using (var factory = new Factory4())
            {
                var tempSwapChain = new SharpDX.DXGI.SwapChain(factory, ((CommandQueue)desc.AssociatedGraphicsQueue).CommandQueueD3D12, swapChainDesc);
                SwapChainDXGI = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                CurrentBackBufferIndex = (uint)SwapChainDXGI.CurrentBackBufferIndex;
            }

            Log.Info("Created SwapChain");
        }

        public override void Present(uint syncInterval)
        {
            SwapChainDXGI.Present((int)syncInterval, PresentFlags.None);
        }

        public override void Dispose()
        {
            SwapChainDXGI.Dispose();
        }
    }
}
