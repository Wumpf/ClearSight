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
    }
}
