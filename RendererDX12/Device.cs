using System;
using ClearSight.Core.Log;
using ClearSight.RendererAbstract.Binding;
using ClearSight.RendererAbstract.CommandSubmission;
using SharpDX.DXGI;

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

            // Todo: Gather different infos for user and internal use
            // e.g. descriptor sizes, padding etc.


            Log.Info("Successfully created a DX12 device with adapter \"{0}\"", Adapter.Description.Description);
        }

        protected override void CopyDescriptorsImpl(DescriptorHeap source, DescriptorHeap destination, Tuple<uint>[] rangeStarts, uint numDescriptors)
        {
            throw new NotImplementedException();
        }

        #region Create
        public override ClearSight.RendererAbstract.SwapChain Create(ref SwapChain.Descriptor desc, string label = "<unnamed swapChain>")
        {
            return new SwapChain(ref desc, this, label);
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