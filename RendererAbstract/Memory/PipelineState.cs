namespace ClearSight.RendererAbstract.Resources
{

    public abstract class GraphicsPipelineState : DeviceChild<GraphicsPipelineState.Descriptor>
    {
        public struct Descriptor
        {
        }

        protected GraphicsPipelineState(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }

    public abstract class ComputePipelineState : DeviceChild<ComputePipelineState.Descriptor>
    {
        public struct Descriptor
        {
        }

        protected ComputePipelineState(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
