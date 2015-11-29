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
    }
}
