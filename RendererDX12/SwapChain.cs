using ClearSight.RendererAbstract.Binding;

namespace ClearSight.RendererDX12
{
    public class SwapChain : RendererAbstract.SwapChain
    {
        public SwapChain(Descriptor desc) : base(ref desc)
        {
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
