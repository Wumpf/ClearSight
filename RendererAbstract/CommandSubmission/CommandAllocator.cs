namespace ClearSight.RendererAbstract.CommandSubmission
{
    /// <summary>
    /// Allocator for command list. 
    /// </summary>
    /// <remarks>
    /// Allocators are usually not created by the user directly, but via a CommandListAllocationPolicy.
    /// </remarks>
    public abstract class CommandAllocator : DeviceChild<CommandAllocator.Descriptor>
    {
        public struct Descriptor
        {
            public CommandListType Type;
        }

        protected CommandAllocator(ref CommandAllocator.Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }

        /// <summary>
        /// Called by the CommandList when recording starts.
        /// </summary>
        //internal abstract void OnStartRecording();

        //internal abstract void OnRecordCommand(ref Command command);

        /// <summary>
        /// Called by the CommandList when recording ends.
        /// </summary>
        //internal abstract void OnEndRecording();



    }
}
