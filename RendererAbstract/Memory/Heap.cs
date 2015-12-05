namespace ClearSight.RendererAbstract.Resources
{
    public abstract class Heap : DeviceChild<Heap.Descriptor>
    {
        public struct Descriptor
        {
            public enum Types
            {
                Default,
                Upload,
                Readback,
                //Custom // Not implemented for now.
            }

            public Types Type;
        }

        protected Heap(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
