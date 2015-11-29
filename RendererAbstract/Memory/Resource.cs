using ClearSight.RendererAbstract.Resources;
using ClearSight.Core;


namespace ClearSight.RendererAbstract.Memory
{
    abstract class Resource : DeviceChild<Heap.Descriptor>
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

        /// <summary>
        /// Heap on which this resource was created.
        /// If null the resource is either destroyed or is a commited resource.
        /// </summary>
        public Heap Heap { get; private set; } = null;
    }
}
