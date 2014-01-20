namespace Recognos.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Helper class to run a set of tasks in parallel. 
    /// This class uses a number of worker threads witch will execute the queued tasks in parallel as much as possible.
    /// The worker threads are background threads and you must call Dispose() or Finish() to ensure all the tasks are finished
    /// </summary>
    public sealed class TaskExecutor : IDisposable
    {
        /// <summary>
        /// Pool of workers used
        /// </summary>
        private Thread[] workers;

        /// <summary>
        /// Event used to signal the workers
        /// </summary>
        private ResetEvent waitEvent = new ResetEvent();

        /// <summary>
        /// Wait event used to wait when flushing tasks
        /// </summary>
        private EventWaitHandle flushEvent = new AutoResetEvent(false);

        /// <summary>
        /// Queue of tasks to execute
        /// </summary>
        private ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();

        /// <summary>
        /// Queue of errors produced by running the tasks
        /// </summary>
        private ConcurrentQueue<TaskErrorEventArgs> errors = new ConcurrentQueue<TaskErrorEventArgs>();

        #region Fields for properties and flags

        /// <summary>
        /// Total number of added tasks
        /// </summary>
        private int addedTasksCount;

        /// <summary>
        /// Number of tasks for witch the execution has started
        /// </summary>
        private int startedTasksCount;

        /// <summary>
        /// Number of completed tasks
        /// </summary>
        private int completedTasksCount;

        /// <summary>
        /// Number of tasks that finished with unhandeled exception
        /// </summary>
        private int errorCount;

        /// <summary>
        /// Flag used to signal if finish has been called
        /// </summary>
        private bool finished;

        /// <summary>
        /// Flag indicating whether the instanec has been disposed
        /// </summary>
        private bool disposed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the TaskExecutor class.
        /// The WorkerCount is set to the Number of cores
        /// </summary>
        public TaskExecutor()
            : this(DefaultWorkerCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaskExecutor class.
        /// </summary>
        /// <param name="workerCount">Number of workers to use</param>
        public TaskExecutor(int workerCount)
            : this(workerCount, false, "ParallelRunner")
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaskExecutor class.
        /// </summary>
        /// <param name="workerCount">Worker count</param>
        /// <param name="highPriority">High priority workers</param>
        /// <param name="name">Name of this task runner ( used in thread names )</param>
        public TaskExecutor(int workerCount, bool highPriority, string name)
        {
            RunAsync = true;
            Name = name;

            WorkerCount = workerCount;
            workers = new Thread[WorkerCount];
            for (int i = 0; i < WorkerCount; i++)
            {
                workers[i] = new Thread(new ThreadStart(RunWorker));
                workers[i].IsBackground = true;
                workers[i].Name = string.Format(CultureInfo.InvariantCulture, "{0} Worker {1}", Name, i);
                if (highPriority)
                {
                    workers[i].Priority = ThreadPriority.AboveNormal;
                }

                workers[i].Start();
            }
        }

        #region Events and properties

        /// <summary>
        /// Event witch will be raised if a task throws an exception
        /// </summary>
        /// <remarks>
        /// Since the tasks are run on a worker thread and we whant this event
        /// to be raised on the runner's thread we can only raise it when Finish() is beeing called. 
        /// For an event that is raised on the worker thread <see cref="OnThreadTaskError"/>.
        /// The execution of the other queued tasks will continue evean if a task throws an exception.
        /// </remarks>
        public event EventHandler<TaskErrorEventArgs> OnTaskError;

        /// <summary>
        /// This event will be raised on the worker thread when a task throws an exception.
        /// The execution of the other queued tasks will continue evean if a task throws an exception.
        /// </summary>
        public event EventHandler<TaskErrorEventArgs> OnThreadTaskError;

        /// <summary>
        /// Gets the default worker count.
        /// </summary>
        /// <value>The default worker count.</value>
        public static int DefaultWorkerCount
        {
            get
            {
                return System.Environment.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets the number of workers
        /// </summary>
        public int WorkerCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether at least one of the tasks has thrown an error
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return errorCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tasks are runned async.
        /// </summary>
        /// <value><c>true</c> if tasks are run async; otherwise, <c>false</c>.</value>
        public bool RunAsync { get; set; }

        /// <summary>
        /// Gets the name of the runner
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the total number of tasks that have been aded to this runner
        /// </summary>
        public int TotalAddedTasks
        {
            get
            {
                return addedTasksCount;
            }
        }

        /// <summary>
        /// Gets the total number of tasks that have been started by this runner
        /// </summary>
        public int StartedTasksCount
        {
            get
            {
                return startedTasksCount;
            }
        }

        /// <summary>
        /// Gets the total number of tasks that this runner has executed succesfully
        /// </summary>
        public int FinishedTasksCount
        {
            get
            {
                return completedTasksCount;
            }
        }

        /// <summary>
        /// Gets the total number of tasks that ended with an unhandeled exception
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return errorCount;
            }
        }

        /// <summary>
        /// Gets the number of remaining tasks to execute
        /// </summary>
        public int RemainingTasks
        {
            get
            {
                return TotalAddedTasks - FinishedTasksCount - ErrorCount;
            }
        }

        #endregion

        /// <summary>
        /// Add a task to the queue
        /// </summary>
        /// <param name="task">Task to add</param>
        public void AddTask(Action task)
        {
            Check.NotNull(task, "task");
            Check.Condition(!finished, "This runner has already been terminated by calling Finish");

            if (!RunAsync)
            {
                task();
                return;
            }

            // increment the number of added tasks and queue the tasks
            Interlocked.Increment(ref addedTasksCount);

            tasks.Enqueue(task);

            // wake up one sleeping worker
            waitEvent.WakeOne();
        }

        /// <summary>
        /// Wait for all currenty added tasks to finish executing and continue waiting for tasks
        /// </summary>
        public void FlushTasks()
        {
            // if we still have active workers
            while (RemainingTasks > 0)
            {
                // all workers sould run
                waitEvent.WakeAll();

                // wait on the flushEvent
                flushEvent.WaitOne();
            }

            // workers can go to sleep
            waitEvent.Reset();
        }

        /// <summary>
        /// Wait for the tasks to finish executing.
        /// </summary>
        /// <remarks>
        /// After calling finish, you can't add any more tasks to this runner.
        /// </remarks>
        public void Finish()
        {
            if (finished)
            {
                return;
            }

            // signal the finished event
            finished = true;

            waitEvent.WakeAll();

            foreach (Thread worker in workers)
            {
                worker.Join();
            }

            List<TaskErrorEventArgs> finalErrors = new List<TaskErrorEventArgs>();
            TaskErrorEventArgs err = null;

            while (errors.TryDequeue(out err))
            {
                if (OnTaskError != null)
                {
                    OnTaskError(this, err);
                }

                if (!err.WasHandled)
                {
                    finalErrors.Add(err);
                }
            }

            if (finalErrors.Count > 0)
            {
                throw new AggregateException(finalErrors.Select(e => e.TaskException));
            }
        }

        /// <summary>
        /// Ensure the wait handles are closed
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Finish();
            waitEvent.Dispose();
            flushEvent.Close();
            flushEvent.Dispose();
            GC.SuppressFinalize(this);
            disposed = true;
        }

        /// <summary>
        /// Start running the tasks
        /// </summary>
        private void RunWorker()
        {
            // run until we are signaled to finish
            while (!finished)
            {
                // run queued tasks
                RunPendingTasks();

                // we are idle but are not finished 
                if (!finished)
                {
                    // signal the flushing event
                    flushEvent.Set();

                    // wait for new tasks 
                    waitEvent.Wait();
                }
            }

            // run remaining tasks
            RunPendingTasks();

            // signal the flushing event
            flushEvent.Set();
        }

        /// <summary>
        /// Run tasks in the queue
        /// </summary>
        private void RunPendingTasks()
        {
            Action task;
            while (tasks.TryDequeue(out task))
            {
                RunTask(task);
            }
        }

        /// <summary>
        /// Runs a task and monitors it for unhandeled exceptions
        /// </summary>
        /// <param name="task">Task to run</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We only need to handle errors from tasks on the main thread")]
        private void RunTask(Action task)
        {
            try
            {
                // increment the number of started tasks
                Interlocked.Increment(ref startedTasksCount);

                // run the actual task
                task();

                // increment the number of completed tasks
                Interlocked.Increment(ref completedTasksCount);
            }
            catch (Exception ex)
            {
                // increment the number of errors
                Interlocked.Increment(ref errorCount);

                TaskErrorEventArgs args = new TaskErrorEventArgs(task, ex);
                errors.Enqueue(args);
                if (OnThreadTaskError != null)
                {
                    OnThreadTaskError(this, args);
                }
            }
        }
    }
}
