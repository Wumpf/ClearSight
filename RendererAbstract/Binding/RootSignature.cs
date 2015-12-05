namespace ClearSight.RendererAbstract.Binding
{
    public abstract class RootSignature : DeviceChild<RootSignature.Descriptor>
    {
        public struct Descriptor
        {
            public enum Types
            {
            }

            public Types Type;
        }

        protected RootSignature(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {

        }
    }
}
