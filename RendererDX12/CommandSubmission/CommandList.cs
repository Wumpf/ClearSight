using System;
using System.Numerics;
using ClearSight.RendererAbstract.CommandSubmission;
using SharpDX.Direct3D12;

using CommandListType = ClearSight.RendererAbstract.CommandSubmission.CommandListType;
using CommandListTypeDX = SharpDX.Direct3D12.CommandListType;

using DescriptorHeap = ClearSight.RendererAbstract.Binding.DescriptorHeap;
using DescriptorHeapDX = ClearSight.RendererDX12.Binding.DescriptorHeap;

using Resource = ClearSight.RendererAbstract.Memory.Resource;
using ResourceDX = ClearSight.RendererDX12.Memory.Resource;

namespace ClearSight.RendererDX12.CommandSubmission
{
    public partial class CommandList : ClearSight.RendererAbstract.CommandSubmission.CommandList
    {

        public SharpDX.Direct3D12.GraphicsCommandList CommandListD3D12 { get; private set; }

        // In DX12 command list and allocator start ready to record.
        private bool commandListOpen = true;

        internal CommandList(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            CommandListD3D12 = ((Device)Device).DeviceD3D12.CreateCommandList(InternalUtils.GetDXCommandListType(Desc.Type), ((CommandAllocator)ActiveAllocator).AllocatorD3D12, null);
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

        public override void PerformResourceTransitionImpl(BarrierType barrierType, Resource resource, ResourceUsage usageBefore, ResourceUsage usageAfter, uint subresource)
        {
            SharpDX.Direct3D12.ResourceStates before = InternalUtils.GetResourceStateDX(usageBefore);
            SharpDX.Direct3D12.ResourceStates after = InternalUtils.GetResourceStateDX(usageAfter);
            CommandListD3D12.ResourceBarrierTransition(((ResourceDX)resource).ResourceD3D12, (int) subresource, before, after);
        }

        protected override void SetRenderTargetsImpl(DescriptorHeap descriptorHeapColor, uint[] slotsColor, DescriptorHeap descriptorHeapDepthStencil, uint slotDepthStencil)
        {
            bool continuous = true;
            for (int i = 1; i < slotsColor.Length; ++i)
            {
                if (slotsColor[i] != slotsColor[i - 1] + 1)
                {
                    continuous = false;
                    break;
                } 
            }

            CpuDescriptorHandle? depthStencilCpuDescriptorHandle = ((DescriptorHeapDX)descriptorHeapDepthStencil)?.GetCPUHandle(slotDepthStencil);

            if (continuous)
            {
                CommandListD3D12.SetRenderTargets(slotsColor.Length, ((DescriptorHeapDX)descriptorHeapColor).GetCPUHandle(slotsColor[0]), true, depthStencilCpuDescriptorHandle);
            }
            else
            {
                throw new NotImplementedException("Setting color render targets with non-continuous descriptor slots are not yet supported.");
                //CommandListD3D12.SetRenderTargets(slotsColor.Length, ((Binding.DescriptorHeap)descriptorHeapColor).GetCPUHandle(slotsColor[0]), false, depthStencilCpuDescriptorHandle);
            }
        }

        // Streamout is not supported.

        #region Commands

        protected override void ClearRenderTargetViewImpl(ClearTarget clearTarget, Vector4 clearColor, float clearDepth, byte clearStencil)
        {
            if ((clearTarget & ClearTarget.Color) != 0)
            {
                var clearColorDx = InternalUtils.ConvVec4Color(clearColor);
                foreach (var slot in slotsColor)
                {
                    CommandListD3D12.ClearRenderTargetView(((DescriptorHeapDX)descriptorHeapColor).GetCPUHandle(slot), clearColorDx);
                }
            }
            if ((clearTarget & ClearTarget.DepthStencil) != 0)
            {
                CommandListD3D12.ClearDepthStencilView(((DescriptorHeapDX)descriptorHeapDepthStencil).GetCPUHandle(slotDepthStencil), 
                                                        ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, clearDepth, clearStencil);
            }
        }

        #endregion

        public override void EndRecordingImpl()
        {
            CommandListD3D12.Close();
            commandListOpen = false;
        }
    }
}
