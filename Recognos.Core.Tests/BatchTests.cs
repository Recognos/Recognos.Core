using System;
using System.Linq;
using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{
    public class BatchTests
    {
        [Fact]
        public void Batch_SplitsCollectionIntoExpectedBatches()
        {
            var data = Enumerable.Range(0, 5);

            for (int i = 1; i < 10; i++)
            {
                data.Batch(i).Sum(b => b.Count()).Should().Be(data.Count());
            }

            Assert.Throws<ArgumentException>(() => data.Batch(0).ToArray());
            data.Batch(1).Should().Equal(data.Select(d => new[] { d }), (x, y) => x.SequenceEqual(y));
            data.Batch(2).Should().Equal(new[] { new[] { 0, 1 }, new[] { 2, 3 }, new[] { 4 } }, (x, y) => x.SequenceEqual(y));
        }
    }
}
