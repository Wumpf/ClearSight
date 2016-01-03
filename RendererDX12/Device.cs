using System;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract.Memory;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using CommandAllocator = ClearSight.RendererAbstract.CommandSubmission.CommandAllocator;
using CommandList = ClearSight.RendererAbstract.CommandSubmission.CommandList;
using CommandQueue = ClearSight.RendererAbstract.CommandSubmission.CommandQueue;
using DescriptorHeap = ClearSight.RendererAbstract.Binding.DescriptorHeap;
using Fence = ClearSight.RendererAbstract.CommandSubmission.Fence;
using Resource = ClearSight.RendererAbstract.Memory.Resource;
using RenderTargetViewDescription = ClearSight.RendererAbstract.Binding.RenderTargetViewDescription;

namespace ClearSight.RendererDX12
{
    public class Device : RendererAbstract.Device
    {
        /// <summary>
        /// Wraps the SharpDX feature level, so that applications do not need to add SharpDX as a dependency.
        /// </summary>
        public enum FeatureLevel
        {
            Level_9_1 = SharpDX.Direct3D.FeatureLevel.Level_9_1,
            Level_9_2 = SharpDX.Direct3D.FeatureLevel.Level_9_2,
            Level_9_3 = SharpDX.Direct3D.FeatureLevel.Level_9_3,
            Level_10_0 = SharpDX.Direct3D.FeatureLevel.Level_10_0,
            Level_10_1 = SharpDX.Direct3D.FeatureLevel.Level_10_1,
            Level_11_0 = SharpDX.Direct3D.FeatureLevel.Level_11_0,
            Level_11_1 = SharpDX.Direct3D.FeatureLevel.Level_11_1,
            Level_12_0 = SharpDX.Direct3D.FeatureLevel.Level_12_0,
            Level_12_1 = SharpDX.Direct3D.FeatureLevel.Level_12_1,
        }

        public SharpDX.Direct3D12.Device DeviceD3D12 { get; private set; }
        public SharpDX.DXGI.Adapter Adapter { get; private set; }

        
        private readonly int[] descriptorHandleIncrementSize = new int[Enum.GetValues(typeof(DescriptorHeap.Descriptor.ResourceDescriptorType)).Length];

        internal int GetDescriptorHandleIncrementSize(DescriptorHeap.Descriptor.ResourceDescriptorType type)
        {
            return descriptorHandleIncrementSize[(int)type];
        }

        public Device(ref Descriptor desc, FeatureLevel featureLevel = FeatureLevel.Level_12_0) : base(ref desc)
        {
            if (desc.DebugDevice)
            {
                var debugInterface = SharpDX.Direct3D12.DebugInterface.Get();
                if (debugInterface != null)
                {
                    debugInterface.EnableDebugLayer();
                }
                else
                {
                    Log.Error("Failed to obtain DX12 debug layer.");
                }
            }

            // Use first adapter that is supported.
            using (SharpDX.DXGI.Factory dxgiFactory = new Factory4())
            {
                for (int adapterIndex = 0;; ++adapterIndex)
                {
                    var adapter = dxgiFactory.GetAdapter(adapterIndex);
                    if (adapter == null)
                    {
                        // TODO: Throw exception
                        return;
                    }

                    try
                    {
                        DeviceD3D12 = new SharpDX.Direct3D12.Device(adapter, (SharpDX.Direct3D.FeatureLevel) featureLevel);
                        Adapter = adapter;
                        break;
                    }

                    catch (Exception)
                    {
                        DeviceD3D12 = null;
                        adapter.Dispose();
                    }
                }
            }

            // Get Resource handle increment sizes.
            descriptorHandleIncrementSize[(int)DescriptorHeap.Descriptor.ResourceDescriptorType.Sampler] = DeviceD3D12.GetDescriptorHandleIncrementSize(DescriptorHeapType.Sampler);
            descriptorHandleIncrementSize[(int)DescriptorHeap.Descriptor.ResourceDescriptorType.DepthStencil] = DeviceD3D12.GetDescriptorHandleIncrementSize(DescriptorHeapType.DepthStencilView);
            descriptorHandleIncrementSize[(int)DescriptorHeap.Descriptor.ResourceDescriptorType.RenderTarget] = DeviceD3D12.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            descriptorHandleIncrementSize[(int)DescriptorHeap.Descriptor.ResourceDescriptorType.ShaderResource] = DeviceD3D12.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

            Memory.Enums.InitLookupTables();

            Log.Info("Successfully created a DX12 device with adapter \"{0}\"", Adapter.Description.Description);
        }

        protected override void CopyDescriptorsImpl(DescriptorHeap source, DescriptorHeap destination, Tuple<uint>[] rangeStarts, uint numDescriptors)
        {
            throw new NotImplementedException();
        }

        protected override void CreateRenderTargetViewImpl(DescriptorHeap target, uint descriptorHeapSlot, Resource resource, ref RenderTargetViewDescription description)
        {
            var renderTargetViewDescription = new SharpDX.Direct3D12.RenderTargetViewDescription
            {
                Format = Memory.Enums.ToDXGIFormat[(int) description.Format],
                Dimension = Memory.Enums.ToRTVDimension[(int)description.Dimension]
            };
            switch (description.Dimension)
            {
                case Dimension.Buffer:
                    renderTargetViewDescription.Buffer.ElementCount = (int) description.Buffer.ElementCount;
                    renderTargetViewDescription.Buffer.FirstElement = (int) description.Buffer.FirstElement;
                    break;
                case Dimension.Texture1D:
                    renderTargetViewDescription.Texture1D.MipSlice = (int) description.Texture.MipSlice;
                    break;
                case Dimension.Texture1DArray:
                    renderTargetViewDescription.Texture1DArray.MipSlice = (int)description.Texture.MipSlice;
                    renderTargetViewDescription.Texture1DArray.FirstArraySlice = (int) description.Texture.FirstSlice;
                    renderTargetViewDescription.Texture1DArray.ArraySize = (int) description.Texture.SliceCount;
                    break;
                case Dimension.Texture2D:
                    renderTargetViewDescription.Texture2D.MipSlice = (int)description.Texture.MipSlice;
                    renderTargetViewDescription.Texture2D.PlaneSlice = 0; // Not yet supported.
                    break;
                case Dimension.Texture2DArray:
                    renderTargetViewDescription.Texture2DArray.MipSlice = (int)description.Texture.MipSlice;
                    renderTargetViewDescription.Texture2DArray.FirstArraySlice = (int)description.Texture.FirstSlice;
                    renderTargetViewDescription.Texture2DArray.ArraySize = (int)description.Texture.SliceCount;
                    renderTargetViewDescription.Texture2DArray.PlaneSlice = 0; // Not yet supported.
                    break;
                case Dimension.Texture2DMs:
                    // nothing
                    break;
                case Dimension.Texture2DMsArray:
                    renderTargetViewDescription.Texture2DMSArray.FirstArraySlice = (int)description.Texture.FirstSlice;
                    renderTargetViewDescription.Texture2DMSArray.ArraySize = (int)description.Texture.SliceCount;
                    break;
                case Dimension.Texture3D:
                    renderTargetViewDescription.Texture3D.MipSlice = (int)description.Texture.MipSlice;
                    renderTargetViewDescription.Texture3D.FirstDepthSlice = (int)description.Texture.FirstSlice;
                    renderTargetViewDescription.Texture3D.DepthSliceCount = (int)description.Texture.SliceCount;
                    break;
                default:
                    throw new NotImplementedException("Given render target memory dimension not yet supported!");
            }

            DeviceD3D12.CreateRenderTargetView(((Memory.Resource)resource).ResourceD3D12, renderTargetViewDescription, ((Binding.DescriptorHeap)target).GetCPUHandle(descriptorHeapSlot));
        }

        #region Create

        public override ClearSight.RendererAbstract.SwapChain Create(ref SwapChain.Descriptor desc, string label = "<unnamed swapChain>")
        {
            return new SwapChain(ref desc, this, label);
        }

        public override DescriptorHeap Create(ref DescriptorHeap.Descriptor desc, string label = "<unnamed descriptorHeap>")
        {
            return new Binding.DescriptorHeap(ref desc, this, label);
        }

        public override CommandAllocator Create(ref CommandAllocator.Descriptor desc, string label = "<unnamed commandAllocator>")
        {
            return new CommandSubmission.CommandAllocator(ref desc, this, label);
        }

        public override CommandQueue Create(ref CommandQueue.Descriptor desc, string label = "<unnamed commandQueue>")
        {
            return new CommandSubmission.CommandQueue(ref desc, this, label);
        }

        public override CommandList Create(ref CommandList.Descriptor desc, string label = "<unnamed commandList>")
        {
            return new CommandSubmission.CommandList(ref desc, this, label);
        }

        public override Fence Create(ref Fence.Descriptor desc, string label = "<unnamed fence>")
        {
            return new CommandSubmission.Fence(ref desc, this, label);
        }

        public override Resource Create(ref Resource.Descriptor desc, string label = "<unnamed resource>")
        {
            return new Memory.Resource(ref desc, this, label);
        }

        #endregion

        protected override void SetStablePowerStateImpl(bool enable)
        {
            DeviceD3D12.StablePowerState = enable;
            Log.Info("Destroyed DX12 device!");
        }

        /// <summary>
        /// Destroys the device.
        /// </summary>
        public override void Dispose()
        {
            DeviceD3D12.Dispose();
            Adapter.Dispose();
        }

        #region Command Allocator

        //SharpDX.Direct3D12.CommandAllocator

        #endregion
    }
}