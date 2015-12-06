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

        public SharpDX.Direct3D12.CommandList CommandListD3D12 { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Fence CompletionFence { get; private set; }

        /// <summary>
        /// List of command allocators.
        /// </summary>
        public List<SharpDX.Direct3D12.CommandAllocator> CommandAllocators { get; private set; }


        internal CommandList(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            
        }

        protected override void DestroyImpl()
        {
            
        }

        public override void StartRecordingImpl()
        {
           
        }

        public override void EndRecordingImpl()
        {
            
        }
    }
}
