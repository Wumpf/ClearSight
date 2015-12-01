using System;
using ClearSight.RendererAbstract.Binding;
using ClearSight.RendererAbstract.Resources;

namespace ClearSight.RendererDX12
{
    public abstract class Device : IDisposable
    {
        protected void CopyDescriptorsImpl(DescriptorHeap source, DescriptorHeap destination, Tuple<uint>[] rangeStarts, uint numDescriptors)
        {
            throw new NotImplementedException();
        }

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