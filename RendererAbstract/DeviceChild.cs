using System;
using System.Dynamic;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Threading;

namespace ClearSight.RendererAbstract
{
    /// <summary>
    /// Base class for all objects that are created and managed by the device.
    /// </summary>
    public abstract class DeviceChild<TDescriptor> : IDisposable
        where TDescriptor : struct
    {
        /// <summary>
        /// The descriptor, set by Device.Create
        /// </summary>
        public TDescriptor Desc { get; internal set; }

        /// <summary>
        /// Device from which this DeviceChild was created. Set by Device.Create
        /// </summary>
        public Device Device { get; set; }

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
            CreateImpl();
        }

        protected abstract void CreateImpl();

        /// <summary>
        /// Destroys the resource. Called by Dispose or RemoveRef if the device child is no longer in use.
        /// </summary>
        internal void Destroy()
        {
            DestroyImpl();
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
                ClearSight.Core.Assert.Always(CurrentState == State.Normal,
                    "It is only possible to lock intact device childs.");
                ClearSight.Core.Assert.Debug(usageCount < 0, "Invalid lock count!");

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
                ClearSight.Core.Assert.Always(CurrentState == State.Normal,
                    "It is only possible to unlock intact device childs.");
                ClearSight.Core.Assert.Debug(usageCount <= 0, "Invalid lock count!");

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
        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (InUse)
                {
                    CurrentState = State.WaitingForUnlockToDispose;
                }
                else
                {
                    Destroy();
                    CurrentState = State.Invalid;
                    GC.SuppressFinalize(this);
                }
            }
        }


        ~DeviceChild()
        {
            ClearSight.Core.Assert.Debug(CurrentState == State.Invalid, "DeviceChild was not disposed explicitely!");
            ClearSight.Core.Assert.Debug(!InUse, "DeviceChild is still in use during finalization!");
        }
    }
}
