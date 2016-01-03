using SwapChainDX = ClearSight.RendererDX12.SwapChain;

namespace ClearSight.RendererDX12.Memory
{
    public class Resource : RendererAbstract.Memory.Resource
    {
        public SharpDX.Direct3D12.Resource ResourceD3D12 { get; private set; }

        internal Resource(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            // todo
        }

        protected override void DestroyImpl()
        {
            // todo
        }

        protected override void CreateFromSwapChainImpl(RendererAbstract.SwapChain swapChain, uint backbufferIndex)
        {
            ResourceD3D12 = ((SwapChainDX) swapChain).SwapChainDXGI.GetBackBuffer<SharpDX.Direct3D12.Resource>((int) backbufferIndex);
        }
    }
}
