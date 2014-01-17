using System;
using FluentAssertions;
using Recognos.Core;
using Xunit;

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

            var result = Retry.RunWithRetry<Exception, int>(action, 5, error);

            result.Should().Be(10);

            i.Should().Be(2);
            count.Should().Be(1);
            exception.Should().BeTrue();

        }
    }
}
