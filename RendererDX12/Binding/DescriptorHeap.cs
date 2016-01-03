using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearSight.Core;
using SharpDX.Direct3D12;

namespace ClearSight.RendererDX12.Binding
{
    public class DescriptorHeap : RendererAbstract.Binding.DescriptorHeap
    {
        public SharpDX.Direct3D12.DescriptorHeap DescriptorHeapDX12 { get; private set; }

        internal DescriptorHeap(ref Descriptor desc, RendererAbstract.Device device, string label) : base(ref desc, device, label)
        {
        }

        protected override void CreateImpl()
        {
            SharpDX.Direct3D12.DescriptorHeapDescription descDx = new SharpDX.Direct3D12.DescriptorHeapDescription
            {
                DescriptorCount = (int) Desc.NumResourceDescriptors,
                NodeMask = 0
            };

            switch (Desc.Type)
            {
                case Descriptor.ResourceDescriptorType.ShaderResource:
                    descDx.Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView;
                    descDx.Flags = DescriptorHeapFlags.ShaderVisible;
                    break;
                case Descriptor.ResourceDescriptorType.Sampler:
                    descDx.Type = DescriptorHeapType.Sampler;
                    descDx.Flags = DescriptorHeapFlags.ShaderVisible;
                    break;
                case Descriptor.ResourceDescriptorType.RenderTarget:
                    descDx.Type = DescriptorHeapType.RenderTargetView;
                    descDx.Flags = DescriptorHeapFlags.None;
                    break;
                case Descriptor.ResourceDescriptorType.DepthStencil:
                    descDx.Type = DescriptorHeapType.DepthStencilView;
                    descDx.Flags = DescriptorHeapFlags.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DescriptorHeapDX12 = ((Device) Device).DeviceD3D12.CreateDescriptorHeap(descDx);
        }

        protected override void DestroyImpl()
        {
            DescriptorHeapDX12.Dispose();
        }

        internal CpuDescriptorHandle GetCPUHandle(uint slot)
        {
            Assert.Debug(slot < Desc.NumResourceDescriptors, "Descriptorslot is out of range.");

            var handle = DescriptorHeapDX12.CPUDescriptorHandleForHeapStart;
            handle.Ptr += ((Device) Device).GetDescriptorHandleIncrementSize(Desc.Type) * slot;
            return handle;
        }
    }
}
