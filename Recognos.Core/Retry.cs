namespace Recognos.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Utility class to retry an action in case of failure.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Default retry count
        /// </summary>
        public const int DefaultRetryCount = 3;

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void RunWithRetry(Action action)
        {
            RunWithRetry<Exception>(action, DefaultRetryCount);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        public static void RunWithRetry(Action action, int count)
        {
            RunWithRetry<Exception>(action, count);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static void RunWithRetry<TException>(Action action)
             where TException : Exception
        {
            RunWithRetry<TException>(action, DefaultRetryCount);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static void RunWithRetry<TException>(Action action, int count)
             where TException : Exception
        {
            RunWithRetry<TException>(action, count, (_, __) => { });
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        /// <param name="error">Action to perform on error</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static void RunWithRetry<TException>(Action action, int count, Action<TException, int> error)
            where TException : Exception
        {
            int i = 0;
            bool completed = false;

            while (!completed)
            {
                try
                {
                    action();
                    completed = true;
                }
                catch (TException ex)
                {
                    i++;
                    error(ex, i);
                    if (i == count)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">Type returned by the action</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        /// <param name="error">Action to perform on error</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static T RunWithRetry<TException, T>(Func<T> action, int count, Action<TException, int> error)
            where TException : Exception
        {
            int i = 0;

            do
            {
                try
                {
                    return action();
                }
                catch (TException ex)
                {
                    i++;
                    error(ex, i);
                    if (i == count)
                    {
                        throw;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">Type returned by the action</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static T RunWithRetry<TException, T>(Func<T> action, int count)
            where TException : Exception
        {
            return RunWithRetry<TException, T>(action, count, (_, __) => { });
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">Type returned by the action</typeparam>
        /// <param name="action">The action.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static T RunWithRetry<TException, T>(Func<T> action)
            where TException : Exception
        {
            return RunWithRetry<TException, T>(action, DefaultRetryCount, (_, __) => { });
        }
    }
}
