using ClearSight.Core;
using System.Diagnostics;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    public enum CommandListType
    {
        Graphics,
        Compute,
        Copy,
        //Bundle // Not implemented for now.
    };

    public abstract class CommandList : DeviceChild<CommandList.Descriptor>
    { 
        public struct Descriptor
        {
            public CommandListType Type;

            /// <summary>
            /// Attention, destorying the allocation the policy is up to the user.
            /// </summary>
            public ICommandListAllocationPolicy AllocationPolicy;
        }

        public bool Recording { get; private set; } = false;

        public CommandAllocator ActiveAllocator { get; private set; } = null;

        protected CommandList(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Assert.Debug(desc.AllocationPolicy.Type == desc.Type, "Allocation policy allocator type needs to match command list type!");
            ActiveAllocator = desc.AllocationPolicy.GetCurrentAllocator();
            Assert.Debug(ActiveAllocator.Desc.Type == desc.Type, "Allocator type needs to match command list type!");

            Create();
        }

        public void StartRecording()
        {
            Assert.Always(!Recording, "CommandList is already in recording state!");

            Recording = true;

            ActiveAllocator = Desc.AllocationPolicy.GetCurrentAllocator();
            Assert.Debug(ActiveAllocator.Desc.Type == Desc.Type, "Allocator type needs to match command list type!");
            ActiveAllocator.AddRef();
            //ActiveAllocator.OnStartRecording();

            StartRecordingImpl();
        }
        public abstract void StartRecordingImpl();


        public void RecordCommand(ref Command command)
        {
           // ActiveAllocator.OnRecordCommand(ref command);
            RecordCommandImpl(ref command);
        }
        public abstract void RecordCommandImpl(ref Command command);


        public void EndRecording()
        {
            Assert.Always(Recording, "CommandList is was not in recording state!");
            EndRecordingImpl();
            Recording = false;

            //ActiveAllocator.OnEndRecording();
            ActiveAllocator.RemoveRef();
        }
        public abstract void EndRecordingImpl();
    }
}
