using System;

namespace ClearSight.RendererAbstract
{
    /// <summary>
    /// Base class for all objects that are created and managed by the device.
    /// </summary>
    public abstract class DeviceChild : IDisposable
    {
        /// <summary>
        /// Currently used memory on the CPU.
        /// </summary>
        public int SizeCpu { get; private set; } = 0;

        /// <summary>
        /// Currently used memory on the GPU.
        /// </summary>
        public int SizeGpu { get; private set; } = 0;

        public void Dispose()
        {
            
        }
    }
}
