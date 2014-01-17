using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Recognos.Core.Threading;
using Xunit;

namespace Recognos.Test.Core
{
    public class ParalelTaskRunnerTest
    {

        [Fact]
        public void ParalelRunnerTest()
        {
            int task = 100;
            int sum = 0;

            using (TaskExecutor runner = new TaskExecutor())
            {
                object pad = new object();
                Enumerable.Range(0, task).ToList().ForEach(num =>
                    runner.AddTask(() =>
                        {
                            lock (pad) { sum++; }
                        }));
                runner.Finish();
            }
            sum.Should().Be(task);
        }

        [Fact]
        public void ParalelRunnerTestTaskCount()
        {
            using (TaskExecutor runner = new TaskExecutor())
            {

                int count = 10;

                Enumerable.Range(0, count).ToList().ForEach(num => runner.AddTask(() => Thread.Sleep(100)));

                runner.TotalAddedTasks.Should().Be(count);
                runner.RemainingTasks.Should().Be(count);
                runner.ErrorCount.Should().Be(0);
                runner.FinishedTasksCount.Should().Be(0);

                runner.FlushTasks();

                runner.TotalAddedTasks.Should().Be(count);
                runner.RemainingTasks.Should().Be(0);
                runner.FinishedTasksCount.Should().Be(count);
                runner.ErrorCount.Should().Be(0);

                runner.Finish();

                runner.TotalAddedTasks.Should().Be(count);
                runner.RemainingTasks.Should().Be(0);
                runner.FinishedTasksCount.Should().Be(count);
                runner.ErrorCount.Should().Be(0);
            }
        }


        [Fact]
        public void ParalelRunnerFlushTasksTest()
        {
            for (int count = 0; count < 20; count++)
            {
                using (TaskExecutor runner = new TaskExecutor())
                {
                    int i = 0;
                    double r;

                    Enumerable.Range(0, count).ToList().ForEach(num =>
                        runner.AddTask(() =>
                        {
                            for (int j = 0; j < 1000; j++)
                            {
                                r = Math.Sin(0.234234) * Math.Atan(j);
                            }
                            Interlocked.Increment(ref i);
                        }));

                    (i <= count).Should().BeTrue();

                    runner.FlushTasks();

                    i.Should().Be(count);

                    runner.Finish();

                    i.Should().Be(count);
                }
            }
        }

        [Fact]
        public void ParalelRunnerFlushTest10Threads()
        {
            using (TaskExecutor runner = new TaskExecutor(10))
            {

                int i = 0;

                Enumerable.Range(0, 10).ToList().ForEach(n =>
                    runner.AddTask(() =>
                    {
                        if (n % 2 == 0)
                            Thread.SpinWait(int.MaxValue / 200);
                        Interlocked.Increment(ref i);
                    })
                );

                (i <= 10).Should().BeTrue();

                runner.FlushTasks();

                i.Should().Be(10);

                runner.Finish();

                i.Should().Be(10);
            }
        }

        [Fact]
        public void ParalelRunnerFlushTestAutoThreads()
        {
            using (TaskExecutor runner = new TaskExecutor())
            {

                int i = 0;

                int count = 10;

                Enumerable.Range(0, count).ToList().ForEach(n =>
                    runner.AddTask(() =>
                    {
                        if (n % 2 == 0)
                            Thread.SpinWait(int.MaxValue / 200);
                        Interlocked.Increment(ref i);
                    })
                );

                (i <= count).Should().BeTrue();

                runner.FlushTasks();

                i.Should().Be(count);

                runner.Finish();

                i.Should().Be(count);
            }
        }

        [Fact]
        public void ParalelRunnerFlushTestThreadSleep()
        {
            using (TaskExecutor runner = new TaskExecutor())
            {

                int i = 0;

                Enumerable.Range(0, 10).ToList().ForEach(n =>
                    runner.AddTask(() =>
                    {
                        if (n % 2 == 0)
                            Thread.Sleep(100);
                        Interlocked.Increment(ref i);
                    })
                );

                (i <= 10).Should().BeTrue();

                runner.FlushTasks();

                i.Should().Be(10);

                runner.Finish();

                i.Should().Be(10);
            }
        }


        [Fact]
        public void ParalelRunnerExceptionTest()
        {
            using (TaskExecutor runner = new TaskExecutor())
            {

                InvalidOperationException x = new InvalidOperationException();

                runner.AddTask(() => { throw x; });

                ParallelExecutionException pex = null;

                try
                {
                    runner.Finish();
                }
                catch (ParallelExecutionException ex)
                {
                    pex = ex;
                }
                pex.Errors.Single().WasHandled.Should().BeFalse();
                x.Should().Be(pex.Errors.Single().TaskException);
            }
        }

        [Fact]
        public void ParalelRunnerHandeledExceptionTest()
        {
            using (TaskExecutor runner = new TaskExecutor())
            {
                runner.OnTaskError += (s, e) => e.WasHandled = true;

                InvalidOperationException x = new InvalidOperationException();

                runner.AddTask(() => { throw x; });

                Assert.DoesNotThrow(() => runner.Finish());
            }
        }

        [Fact]
        public void ParalelRunnerStressTest()
        {
            // 1000 increments with 2ms sleep each on 2 threads
            // 1000 * 2ms / 2threads = 1000ms min wait

            using (TaskExecutor runner = new TaskExecutor(2))
            {

                int val = 0;

                Enumerable.Range(0, 1000).ToList().ForEach(n => runner.AddTask(() => { Thread.Sleep(2); Interlocked.Increment(ref val); }));

                runner.FlushTasks();

                val.Should().Be(1000);
                runner.Finish();
            }
        }

        [Fact]
        public void ParallelRunnerTestMultipleThreads()
        {
            using (TaskExecutor runner = new TaskExecutor(5))
            {

                int times = 1000;
                int taskCount = 10000;
                int currentTask = 0;
                int val = 0;

                Action task = null;
                task = () =>
                {
                    Interlocked.Increment(ref currentTask);
                    Interlocked.Increment(ref val);
                    if (currentTask <= taskCount)
                        runner.AddTask(task);
                    Thread.SpinWait((taskCount % 30) * 10000);
                };


                Enumerable.Range(0, times).ToList().ForEach(n => runner.AddTask(task));

                runner.FlushTasks();

                val.Should().Be(taskCount + times);
            }
        }
    }
}
