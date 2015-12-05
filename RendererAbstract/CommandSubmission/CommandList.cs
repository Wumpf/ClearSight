namespace ClearSight.RendererAbstract.CommandSubmission
{
    public abstract class CommandList : DeviceChild<CommandList.Descriptor>
    {
        public struct Descriptor
        {
            public enum Types
            {
                Graphics,
                Compute,
                Copy,
                //Bundle // Not implemented for now.
            }

            public Types Type;
        }

        protected CommandList(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
