namespace Recognos.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Utility class to measure how much time a block of code takes.
    /// <code>
    /// using(new TimedAction( t => log.Info("action duration {0}",t))
    /// {
    ///     // block to measure duration
    /// }
    /// </code>
    /// </summary>
    public sealed class TimedAction : IDisposable
    {
        /// <summary>
        /// Action to execute
        /// </summary>
        private readonly Action<TimeSpan> action;

        /// <summary>
        /// Time keeper
        /// </summary>
        private readonly Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Flag indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public TimedAction(Action<TimeSpan> action)
        {
            this.action = action;
            this.watch.Start();
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

            action(watch.Elapsed);
            watch.Stop();
            disposed = true;
        }
    }
}
