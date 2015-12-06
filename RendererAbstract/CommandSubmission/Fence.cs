using System;

namespace ClearSight.RendererAbstract.CommandSubmission
{
    public abstract class Fence : DeviceChild<Fence.Descriptor>
    {
        public struct Descriptor
        {
            public long InitialValue;
        }

        protected Fence(ref Descriptor desc, Device device, string label) : base(ref desc, device, label)
        {
            Create();
        }

        /// <summary>
        /// Current completed value of the fence.
        /// </summary>
        public abstract long Value { get; }

        /// <summary>
        /// Signals the fence with the given value.
        /// </summary>
        public abstract void Signal(long value);

        /// <summary>
        /// Blocks until the fence has reached the given value or the timeout was reached.
        /// </summary>
        public abstract void WaitForCompletion(long value, TimeSpan timeout);
    }
}
