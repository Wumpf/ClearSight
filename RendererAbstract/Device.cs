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

        public abstract CommandQueue Create(ref CommandQueue.Descriptor desc, string label = "<unnamed>");

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