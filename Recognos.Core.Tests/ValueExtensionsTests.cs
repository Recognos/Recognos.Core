using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;using Xunit;

using Recognos.Core;

namespace Recognos.Test.Core
{
    
    public class ValueExtensionsTests
    {
        [Fact]
        public void ValueExtensions_GuidIsNullOrEmpty()
        {
            Guid empty = Guid.Empty;
            Guid defaultG = new Guid();
            Guid notEmpty = Guid.NewGuid();

            empty.IsDefault().Should().BeTrue();
            defaultG.IsDefault().Should().BeTrue();
            notEmpty.IsDefault().Should().BeFalse();
        }

        [Fact]
        public void ValueExtensions_DateTimeIsNullOrEmpty()
        {
            DateTime emptyDate = DateTime.MinValue;
            DateTime defaultG = new DateTime();
            DateTime notEmptyDate = DateTime.Now;

            emptyDate.IsDefault().Should().BeTrue();
            defaultG.IsDefault().Should().BeTrue();            
            notEmptyDate.IsDefault().Should().BeFalse();
        }
    }
}
