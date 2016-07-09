using System;
using System.Linq;
using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{
    public class QueuedTaskRunnerTest
    {
        [Fact]
        public void QueuedTaskRunnerRunTaskTest()
        {
            const int nrTask = 100;
            int count = 0;

            using (TaskExecutor runner = new TaskExecutor(1))
            {
                for (int i = 0; i < nrTask; ++i)
                {
                    runner.AddTask(() => count++);
                }
            }

            count.Should().Be(nrTask);
        }

        [Fact]
        public void QueuedTaskRunnerWorkerThreadTest()
        {
            TaskExecutor runner = new TaskExecutor(1);
            bool hasThrown = false;
            try
            {
                runner.Finish();
                runner.Finish();
                runner.Dispose();
            }
            catch
            {
                hasThrown = true;
            }
            hasThrown.Should().BeFalse();
        }

        [Fact]
        public void QueuedTaskRunnerErrorTest()
        {
            Action action = () => { throw new InvalidOperationException("boom"); };
            int calledTimes = 0;
            int baseThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            using (TaskExecutor runner = new TaskExecutor(1))
            {
                runner.HasErrors.Should().BeFalse();
                runner.OnTaskError += (sender, args) =>
                {
                    sender.Should().BeSameAs(runner);
                    args.TaskException.Should().BeOfType<InvalidOperationException>();
                    System.Threading.Thread.CurrentThread.ManagedThreadId.Should().Be(baseThreadId);
                    calledTimes++;
                    args.WasHandled = true;
                };
                runner.OnThreadTaskError += (sender, args) =>
                {
                    sender.Should().BeSameAs(runner);
                    args.TaskException.Should().BeOfType<InvalidOperationException>();
                    baseThreadId.Should().NotBe(System.Threading.Thread.CurrentThread.ManagedThreadId);

                    args.WasHandled = true;
                };

                foreach (var n in Enumerable.Range(0, 5))
                {
                    runner.AddTask(action);
                }
                runner.Finish();
                runner.HasErrors.Should().BeTrue();
            }
            calledTimes.Should().Be(5);
        }
    }
}
