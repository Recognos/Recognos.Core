namespace Recognos.Core.Threading
{
    using System;
    
    /// <summary>
    /// Event arguments for task errors
    /// </summary>
    public class TaskErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TaskErrorEventArgs class.
        /// </summary>
        /// <param name="task">Task that has generated the error</param>
        /// <param name="taskException">The exception that has been thrown</param>
        public TaskErrorEventArgs(Action task, Exception taskException)
        {
            Task = task;
            TaskException = taskException;
        }

        /// <summary>
        /// Gets the task that generated the error
        /// </summary>
        public Action Task { get; private set; }

        /// <summary>
        /// Gets the exception that has been thrown by the task
        /// </summary>
        public Exception TaskException { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exception was handeled
        /// </summary>
        public bool WasHandled { get; set; }
    }
}
