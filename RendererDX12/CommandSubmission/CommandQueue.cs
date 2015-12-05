using System;
using SharpDX.Direct3D12;

namespace ClearSight.RendererDX12.CommandSubmission
{
    public class CommandQueue : RendererAbstract.CommandSubmission.CommandQueue
    {
        public SharpDX.Direct3D12.CommandQueue CommandQueueD3D12 { get; private set; }

        /// <exception cref="SharpDX.SharpDXException"></exception>
        protected override void CreateImpl()
        {
            var descd3d12 = new CommandQueueDescription
            {
                Priority = (int) CommandQueuePriority.Normal,
                Flags = CommandQueueFlags.None, // Disabling GPU timeout is not yet exposed.
                NodeMask = 0
            };

            switch (Desc.Type)
            {
                case Descriptor.Types.Graphics:
                    descd3d12.Type = CommandListType.Direct;
                    break;
                case Descriptor.Types.Compute:
                    descd3d12.Type = CommandListType.Compute;
                    break;
                case Descriptor.Types.Copy:
                    descd3d12.Type = CommandListType.Copy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CommandQueueD3D12 = ((Device) Device).DeviceD3D12.CreateCommandQueue(descd3d12);
            CommandQueueD3D12.Name = Label;
        }

        protected override void DestroyImpl()
        {
            CommandQueueD3D12.Dispose();
        }

        public CommandQueue(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
        }
    }
}
