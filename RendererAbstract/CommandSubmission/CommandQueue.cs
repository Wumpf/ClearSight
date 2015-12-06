using System;
using ClearSight.RendererAbstract;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    public abstract class CommandQueue : DeviceChild<CommandQueue.Descriptor>
    {
        public struct Descriptor
        {
            public CommandListType Type;
        }

        protected CommandQueue(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }
    }
}
