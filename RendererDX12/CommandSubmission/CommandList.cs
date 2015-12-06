using System;
using System.Collections.Generic;
using ClearSight.RendererAbstract;
using ClearSight.RendererAbstract.CommandSubmission;
using CommandListType = ClearSight.RendererAbstract.CommandSubmission.CommandListType;
using CommandListTypeDX = SharpDX.Direct3D12.CommandListType;

namespace ClearSight.RendererDX12.CommandSubmission
{
    public class CommandList : ClearSight.RendererAbstract.CommandSubmission.CommandList
    {
        public static CommandListTypeDX GetDXCommandListType(CommandListType type)
        {
            switch (type)
            {
                case CommandListType.Graphics:
                    return CommandListTypeDX.Direct;
                case CommandListType.Compute:
                    return CommandListTypeDX.Compute;
                case CommandListType.Copy:
                    return CommandListTypeDX.Copy;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public SharpDX.Direct3D12.GraphicsCommandList CommandListD3D12 { get; private set; }

        // In DX12 command list and allocator start ready to record.
        private bool commandListOpen = true;

        internal CommandList(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            CommandListD3D12 = ((Device)Device).DeviceD3D12.CreateCommandList(GetDXCommandListType(Desc.Type), ((CommandAllocator)ActiveAllocator).AllocatorD3D12, null);
            CommandListD3D12.Name = Label;
        }

        protected override void DestroyImpl()
        {
            CommandListD3D12.Dispose();
        }

        public override void StartRecordingImpl()
        {   
            if (!commandListOpen)
            {
                ((CommandAllocator) ActiveAllocator).AllocatorD3D12.Reset();
                CommandListD3D12.Reset(((CommandAllocator) ActiveAllocator).AllocatorD3D12, null);
                commandListOpen = true;
            }
        }

        public override void RecordCommandImpl(ref Command command)
        {
            throw new NotImplementedException();
        }

        public override void EndRecordingImpl()
        {
            CommandListD3D12.Close();
            commandListOpen = false;
        }
    }
}
