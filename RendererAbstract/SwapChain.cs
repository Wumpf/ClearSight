using System;
using ClearSight.Core;
using ClearSight.RendererAbstract.CommandSubmission;

namespace ClearSight.RendererAbstract
{
    public abstract class SwapChain : IDisposable
    {
        public struct Descriptor
        {
            /// <summary>
            /// Swap chain may flush this queue on present.
            /// </summary>
            public CommandQueue AssociatedGraphicsQueue;

            public uint BufferCount;
            public uint Width;
            public uint Height;

            public uint SampleCount;
            public uint SampleQuality;

            public IntPtr WindowHandle;
            public bool Fullscreen;
        }
        public Descriptor Desc { get; private set; }

        /// <summary>
        /// Index of the active backbuffer.
        /// </summary>
        public uint CurrentBackBufferIndex = 0;


        protected SwapChain(ref Descriptor desc)
        {
            Assert.Debug(desc.BufferCount > 0, "Invalid backbuffer count!");
            Assert.Debug(desc.AssociatedGraphicsQueue != null, "No graphics queue passed!");
            Assert.Debug(desc.AssociatedGraphicsQueue.Desc.Type == CommandQueue.Descriptor.Types.Graphics, "Passed command queue needs to be a graphics queue!");

            Desc = desc;
        }

        /// <summary>
        /// Performs a back buffer flip.
        /// </summary>
        /// <param name="syncInterval">Synchronize presentation after the nth vertical blank. 0 means no synchronization.</param>
        public abstract void Present(uint syncInterval = 1);

        public abstract void Dispose();
    }
}
