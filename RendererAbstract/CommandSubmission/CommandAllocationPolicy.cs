using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearSight.Core;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    public interface ICommandListAllocationPolicy : IDisposable
    {
        /// <summary>
        /// Type to which this allocation policy applies.
        /// </summary>
        CommandListType Type { get; }

        CommandAllocator GetCurrentAllocator();
    }

    public class CommandListSingleAllocationPolicy : ICommandListAllocationPolicy
    {
        public CommandListType Type => Allocator.Desc.Type;

        public CommandAllocator Allocator { get; private set; }

        public CommandListSingleAllocationPolicy(CommandListType type, Device device)
        {
            var allocatorDesc = new CommandAllocator.Descriptor() {Type = type};
            Allocator = device.Create(ref allocatorDesc);
            Allocator.AddRef();
        }

        public CommandAllocator GetCurrentAllocator()
        {
            return Allocator;
        }

        ~CommandListSingleAllocationPolicy()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (Allocator != null)
            {
                Allocator.RemoveRef();
                Allocator.Dispose();
                Allocator = null;
            }
        }
    }


    /// <summary>
    /// CommandList allocation policy that creates SwapChain.Desc.MaxFramesInFlight allocators and changes the outputed allocator every BeginFrame.
    /// </summary>
    public class CommandListInFlightFrameAllocationPolicy : ICommandListAllocationPolicy
    {
        public CommandAllocator[] PerInflightFrameAllocators { get; private set; }
        public uint ActiveAllocator { get; private set; } = 0;

        private readonly SwapChain swapChain;

        public CommandListType Type => PerInflightFrameAllocators[0].Desc.Type;

        public CommandListInFlightFrameAllocationPolicy(CommandListType type, SwapChain swapChain)
        {
            var allocatorDesc = new CommandAllocator.Descriptor() { Type = type };
            PerInflightFrameAllocators = new CommandAllocator[swapChain.Desc.MaxFramesInFlight];
            for (int i = 0; i < swapChain.Desc.MaxFramesInFlight; ++i)
            {
                PerInflightFrameAllocators[i] = swapChain.Device.Create(ref allocatorDesc);
                PerInflightFrameAllocators[i].AddRef();
            }

            swapChain.OnBeginFrame += SetActiveAllocator;
            swapChain.AddRef(); // Swap chain is not allowed to die earlier than this object.
            this.swapChain = swapChain;
        }

        public CommandAllocator GetCurrentAllocator()
        {
            return PerInflightFrameAllocators[ActiveAllocator];
        }

        private void SetActiveAllocator(uint activeInFlightFrame)
        {
            Assert.Debug(activeInFlightFrame < PerInflightFrameAllocators.Length, "activeInFlightFrame index is higher than expected!");
            ActiveAllocator = activeInFlightFrame;
        }


        ~CommandListInFlightFrameAllocationPolicy()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (PerInflightFrameAllocators != null)
            {
                // Make sure that it is allowed to destory the Allocators.
                swapChain.WaitUntilAllFramesCompleted();
                swapChain.RemoveRef();
                swapChain.OnBeginFrame -= SetActiveAllocator;

                foreach (var alloc in PerInflightFrameAllocators)
                {
                    alloc.RemoveRef();
                    alloc.Dispose();
                }
                PerInflightFrameAllocators = null;  
            }
        }
    }
}
