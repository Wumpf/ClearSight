namespace ClearSight.RendererAbstract.CommandSubmission
{
    public abstract class Fence : DeviceChild<Fence.Descriptor>
    {
        public struct Descriptor
        {
        }

        protected Fence(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
