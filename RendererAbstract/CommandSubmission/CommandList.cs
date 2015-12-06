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
        }

        public bool Recording { get; private set; } = false;

        protected CommandList(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }

        public void StartRecording()
        {
            Assert.Always(!Recording, "CommandList is already in recording state!");
            StartRecordingImpl();
            Recording = true;

        }

        public abstract void StartRecordingImpl();

        public void EndRecording()
        {
            Assert.Always(Recording, "CommandList is was not in recording state!");
            EndRecordingImpl();
            Recording = false;
        }
        public abstract void EndRecordingImpl();
    }
}
