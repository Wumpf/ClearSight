using System.Collections.Generic;
using ClearSight.RendererAbstract.Binding;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    /// <summary>
    /// Allocator for command list. 
    /// </summary>
    /// <remarks>
    /// Allocators are usually not created by the user directly, but via a CommandListAllocationPolicy.
    /// </remarks>
    public abstract class CommandAllocator : DeviceChild<CommandAllocator.Descriptor>
    {
        public struct Descriptor
        {
            public CommandListType Type;
        }

        protected CommandAllocator(ref CommandAllocator.Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }

        internal void RegisterResourceUse(DeviceChildBase usedResource)
        {
            if (usedResource != null)
            {
                usedResource.AddRef();
                usedResources.Add(usedResource);
            }
        }

        internal void ResetResourceUse()
        {
            foreach (var usedResource in usedResources)
            {
                usedResource.RemoveRef();
            }
            usedResources.Clear();
        }

        internal override void Destroy()
        {
            ResetResourceUse();
            base.Destroy();
        }

        private List<DeviceChildBase> usedResources = new List<DeviceChildBase>();
    }
}
