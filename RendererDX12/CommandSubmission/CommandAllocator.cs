using ClearSight.RendererAbstract.CommandSubmission;

namespace ClearSight.RendererDX12.CommandSubmission
{ 
    public class CommandAllocator : ClearSight.RendererAbstract.CommandSubmission.CommandAllocator
    {
        public SharpDX.Direct3D12.CommandAllocator AllocatorD3D12 { get; private set; }


        internal CommandAllocator(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
        }

        /// <exception cref="SharpDX.SharpDXException"></exception>
        protected override void CreateImpl()
        {
            AllocatorD3D12 = ((Device)Device).DeviceD3D12.CreateCommandAllocator(InternalUtils.GetDXCommandListType(Desc.Type));
            AllocatorD3D12.Name = Label;
        }

        protected override void DestroyImpl()
        {
            AllocatorD3D12.Dispose();
        }
    }
}
