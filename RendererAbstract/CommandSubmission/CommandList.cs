//#define SAVE_COMMAND_RECORDS

using System;
using System.Collections.Generic;
using ClearSight.Core;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ClearSight.RendererAbstract.Binding;
using ClearSight.RendererAbstract.Memory;


namespace ClearSight.RendererAbstract.CommandSubmission
{
    public enum CommandListType
    {
        Graphics,

        // Specialization/limitations not supported yet
        //Compute,
        //Copy,

        // Not implemented for now.
        //Bundle 
    };

    public abstract class CommandList : DeviceChild<CommandList.Descriptor>
    { 
        public struct Descriptor
        {
            public CommandListType Type;

            /// <summary>
            /// Attention, destorying the allocation the policy is up to the user.
            /// </summary>
            public ICommandListAllocationPolicy AllocationPolicy;
        }

        public bool Recording { get; private set; } = false;

        public CommandAllocator ActiveAllocator { get; private set; } = null;


        protected CommandList(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Assert.Debug(desc.AllocationPolicy.Type == desc.Type, "Allocation policy allocator type needs to match command list type!");
            ActiveAllocator = desc.AllocationPolicy.GetCurrentAllocator();
            Assert.Debug(ActiveAllocator.Desc.Type == desc.Type, "Allocator type needs to match command list type!");

            Create();
        }

        #region Recording User Interface

        public void StartRecording()
        {
            Assert.Always(!Recording, "CommandList is already in recording state!");

            Recording = true;

            ActiveAllocator = Desc.AllocationPolicy.GetCurrentAllocator();
            Assert.Debug(ActiveAllocator.Desc.Type == Desc.Type, "Allocator type needs to match command list type!");
            ActiveAllocator.AddRef();
            ActiveAllocator.ResetResourceUse();

            // Reset redundancy check state.
            descriptorHeapColor = null;
            slotsColor = null;
            descriptorHeapDepthStencil = null;
            slotDepthStencil = UInt32.MaxValue;

            StartRecordingImpl();
        }
        public abstract void StartRecordingImpl();

        #region State

        public enum BarrierType
        {
            SplitBegin,
            SplitEnd,
            Immediate
        };

        private const uint AllSubResources = 0xffffffff;

        public void PerformResourceTransition(BarrierType barrierType, Resource resource, ResourceUsage usageBefore, ResourceUsage usageAfter, uint subresource = AllSubResources)
        {
            Debug.Assert(resource != null, "No resource given to transition.");
            Debug.Assert(usageBefore != usageAfter, "Usage before and afterwards is the same.");

            PerformResourceTransitionImpl(barrierType, resource, usageBefore, usageAfter, subresource);
        }
        public abstract void PerformResourceTransitionImpl(BarrierType barrierType, Resource resource, ResourceUsage usageBefore, ResourceUsage usageAfter, uint subresource);

        public void SetRenderTargets(DescriptorHeap descriptorHeapColor, uint slotColor, DescriptorHeap descriptorHeapDepthStencil, uint slotDepthStencil)
        {
            SetRenderTargets(descriptorHeapColor, new uint[] {slotColor}, descriptorHeapDepthStencil, slotDepthStencil);
        }

        /// <summary>
        /// You need to make sure manually that all associated resources are in the RenderTargetColor / DepthWrite state.
        /// </summary>
        /// <param name="descriptorHeapColor">Can be null if no color target should be bound</param>
        /// <param name="slotsColor"></param>
        /// <param name="descriptorHeapDepthStencil">Can be null if no depth target should be bound.</param>
        /// <param name="slotDepthStencil"></param>
        public void SetRenderTargets(DescriptorHeap descriptorHeapColor, uint[] slotsColor, DescriptorHeap descriptorHeapDepthStencil, uint slotDepthStencil)
        {
            // Validitiy checks.
            Debug.Assert(descriptorHeapColor == null || descriptorHeapColor.Desc.Type == DescriptorHeap.Descriptor.ResourceDescriptorType.RenderTarget, "Descriptor heap for render target needs to be a rendertarget descriptor heap.");
#if DEBUG
            if (descriptorHeapColor != null)
            {
                Debug.Assert(slotsColor != null, "No descriptor slots given for render target color.");
                foreach (uint slot in slotsColor)
                {
                    Debug.Assert(descriptorHeapColor.Desc.NumResourceDescriptors >= slot, "Invalid descriptor heap slot.");
                }
            }
#endif
            Debug.Assert(descriptorHeapDepthStencil == null || descriptorHeapDepthStencil.Desc.Type == DescriptorHeap.Descriptor.ResourceDescriptorType.RenderTarget, 
                "Descriptor heap for depth needs to be a rendertarget descriptor heap.");
            Debug.Assert(descriptorHeapDepthStencil == null || descriptorHeapDepthStencil.Desc.NumResourceDescriptors < slotDepthStencil, "Invalid descriptor heap slot.");


            // Redundancy check.
            bool sameColor = descriptorHeapColor == this.descriptorHeapColor &&
                                slotsColor.SequenceEqual(this.slotsColor);
            bool sameDs = descriptorHeapDepthStencil == this.descriptorHeapDepthStencil &&
                          slotDepthStencil == this.slotDepthStencil;

            if (sameColor && sameDs)
            {
                return;
            }
            if (sameDs)
            {
                descriptorHeapDepthStencil = null;
            }
            else if (sameColor)
            {
                descriptorHeapColor = null;
                slotsColor = null;
            }

            // State change.
            this.descriptorHeapColor = descriptorHeapColor;
            this.slotsColor = slotsColor;
            this.descriptorHeapDepthStencil = descriptorHeapDepthStencil;
            this.slotDepthStencil = slotDepthStencil;
            ActiveAllocator.RegisterResourceUse(descriptorHeapColor);
            ActiveAllocator.RegisterResourceUse(descriptorHeapDepthStencil);

            // Specific impl.
            SetRenderTargetsImpl(descriptorHeapColor, slotsColor, descriptorHeapDepthStencil, slotDepthStencil);
        }
        protected abstract void SetRenderTargetsImpl(DescriptorHeap descriptorHeapColor, uint[] slotsColor, DescriptorHeap descriptorHeapDepthStencil, uint slotDepthStencil);

        #endregion

        #region Commands

        [Flags]
        public enum ClearTarget
        {
            Color = 1,
            DepthStencil = 2,
            ColorDepthStencil = Color | DepthStencil
        }

        /// <summary>
        /// Performs clear operation on active render target.
        /// </summary>
        public void ClearRenderTargetView(ClearTarget clearTarget, Vector4 clearColor, float clearDepth = 1.0f, byte clearStencil = 0)
        {
            Debug.Assert((clearTarget & ClearTarget.Color) == 0 || descriptorHeapColor != null, "No color target bound to clear!");
            Debug.Assert((clearTarget & ClearTarget.DepthStencil) == 0 || descriptorHeapDepthStencil != null, "No depth/stencil target bound to clear!");
            ClearRenderTargetViewImpl(clearTarget, clearColor, clearDepth, clearStencil);
        }
        protected abstract void ClearRenderTargetViewImpl(ClearTarget clearTarget, Vector4 clearColor, float clearDepth, byte clearStencil);

        #endregion


        public void EndRecording()
        {
            Assert.Always(Recording, "CommandList is was not in recording state!");
            EndRecordingImpl();
            Recording = false;

            //ActiveAllocator.OnEndRecording();
            ActiveAllocator.RemoveRef();
        }
        public abstract void EndRecordingImpl();

        #endregion

        #region State

        protected DescriptorHeap descriptorHeapColor;
        protected uint[] slotsColor;
        protected DescriptorHeap descriptorHeapDepthStencil;
        protected uint slotDepthStencil;

        #endregion
    }
}
