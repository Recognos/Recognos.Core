namespace Recognos.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// A wait handle like struct witch combines an AutoResetEvent and a ManualResetEvent
    /// </summary>
    internal sealed class ResetEvent : IDisposable
    {
        /// <summary>
        /// Array of handles to wait on.
        /// </summary>
        private WaitHandle[] handles;

        /// <summary>
        /// The auto event.
        /// </summary>
        private EventWaitHandle autoEvent;

        /// <summary>
        /// The manual event.
        /// </summary>
        private EventWaitHandle manualEvent;

        /// <summary>
        /// Flag indicating that the current instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetEvent"/> class.
        /// </summary>
        public ResetEvent()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetEvent"/> class.
        /// </summary>
        /// <param name="initialState">The initial state of the event.</param>
        public ResetEvent(bool initialState)
        {
            autoEvent = new AutoResetEvent(initialState);
            manualEvent = new ManualResetEvent(initialState);
            handles = new WaitHandle[] { autoEvent, manualEvent };
        }

        /// <summary>
        /// Waits for a signal
        /// </summary>
        public void Wait()
        {
            WaitHandle.WaitAny(handles);
        }

        /// <summary>
        /// Wake one waiting thread
        /// </summary>
        public void WakeOne()
        {
            autoEvent.Set();
        }

        /// <summary>
        /// Wake all waiting threads. This method does not reset the event. The <see cref="Reset"/> method must be called
        /// to reset the event.
        /// </summary>
        public void WakeAll()
        {
            manualEvent.Set();
        }

        /// <summary>
        /// Reset the event to the non-signaled state
        /// </summary>
        public void Reset()
        {
            manualEvent.Reset();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            autoEvent.Close();
            manualEvent.Close();
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}