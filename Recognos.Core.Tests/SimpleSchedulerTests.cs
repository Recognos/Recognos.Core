using System;
using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{
    public class SimpleSchedulerTests
    {
        [Fact]
        public void Scheduler_ExecutesAction()
        {
            bool wasCalled = false;

            // time to call the action
            TimeSpan time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(1));
            string nextSecond = time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture);

            using (SimpleScheduler scheduler = new SimpleScheduler(() => wasCalled = true, nextSecond))
            {
                scheduler.Start();

                System.Threading.Thread.Sleep(2000);

                // due to strange timings this test might fail. incrementing the wait time might help
                wasCalled.Should().BeTrue();
            }
        }

        [Fact]
        public void Scheduler_CanBeDisabled()
        {
            foreach (string time in SimpleScheduler.DisablingValues)
            {
                bool wasCalled = false;

                using (SimpleScheduler scheduler = new SimpleScheduler(() => wasCalled = true, time))
                {
                    scheduler.Start();

                    scheduler.IsRunning.Should().BeFalse();
                    wasCalled.Should().BeFalse();
                }
            }
        }

        [Fact]
        public void Scheduler_ThrowsExceptionOnEmptyTimeToRun()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (SimpleScheduler scheduler = new SimpleScheduler(() => { }, string.Empty)) { }
            });
        }

        [Fact]
        public void Scheduler_ThrowsExceptionOnUnknownTimeToRun()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (SimpleScheduler scheduler = new SimpleScheduler(() => { }, "10")) { }
            });
        }

        [Fact]
        public void Scheduler_ThrowsExceptionOnNullTimeToRun()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (SimpleScheduler scheduler = new SimpleScheduler(() => { }, null)) { }
            });
        }
    }
}
