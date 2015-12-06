using System;
using ClearSight.RendererAbstract.Binding;
using ClearSight.RendererAbstract.CommandSubmission;
using ClearSight.RendererAbstract.Resources;

namespace ClearSight.RendererAbstract
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Features intentionally omitted so far:
    /// * Paging (make resident, evict)
    /// * Multi-GPU support (shared handle functions & for some other functions info for which gpu they should be executed)
    /// * Retrieve heap properties (is loosly connected to have custom heap properties)
    /// * Tiled resources
    /// </remarks>
    public abstract class Device : IDisposable
    {
        public struct Descriptor
        {
            public bool DebugDevice;
        }

        public Descriptor Desc { get; private set; }

        protected Device(ref Descriptor desc)
        {
            Desc = desc;
        }

        /// <summary>
        /// Copies descriptors from a source to a destination.
        /// </summary>
        public void CopyDescriptors(DescriptorHeap source, DescriptorHeap destination, Tuple<uint>[] rangeStarts, uint numDescriptors)
        {
            throw new NotImplementedException();
            CopyDescriptorsImpl(source, destination, rangeStarts, numDescriptors);
        }
        protected abstract void CopyDescriptorsImpl(DescriptorHeap source, DescriptorHeap destination, Tuple<uint>[] rangeStarts, uint numDescriptors);

        #region Create

        public abstract SwapChain Create(ref SwapChain.Descriptor desc, string label = "<unnamed swapChain>");

        public abstract CommandQueue Create(ref CommandQueue.Descriptor desc, string label = "<unnamed commandQueue>");
        public abstract CommandList Create(ref CommandList.Descriptor desc, string label = "<unnamed commandList>");

        /// <summary>
        /// Creates a CommandAllocator. Usually not needed, since this is done by the CommandListAllocaionPolicy.
        /// </summary>
        public abstract CommandAllocator Create(ref CommandAllocator.Descriptor desc, string label = "<unnamed commandAllocator>");
        public abstract Fence Create(ref Fence.Descriptor desc, string label = "<unnamed fence>");

        #endregion

        /// <summary>
        /// This method ensures the GPU timestamp counter does not stop ticking during idle periods.
        /// </summary>
        public void SetStablePowerState(bool enable)
        {
            SetStablePowerStateImpl(enable);
        }
        protected abstract void SetStablePowerStateImpl(bool enable);

        /// <summary>
        /// Destroys the device.
        /// </summary>
        public abstract void Dispose();
    }
}