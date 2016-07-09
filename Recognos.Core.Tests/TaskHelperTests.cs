using System;
using FluentAssertions;
using Recognos.Core;
using Xunit;
using System.Threading.Tasks;

namespace Recognos.Test.Core
{
    public class TaskHelperTests
    {
        [Fact]
        public void RunWithRetry_ActionIsExecuted()
        {
            int i = 0;
            int count = 0;

            Action action = () => { i = 1; count++; };

            Retry.RunWithRetry(action, 5);

            i.Should().Be(1);
            count.Should().Be(1);
        }

        [Fact]
        public void RunWithRetry_RetryOnException()
        {
            int i = 0;

            Action action = () => { i++; throw new InvalidOperationException(); };

            bool result = false;
            try
            {
                Retry.RunWithRetry(action, 5);
                result = true;
            }
            catch
            {
            }

            i.Should().Be(5);
            result.Should().BeFalse();
        }

        [Fact]
        public void RunWithRetry_RetryOnExceptionOnce()
        {
            int i = 0;
            bool exception = false;

            Action action = () => { i++; if (i < 2) { exception = true; throw new InvalidOperationException(); } };

            Retry.RunWithRetry(action, 5);

            i.Should().Be(2);
            exception.Should().BeTrue();
        }

        [Fact]
        public void RunWithRetry_RetryOnExceptionOnceWithErrorAction()
        {
            int i = 0;
            bool exception = false;
            Exception x = null;
            int count = -1;

            Action action = () => { i++; if (i < 2) { exception = true; throw new InvalidOperationException(); } };
            Action<Exception, int> error = (e, c) => { x = e; count = c; };

            Retry.RunWithRetry(action, 5, error);

            i.Should().Be(2);
            count.Should().Be(1);
            exception.Should().BeTrue();
        }

        [Fact]
        public void RunWithRetryFunction_RetryOnExceptionOnceWithErrorAction()
        {
            int i = 0;
            bool exception = false;
            Exception x = null;
            int count = -1;

            Func<int> action = () => { i++; if (i < 2) { exception = true; throw new InvalidOperationException(); } else return 10; };
            Action<Exception, int> error = (e, c) => { x = e; count = c; };

            var result = Retry.RunWithRetry(action, 5, error);

            result.Should().Be(10);

            i.Should().Be(2);
            count.Should().Be(1);
            exception.Should().BeTrue();
        }

        [Fact]
        public async Task RunWithRetryAsync_ActionIsExecuted()
        {
            int i = 0;
            int count = 0;

            Func<Task> action = async () =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                i = 1;
                count++;
            };

            await Retry.RunWithRetryAsync(action, 5).ConfigureAwait(false);

            i.Should().Be(1);
            count.Should().Be(1);
        }

        [Fact]
        public async Task RunWithRetryAsync_RetryOnException()
        {
            int i = 0;

            Func<Task> action = async () =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                i++;
                throw new InvalidOperationException();
            };

            bool result = false;
            try
            {
                await Retry.RunWithRetryAsync(action, 5).ConfigureAwait(false);
                result = true;
            }
            catch
            {
            }

            i.Should().Be(5);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RunWithRetryAsync_RetryOnExceptionOnce()
        {
            int i = 0;
            bool exception = false;

            Func<Task> action = async () =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                i++;
                if (i < 2)
                {
                    exception = true;
                    throw new InvalidOperationException();
                }
            };

            await Retry.RunWithRetryAsync(action, 5).ConfigureAwait(false);

            i.Should().Be(2);
            exception.Should().BeTrue();
        }

        [Fact]
        public async Task RunWithRetryAsync_RetryOnExceptionOnceWithErrorAction()
        {
            int i = 0;
            bool exception = false;
            Exception x = null;
            int count = -1;

            Func<Task> action = async () =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                i++;
                if (i < 2)
                {
                    exception = true;
                    throw new InvalidOperationException();
                }
            };
            Func<Exception, int, Task> error = async (e, c) =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                x = e;
                count = c;
            };

            await Retry.RunWithRetryAsync(action, 5, error).ConfigureAwait(false);

            i.Should().Be(2);
            count.Should().Be(1);
            exception.Should().BeTrue();
        }

        [Fact]
        public async Task RunWithRetryAsyncFunction_RetryOnExceptionOnceWithErrorAction()
        {
            int i = 0;
            bool exception = false;
            Exception x = null;
            int count = -1;

            Func<Task<int>> action = async () =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                i++;
                if (i < 2)
                {
                    exception = true;
                    throw new InvalidOperationException();
                }
                else return 10;
            };
            Func<Exception, int, Task> error = async (e, c) =>
            {
                await Task.FromResult(0).ConfigureAwait(false);
                x = e;
                count = c;
            };

            var result = await Retry.RunWithRetryAsync(action, 5, error).ConfigureAwait(false);

            result.Should().Be(10);

            i.Should().Be(2);
            count.Should().Be(1);
            exception.Should().BeTrue();
        }
    }
}
