using FluentAssertions;
using Recognos.Core;
using Xunit;

namespace Recognos.Test.Core
{

    public class ObjectExtensions
    {
        [Fact]
        public void Object_AsEnumberable()
        {
            string x = "x";
            x.AsEnumerable().Should().Equal(new string[] { x });
        }

    }
}
