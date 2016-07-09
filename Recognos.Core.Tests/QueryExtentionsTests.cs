using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Recognos.Core;

namespace Recognos.Test.Core
{
    public class QueryExtentionsTests
    {
        private static readonly IQueryable<int> data = new List<int> { 1, 2, 3, 4, 5 }.AsQueryable();

        [Fact]
        public void QueryExtension_ApplyIf()
        {
            var result = data.ApplyIf(true, i => i == 1);
             result.Count().Should().Be(1);

            result = data.ApplyIf(false, i => i == 1);
             result.Count().Should().Be(data.Count());
        }

        [Fact]
        public void QueryExtension_ApplyIfNotNull()
        {
            var result = data.ApplyIfNotNull(new object(), i => i == 1);
             result.Count().Should().Be(1);

            result = data.ApplyIfNotNull((object)null, i => i == 1);
             result.Count().Should().Be(data.Count());
        }

        [Fact]
        public void QueryExtension_ApplyIfNotEmptyString()
        {
            var result = data.ApplyIfNotEmpty("asd", i => i == 1);
             result.Count().Should().Be(1);
            
            result = data.ApplyIfNotEmpty(null, i => i == 1);
             result.Count().Should().Be(data.Count());
        }

        [Fact]
        public void QueryExtension_ApplyIfNotEmptyGuid()
        {
            var result = data.ApplyIfNotEmpty(Guid.NewGuid(), i => i == 1);
             result.Count().Should().Be(1);

            result = data.ApplyIfNotEmpty(Guid.Empty, i => i == 1);
             result.Count().Should().Be(data.Count());
        }

        [Fact]
        public void QueryExtension_ApplyIfNotEmptyDate()
        {
            var result = data.ApplyIfNotEmpty(DateTime.Now, i => i == 1);
             result.Count().Should().Be(1);

            result = data.ApplyIfNotEmpty(new DateTime(), i => i == 1);
             result.Count().Should().Be(data.Count());
        }

        [Fact]
        public void QueryExtension_ApplyIfNotEmptyNullableInt()
        {
            var result = data.ApplyIfNotEmpty((int?)0, i => i == 1);
             result.Count().Should().Be(1);

            result = data.ApplyIfNotEmpty((int?)null, i => i == 1);
             result.Count().Should().Be(data.Count());
        }
    }
}
