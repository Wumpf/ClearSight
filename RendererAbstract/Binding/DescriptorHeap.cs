using ClearSight.RendererAbstract.Memory;

namespace ClearSight.RendererAbstract.Binding
{
    public abstract class DescriptorHeap : DeviceChild<DescriptorHeap.Descriptor>
    {
        public struct Descriptor
        {
            public enum Types
            {
            }

            public Types Type;
        }

        protected DescriptorHeap(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
