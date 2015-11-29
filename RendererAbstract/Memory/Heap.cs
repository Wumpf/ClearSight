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
    }
}
