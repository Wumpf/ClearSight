using System;
using System.Diagnostics;
using ClearSight.Core;
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

        public void ExecuteCommandList(CommandList commandList)
        {
            ExecuteCommandLists(new CommandList[] { commandList });
        }

        public void ExecuteCommandLists(CommandList[] commandLists)
        {
            foreach (CommandList list in commandLists)
            {
                Assert.Debug(!list.Recording, "Cannot execute a command list which is still in the recording state!");
            }
            
            ExecuteCommandListsImpl(commandLists);
        }

        protected abstract void ExecuteCommandListsImpl(CommandList[] commandLists);
    }
}
