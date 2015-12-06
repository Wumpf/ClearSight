using System;
using System.Diagnostics;
using ClearSight.Core;
using ClearSight.RendererAbstract.CommandSubmission;

namespace ClearSight.RendererAbstract
{
    /// <summary>
    /// Abstraction for swap chain. Contains powerful helper methods for frame synchronization as necessary for modern APIs.
    /// </summary>
    /// <remarks>
    /// Synchronization capabilities require an internal Fence. However, using such a centralized fence makes it easy to avoid fences in various places which need to sync with frame display.
    /// This applies to practically all resources that need updates and are used to create render a frame.
    /// Note that the number of frames the CPU has finished ("FramesInFlight") is completely decoupled from the number of frames the GPU has finished for display.
    /// Technically the SwapChain is not a real DeviceChild, but implementing it as such simplifies the code and does not lead to any constraints.
    /// </remarks>
    public abstract class SwapChain : DeviceChild<SwapChain.Descriptor>
    {
        public struct Descriptor
        {
            /// <summary>
            /// Swap chain may flush this queue on present.
            /// </summary>
            public CommandQueue AssociatedGraphicsQueue;

            /// <summary>
            /// The SwapChain will guarantee that maximal MaxFramesInFlight are in-flight (important from CPU -> GPU synchronization).
            /// </summary>
            /// <remarks>
            /// This means that there should be exactly MaxFramesInFlight versions of all resources that change every frame!
            /// The more, the more command-queues can the CPU prepare that will executed / rendered to the next backbuffer.
            /// (Needs to be bigger than 0, at least 3 is recommend)
            /// </remarks>
            public uint MaxFramesInFlight;

            /// <summary>
            /// How many backbuffers are there (important for GPU -> Screen synchronization).
            /// </summary>
            /// <remarks>
            /// The more, the more frames can the GPU prepare that will later be shown by the screen.
            /// (Needs to be bigger than 0, at least 3 is recommend)
            /// </remarks>
            public uint BufferCount;

            public uint Width;
            public uint Height;

            public uint SampleCount;
            public uint SampleQuality;

            public IntPtr WindowHandle;
            public bool Fullscreen;
        }

        /// <summary>
        /// Index of the active backbuffer.
        /// </summary>
        public uint CurrentBackBufferIndex = 0;


        protected SwapChain(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Assert.Debug(desc.MaxFramesInFlight > 0, "Invalid in-flight framecount!");
            Assert.Debug(desc.BufferCount > 0, "Invalid backbuffer count!");
            Assert.Debug(desc.AssociatedGraphicsQueue != null, "No graphics queue passed!");
            Assert.Debug(desc.AssociatedGraphicsQueue.Desc.Type == CommandListType.Graphics, "Passed command queue needs to be a graphics queue!");

            var fenceDesc = new Fence.Descriptor { InitialValue = 0 };
            FrameFence = device.Create(ref fenceDesc);
            FrameFence.AddRef();

            Create();
        }

        #region Frame Synchronization & Management

        /// <summary>
        /// Fence used for frame synchronization.
        /// </summary>
        public Fence FrameFence { get; private set; }

        /// <summary>
        /// The number of frames the CPU has finished to assemble for this swapchain.
        /// This is equivalent with the number of times EndFrame has been called.
        /// </summary>
        public long NumCompletedFramesCPU { get; private set; } = 0;

        /// <summary>
        /// The number of frames the GPU has finished to process
        /// </summary>
        public long NumCompletedFramesGPU => FrameFence.Value;

        /// <summary>
        /// Returns how many frames are currently in-flight.
        /// If a frame is currently assembled (between Begin/EndFrame call) this frame is NOT counted.
        /// </summary>
        /// <remarks>
        /// This means how many frames the CPU has prepared but are not yet completed by the GPU.
        /// Between BeginFrame and EndFrame this value always smaller than Desc.MaxFramesInFlight.
        /// If the application is heavily GPU bound, this function will return Desc.MaxFramesInFlight for the entire time outside of Begin/EndFrame.
        /// </remarks>
        public long NumFramesInFlight => NumCompletedFramesCPU - NumCompletedFramesGPU;


        /// <summary>
        /// The index of the frame that is currently about to be assembled (or will be next).
        /// Looping between 0 including and Desc.MaxFramesInFlight excluding.
        /// </summary>
        private uint activeInFlightFrameIndex = 0;


        public delegate void BeginFrameCallback(uint activeFlightFrameIndex);

        /// <summary>
        /// Triggered on BeginFrame.
        /// </summary>
        public event BeginFrameCallback OnBeginFrame;

        /// <summary>
        /// Waits until only Desc.MaxFramesInFlight-1 frames are inflight and will then trigger the OnBeginRendering event.
        /// If an application is highly GPU bound, it will wait for the GPU to complete the rendering for this swapChain in this function.
        /// </summary>
        public void BeginFrame()
        {
            // Fullfill the MaxFramesInFlight constraint.
            Assert.Debug(NumFramesInFlight <= Desc.MaxFramesInFlight, "It should be impossible to have more than Desc.MaxFramesInFlight frames in flight.");
            if (NumFramesInFlight == Desc.MaxFramesInFlight)
            {
                FrameFence.WaitForCompletion(NumCompletedFramesGPU + 1, TimeSpan.MaxValue);
            }

            // Anounce frame start to all listeners.
            OnBeginFrame?.Invoke(activeInFlightFrameIndex);
        }

        /// <summary>
        /// Swaps backbuffer. If syncInterval is bigger than zero, this function must wait until the GPU is able to perform the swap. (TODO: Is that correct?)
        /// </summary>
        /// <remarks>
        /// Signals the internal frameFence that is used to determine how many frames are inflight.
        /// Does not call any additional wait function (like WaitForFreeInflightFrame).
        /// </remarks>
        /// <param name="syncInterval">Synchronize presentation after the nth vertical blank. 0 means no synchronization.</param>
        public void EndFrame(uint syncInterval = 1)
        {
            // Present and mark end of frame with fence signal.
            Present(syncInterval);
            ++NumCompletedFramesCPU;
            FrameFence.Signal(NumCompletedFramesCPU);

            activeInFlightFrameIndex = (activeInFlightFrameIndex + 1) % Desc.MaxFramesInFlight;
        }

        protected abstract void Present(uint syncInterval = 1);


        /// <summary>
        /// Waits until all prepared frames are renderd and the GPU has no more tasks.
        /// Use this method carefully since it will usually come with a huge stall!
        /// </summary>
        void WaitUntilAllFramesCompleted()
        {
            if (NumCompletedFramesGPU != NumCompletedFramesCPU)
            {
                FrameFence.WaitForCompletion(NumCompletedFramesCPU, TimeSpan.MaxValue);
            }
        }

        #endregion

        #region BackBuffer

        public abstract uint ActiveSwapChainBufferIndex { get; }

        #endregion

        /// <summary>
        /// Needs to be called by implementor.
        /// </summary>
        protected override void DestroyImpl()
        {
            WaitUntilAllFramesCompleted();
            FrameFence.RemoveRef();
            FrameFence.Destroy();
        }
    }
}
