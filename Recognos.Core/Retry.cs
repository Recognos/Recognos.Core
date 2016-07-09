namespace Recognos.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Utility class to retry an action in case of failure.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static void RunWithRetry(Action action, int count = 3)
        {
            RunWithRetry<Exception>(action, count);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static Task RunWithRetryAsync(Func<Task> action, int count = 3)
        {
            return RunWithRetryAsync<Exception>(action, count);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static void RunWithRetry<TException>(Action action, int count = 3)
             where TException : Exception
        {
            RunWithRetry<TException>(action, count, null);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static Task RunWithRetryAsync<TException>(Func<Task> action, int count = 3)
             where TException : Exception
        {
            return RunWithRetryAsync<TException>(action, count, null);
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
                    if (error != null)
                    {
                        error(ex, i);
                    }
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
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        /// <param name="error">Action to perform on error</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static async Task RunWithRetryAsync<TException>(Func<Task> action, int count, Func<TException, int, Task> error)
            where TException : Exception
        {
            int i = 0;
            bool completed = false;

            while (!completed)
            {
                try
                {
                    await action().ConfigureAwait(false);
                    completed = true;
                }
                catch (TException ex)
                {
                    i++;
                    if (error != null)
                    {
                        await error(ex, i).ConfigureAwait(false);
                    }
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
                    if (error != null)
                    {
                        error(ex, i);
                    }
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
        /// <param name="error">Action to perform on error</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static async Task<T> RunWithRetryAsync<TException, T>(Func<Task<T>> action, int count, Func<TException, int, Task> error)
            where TException : Exception
        {
            int i = 0;

            do
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (TException ex)
                {
                    i++;
                    if (error != null)
                    {
                        await error(ex, i).ConfigureAwait(false);
                    }
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
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static T RunWithRetry<TException, T>(Func<T> action, int count = 3)
            where TException : Exception
        {
            return RunWithRetry<TException, T>(action, count, null);
        }

        /// <summary>
        /// Runs an action and retries the task if an exception occurs.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="T">Type returned by the action</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The count.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The languages we target support optional parameters")]
        public static Task<T> RunWithRetryAsync<TException, T>(Func<Task<T>> action, int count = 3)
            where TException : Exception
        {
            return RunWithRetryAsync<TException, T>(action, count, null);
        }
    }
}
