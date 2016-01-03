using System;
using System.Dynamic;
using System.Reflection.Emit;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Threading;

namespace ClearSight.RendererAbstract
{
    /// <summary>
    /// Base class for all objects that are created and managed by the device.
    /// </summary>
    public abstract class DeviceChild<TDescriptor> : DeviceChildBase
        where TDescriptor : struct
    {
        /// <summary>
        /// The descriptor, set by Device.Create
        /// </summary>
        public TDescriptor Desc { get; private set; }

        protected DeviceChild(ref TDescriptor desc, Device device, string label) : base(device, label)
        {
            Desc = desc;
        }
    }

    public abstract class DeviceChildBase : IDisposable
    {
        /// <summary>
        /// Device from which this DeviceChild was created. Set by Device.Create
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        /// Device child label, useful for debugging.
        /// </summary>
        public string Label { get; private set; }

        protected DeviceChildBase(Device device, string label)
        {
            Device = device;
            Label = label;
        }

        #region State

        /// <summary>
        /// The creation state a device child can have.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Either there was an error during load, or the resource was already disposed and is waiting for garbage collection to pick it up.
            /// </summary>
            Invalid,

            /// <summary>
            /// Resource is ready to use.
            /// </summary>
            Normal,

            /// <summary>
            /// The resource was already disposed, but is currently locked.
            /// Since only loaded resources can be locked, this also means that the resource is loaded.
            /// </summary>
            WaitingForUnlockToDispose,
        }

        public State CurrentState { get; private set; } = State.Invalid;

        #endregion

        /// <summary>
        /// Called by the device, after setting Desc.
        /// </summary>
        internal void Create()
        {
            ClearSight.Core.Assert.Debug(CurrentState == State.Invalid, "It is only possible to create device children if their are uninitialized.");
            ClearSight.Core.Assert.Debug(!InUse, "It is not possible to create device children that are in use.");
            CreateImpl();

            CurrentState = State.Normal;
        }

        protected abstract void CreateImpl();

        /// <summary>
        /// Destroys the resource. Called by Dispose or RemoveRef if the device child is no longer in use.
        /// </summary>
        internal virtual void Destroy()
        {
            ClearSight.Core.Assert.Debug(CurrentState == State.Normal, "It is only possible to destroy intact device children.");
            ClearSight.Core.Assert.Debug(!InUse, "It is not possible to destroy device children that are in use.");
            DestroyImpl();
            CurrentState = State.Invalid;
        }

        protected abstract void DestroyImpl();

        #region Locking

        /// <summary>
        /// Determines if the resource has a ref count bigger than zero and cannot be destroyed immediately.
        /// </summary>
        public bool InUse => usageCount > 0;

        /// <summary>
        /// Number of active locks.
        /// </summary>
        private int usageCount = 0;

        /// <summary>Increases usage counter (threadsafe).</summary>
        /// <remarks>
        /// Locks the resource against delection. InUse will defer their destruction until all locks are lifted.
        /// Only resources in the Loaded or WaitingForUnlockToDispose state can be (un)locked
        /// </remarks>
        /// <see cref="RemoveRef"/>
        public void AddRef()
        {
            lock (this)
            {
                ClearSight.Core.Assert.Always(CurrentState == State.Normal, "It is only possible to lock intact device children.");
                ClearSight.Core.Assert.Debug(usageCount >= 0, "Invalid lock count!");

                ++usageCount;
            }
        }

        /// <summary>Decreases usage counter (threadsafe).</summary>
        /// <remarks>
        /// If the InUse is now false and the resource was waiting for dispose, dispose will be called again.
        /// Only resources in the Loaded or WaitingForUnlockToDispose state can be (un)locked.
        /// </remarks>
        /// <see cref="AddRef"/>
        public void RemoveRef()
        {
            lock (this)
            {
                ClearSight.Core.Assert.Always(CurrentState == State.Normal, "It is only possible to unlock intact device children.");
                ClearSight.Core.Assert.Debug(usageCount > 0, "Invalid lock count!");

                --usageCount;

                if (CurrentState == State.WaitingForUnlockToDispose && !InUse)
                {
                    ((IDisposable) this).Dispose();
                }
            }
        }

        #endregion

        /// <summary>
        /// Destroys the resource. If the resource is locked, it will be destroyed when all locks are lifted.
        /// Calls unload.
        /// </summary>
       public void Dispose()
        {
            ClearSight.Core.Assert.Debug(CurrentState == State.Normal, "It is only possible to destroy intact device children.");

            lock (this)
            {
                if (InUse)
                {
                    CurrentState = State.WaitingForUnlockToDispose;
                }
                else
                {
                    Destroy();
                    GC.SuppressFinalize(this);
                }
            }
        }


        ~DeviceChildBase()
        {
            ClearSight.Core.Assert.Debug(CurrentState == State.Invalid, "DeviceChild was not disposed explicitely!");
            ClearSight.Core.Assert.Debug(!InUse, "DeviceChild is still in use during finalization!");
        }
    }
}
