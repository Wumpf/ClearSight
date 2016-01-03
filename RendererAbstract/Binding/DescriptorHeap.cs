using System.Collections.Generic;
using System.Diagnostics;
using ClearSight.RendererAbstract.Memory;

namespace ClearSight.RendererAbstract.Binding
{
    public abstract class DescriptorHeap : DeviceChild<DescriptorHeap.Descriptor>
    {
        public struct Descriptor
        {
            public enum ResourceDescriptorType
            {
                /// <summary>
                /// Textures, constant buffers and unordered access views.
                /// </summary>
                ShaderResource,
                Sampler,
                RenderTarget,
                DepthStencil
            }

            public ResourceDescriptorType Type;
            public uint NumResourceDescriptors;
        }

        protected Resource[] associatedResources;

        protected DescriptorHeap(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            associatedResources = new Resource[desc.NumResourceDescriptors];

            Create();
        }

        internal void SetAssociatedResource(uint slot, Resource resource)
        {
            // todo: Check if this slot is in use?
            associatedResources[slot] = resource;
        }
    }
}
