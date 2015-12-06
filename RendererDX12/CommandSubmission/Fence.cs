using System;
using System.Collections.Generic;
using System.Threading;
using ClearSight.RendererAbstract;
using SharpDX.Direct3D12;
using CommandListType = ClearSight.RendererAbstract.CommandSubmission.CommandListType;
using CommandListTypeDX = SharpDX.Direct3D12.CommandListType;

namespace ClearSight.RendererDX12.CommandSubmission
{
    public class Fence : ClearSight.RendererAbstract.CommandSubmission.Fence
    {

        public SharpDX.Direct3D12.Fence FenceD3D12 { get; private set; }

        /// <summary>
        /// Event handle is create on demand by WaitForCompletion and reused after the first time.
        /// </summary>
        public EventWaitHandle EventHandle { get; private set; } = null;


        internal Fence(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
            
        }

        protected override void CreateImpl()
        {
            FenceD3D12 = ((Device) Device).DeviceD3D12.CreateFence(Desc.InitialValue, FenceFlags.None);
            FenceD3D12.Name = Label;
        }

        protected override void DestroyImpl()
        {
            EventHandle.Dispose();
            FenceD3D12.Dispose();
        }

        public override long Value => FenceD3D12.CompletedValue;

        public override void Signal(long value)
        {
            FenceD3D12.Signal(value);
        }

        public override void WaitForCompletion(long value, TimeSpan timeout)
        {
            if (EventHandle == null)
            {
                EventHandle = new AutoResetEvent(false);
            }

            FenceD3D12.SetEventOnCompletion(value, EventHandle.SafeWaitHandle.DangerousGetHandle());
            EventHandle.WaitOne(timeout);
        }
    }
}
