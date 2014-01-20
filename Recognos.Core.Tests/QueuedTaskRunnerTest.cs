using System;
using System.Linq;
using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{
    /// <summary>
    /// Summary description for QueuedTaskTunnerTest
    /// </summary>

    public class QueuedTaskRunnerTest
    {
        [Fact]
        public void QueuedTaskRunnerRunTaskTest()
        {

            int nrTask = 100;
            int count = 0;

            using (TaskExecutor runner = new TaskExecutor(1))
            {
                for (int i = 0; i < nrTask; ++i)
                    runner.AddTask(() => count++);
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
            int baseThreadid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            using (TaskExecutor runner = new TaskExecutor(1))
            {
                runner.HasErrors.Should().BeFalse();
                runner.OnTaskError += (sender, args) =>
                    {
                        sender.Should().BeSameAs(runner);
                        args.TaskException.Should().BeOfType<InvalidOperationException>();
                        System.Threading.Thread.CurrentThread.ManagedThreadId.Should().Be(baseThreadid);
                        calledTimes++;
                        args.WasHandled = true;
                    };
                runner.OnThreadTaskError += (sender, args) =>
                    {
                        sender.Should().BeSameAs(runner);
                        args.TaskException.Should().BeOfType<InvalidOperationException>();
                        baseThreadid.Should().NotBe(System.Threading.Thread.CurrentThread.ManagedThreadId);

                        args.WasHandled = true;
                    };

                Enumerable.Range(0, 5).ToList().ForEach(n => runner.AddTask(action));
                runner.Finish();
                runner.HasErrors.Should().BeTrue();
            }
            calledTimes.Should().Be(5);

        }
    }
}
