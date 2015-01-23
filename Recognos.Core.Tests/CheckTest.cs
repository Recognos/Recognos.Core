namespace Recognos.Test.Core
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Recognos.Core;
    using Xunit;

    public class CheckTest
    {
        [Fact]
        public void PositiveTest1()
        {
            Assert.Throws<ArgumentException>(() => Check.Positive(-1, "x"));
        }

        [Fact]
        public void PositiveTest2()
        {
            Assert.Throws<ArgumentException>(() => Check.Positive(int.MinValue, "x"));
        }

        [Fact]
        public void PositiveTest3()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.Positive(0, "x");
                Check.Positive(int.MaxValue, "x");
            });
        }

        [Fact]
        public void PositiveTest4()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.Positive(0m, "x");
                Check.Positive(decimal.MaxValue, "x");
            });
        }

        [Fact]
        public void PositiveTest5()
        {
            Assert.Throws<ArgumentException>(() => Check.Positive(decimal.MinusOne, "x"));
        }

        [Fact]
        public void PositiveTest6()
        {
            Assert.Throws<ArgumentException>(() => Check.Positive(decimal.MinValue, "x"));
        }

        [Fact]
        public void AbsolutePositiveTest1()
        {
            Assert.Throws<ArgumentException>(() => Check.AbsolutePositive(-1, "x"));
        }

        [Fact]
        public void AbsolutePositiveTest2()
        {
            Assert.Throws<ArgumentException>(() => Check.AbsolutePositive(int.MinValue, "x"));
        }

        [Fact]
        public void AbsolutePositiveTest3()
        {
            Assert.Throws<ArgumentException>(() => Check.AbsolutePositive(0, "x"));
        }

        [Fact]
        public void AbsolutePositiveTest4()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.AbsolutePositive(1, "x");
                Check.AbsolutePositive(int.MaxValue, "x");
            });
        }

        [Fact]
        public void AbsolutePositiveTest5()
        {
            Assert.DoesNotThrow(() =>
            {
                decimal value = 1;
                Check.AbsolutePositive(value, "x");
                Check.AbsolutePositive(decimal.MaxValue, "x");
            });
        }

        [Fact]
        public void AbsolutePositiveTest6()
        {
            Assert.Throws<ArgumentException>(() => Check.AbsolutePositive(decimal.Zero, "x"));
        }

        [Fact]
        public void AbsolutePositiveTest7()
        {
            Assert.Throws<ArgumentException>(() => Check.AbsolutePositive(decimal.MinusOne, "x"));
        }

        [Fact]
        public void NotNullTest()
        {
            Assert.Throws<ArgumentNullException>(() => Check.NotNull((string)null, "x"));
        }

        [Fact]
        public void NotNullTest1()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.NotNull(new object(), "x");
                Check.NotNull(string.Empty, "x");
            });
        }

        [Fact]
        public void NotEmptyStringTest1()
        {
            Assert.Throws<ArgumentException>(() => Check.NotEmpty(null, "x"));
        }

        [Fact]
        public void NotEmptyStringTest2()
        {
            Assert.Throws<ArgumentException>(() => Check.NotEmpty(string.Empty, "x"));
        }
        
        [Fact]
        public void NotEmptyStringTest4()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.NotEmpty("x", "x");
            });
        }

        [Fact]
        public void NotEmptyCollectionTest2()
        {
            Assert.Throws<ArgumentException>(() => Check.NotEmpty<object>(new object[] { }, "x"));
        }

        [Fact]
        public void NotEmptyCollectionTest3()
        {
            Assert.Throws<ArgumentException>(() => Check.NotEmpty<object>(new List<object>(), "x"));
        }

        [Fact]
        public void NotEmptyCollectionTest4()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.NotEmpty(new object[] { new object() }, "x");
            });
        }

        [Fact]
        public void ConditionTest2()
        {
            Assert.Throws<InvalidOperationException>(() => Check.Condition(false, "asd"));
        }

        [Fact]
        public void ConditionTest5()
        {
            Assert.DoesNotThrow(() =>
            {
                Check.Condition(true, "asd");
            });
        }

        #region Helper classes for checking injected members
        internal interface InjectedMember
        {
        }

        internal class ConcreteMember : InjectedMember { }

        internal abstract class BaseTest
        {
            private readonly InjectedMember basemember;

            protected BaseTest(InjectedMember m)
            {
                basemember = m;
                Check.InjectedMembers(this);
            }
        }

        internal class ConcreteTest : BaseTest
        {
            private readonly InjectedMember concretemember;

            public ConcreteTest(InjectedMember test)
                : base(test)
            {
                this.concretemember = test;
                Check.InjectedMembers(this);
            }
        }

        internal class ConcreteTestThrow : BaseTest
        {
            private readonly InjectedMember concretemember;

            public ConcreteTestThrow(InjectedMember test)
                : base(test)
            {
                this.concretemember = null;
                Check.InjectedMembers(this);
                if (this.concretemember != null)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        internal class InjectionTest
        {
            private readonly InjectedMember member;

            public InjectionTest(InjectedMember test)
            {
                member = test;
            }
        }
        #endregion

        [Fact]
        public void CheckInjectionTest()
        {
            InjectionTest test = new InjectionTest(null);
            Assert.Throws<InvalidOperationException>(() => Check.InjectedMembers(test));
        }

        [Fact]
        public void CheckInjectionTest1()
        {
            InjectionTest test = new InjectionTest(new ConcreteMember());
            Assert.DoesNotThrow(() =>
            {
                Check.InjectedMembers(test);
            });
        }

        [Fact]
        public void Check_InjectedMembers_TestBaseClass()
        {
            Assert.DoesNotThrow(() =>
            {
                new ConcreteTest(new ConcreteMember());
            });
        }

        [Fact]
        public void Check_InjectedMembers_TestBaseClass_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new ConcreteTestThrow(new ConcreteMember()));
        }

        [Fact]
        public void Check_ConfitionWithFormatMessage()
        {
            string msg = null;
            try
            {
                Check.Condition(false, "format {0}", "x");
            }
            catch (InvalidOperationException x)
            {
                msg = x.Message;
            }

            msg.Should().Be("format x");
        }
    }
}
