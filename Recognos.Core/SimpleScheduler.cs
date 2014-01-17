namespace Recognos.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Simple scheduler class
    /// </summary>
    public sealed class SimpleScheduler : IDisposable
    {
        /// <summary>
        /// Array of values that can be passed as timeToRun to disable the timer
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly", Justification = "Array contents is immutable")]
        public static readonly string[] DisablingValues = new string[] { "never", "disabled", "none" };

        /// <summary>
        /// Field holding the task to be executed at the scheduled time
        /// </summary>
        private readonly Action taskToRun;

        /// <summary>
        /// Time to run the task
        /// </summary>
        private readonly TimeSpan timeToRun;

        /// <summary>
        /// Interval at witch to run the task
        /// </summary>
        private readonly TimeSpan interval;

        /// <summary>
        /// Flag signaling if the scheduler is disabled
        /// </summary>
        private readonly bool disabled;

        /// <summary>
        /// Time used to run the task
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Flag indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleScheduler"/> class.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <param name="timeToRun">The time to run.</param>
        public SimpleScheduler(Action task, string timeToRun)
            : this(task, timeToRun, TimeSpan.FromDays(1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleScheduler"/> class.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <param name="timeToRunTask">The time to run task.</param>
        /// <param name="interval">The interval.</param>
        public SimpleScheduler(Action task, string timeToRunTask, TimeSpan interval)
        {
            Check.NotNull(task, "task");
            Check.NotEmpty(timeToRunTask, "timeToRunTask");

            this.disabled = DisablingValues.Any(v => v.CaseInsensitiveEquals(timeToRunTask));

            if (!disabled)
            {
                if (!TimeSpan.TryParseExact(timeToRunTask, @"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture, out timeToRun))
                {
                    throw new ArgumentException(Format.Invariant("Bad value {0} for timeToRun, must be hh:mm:ss", timeToRunTask), "timeToRunTask");
                }

                this.taskToRun = task;
                this.interval = interval;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return !disabled && !disabled && timer != null;
            }
        }

        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public void Start()
        {
            if (disabled)
            {
                return;
            }

            if (timer != null)
            {
                throw new InvalidOperationException("Scheduler already running");
            }

            // hour:minute:second for today
            TimeSpan fromMidnight = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            // how much to wait before running
            TimeSpan dueTime;

            if (fromMidnight < timeToRun)
            {
                // we must run today
                dueTime = timeToRun - fromMidnight;
            }
            else
            {
                // hour has passed today, run tomorrow
                TimeSpan untilTomorrow = DateTime.Today.AddDays(1) - DateTime.Now;
                dueTime = untilTomorrow + timeToRun;
            }

            timer = new Timer(new TimerCallback((o) => RunTask()), null, dueTime, interval);
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public void Stop()
        {
            if (disabled)
            {
                return;
            }

            timer.Dispose();
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

            Stop();
            GC.SuppressFinalize(this);
            disposed = true;
        }

        /// <summary>
        /// Runs the scheduled task.
        /// </summary>
        private void RunTask()
        {
            taskToRun();
        }
    }
}
