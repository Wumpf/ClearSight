using ClearSight.RendererAbstract.Resources;

namespace ClearSight.RendererAbstract.Memory
{
    public abstract partial class Resource : DeviceChild<Resource.Descriptor>
    {
        public struct Descriptor
        {
            public enum Types
            {
                Default,
                Upload,
                Readback,
                //Custom // Not implemented for now.

                /// <summary>
                /// Special resource that is created from a swap chain.
                /// Do not use this manually!
                /// </summary>
                Backbuffer,
            }

            public Types Type;
            public Dimension DataDimension;
        }

        /// <summary>
        /// Heap on which this resource was created.
        /// If null the resource is either destroyed or is a commited resource.
        /// </summary>
        public Heap Heap { get; private set; } = null;

        protected Resource(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }

        /// <summary>
        /// Called after the normal create if this resource was created from a swapchain backbuffer.
        /// </summary>
        internal void CreateFromSwapChain(SwapChain swapChain, uint backbufferIndex)
        {
            CreateFromSwapChainImpl(swapChain, backbufferIndex);
        }
        protected abstract void CreateFromSwapChainImpl(SwapChain swapChain, uint backbufferIndex);
    }
}
