using System;
using System.Collections.Generic;
using FluentAssertions;
using Recognos.Core;
using Xunit;
using System.Threading.Tasks;

namespace Recognos.Test.Core
{
    public class StringExtensionsTests
    {
        [Fact]
        public void StringExtensions_StripTags()
        {
            const string sample = "abc<tag>cde";
            const string expected = "abccde";

            sample.StripTags().Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_StripCommentTags()
        {
            const string sample = "abc<!--tag-->cde";
            const string expected = "abccde";

            sample.StripTags().Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_ReplaceTags()
        {
            const string sample = "abc<tag>cde";
            const string expected = "abc     cde";

            sample.StripTags(true).Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_ReplaceEmptyTags()
        {
            const string sample = "abc<>cde";
            const string expected = "abc  cde";
            sample.StripTags(true).Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_GzipCompressionDecompressTest()
        {
            const string input = "abcdefghijklmn0123456789";
            const string expected = "abcdefghijklmn0123456789";
            string compressedText = StringExtensions.GzipCompress(input);
            string actual = StringExtensions.GzipDecompress(compressedText);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task StringExtensions_GzipCompressionDecompressTest_Async()
        {
            string input = "abcdefghijklmn0123456789";
            string expected = "abcdefghijklmn0123456789";
            string compressedText = await StringExtensions.GzipCompressAsync(input).ConfigureAwait(false);
            string actual = await StringExtensions.GzipDecompressAsync(compressedText).ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_CaseInsensitiveEquals()
        {
            const string left = "abcdefghijklmn0123456789";
            const string right = "abcdefghijklmn0123456789";

            StringExtensions.CaseInsensitiveEquals(left, right).Should().BeTrue();
            StringExtensions.CaseSensitiveEquals(left, right).Should().BeTrue();

            StringExtensions.CaseInsensitiveEndsWith(left, right).Should().BeTrue();

            StringExtensions.CaseInsensitiveEquals(string.Empty, string.Empty).Should().BeTrue();
            StringExtensions.CaseSensitiveEquals(string.Empty, string.Empty).Should().BeTrue();


            StringExtensions.CaseInsensitiveEquals(null, null).Should().BeFalse();
            StringExtensions.CaseSensitiveEquals(null, null).Should().BeFalse();

            StringExtensions.CaseInsensitiveEquals(null, string.Empty).Should().BeFalse();
            StringExtensions.CaseSensitiveEquals(null, string.Empty).Should().BeFalse();


            StringExtensions.CaseInsensitiveEquals("A", "a").Should().BeTrue();
            StringExtensions.CaseSensitiveEquals("A", "a").Should().BeFalse();


            StringExtensions.CaseInsensitiveStartsWith("A", "a").Should().BeTrue();
            StringExtensions.CaseSensitiveStartsWith("A", "a").Should().BeFalse();

            StringExtensions.CaseInsensitiveEndsWith("A", "a").Should().BeTrue();
            StringExtensions.CaseSensitiveEndsWith("A", "a").Should().BeFalse();

            StringExtensions.CaseInsensitiveContains("A", "a").Should().BeTrue();
            StringExtensions.CaseSensitiveContains("A", "a").Should().BeFalse();
        }

        [Fact]
        public void StringExtensions_StripTags_Test()
        {
            const string test1 = "abc";
            string test2 = string.Empty;
            const string test3 = null;
            const string test4 = "<a>abc";
            const string test4out = "abc";
            const string test5 = "</a>abc";
            const string test5out = "abc";
            const string test6 = "<abc>";
            string test6out = string.Empty;

            test1.StripTags().Should().Be(test1);
            test2.StripTags().Should().Be(test2);
            test3.StripTags().Should().Be(test3);
            test4.StripTags().Should().Be(test4out);
            test5.StripTags().Should().Be(test5out);
            test6.StripTags().Should().Be(test6out);
        }

        [Fact]
        public void StringExtensions_JoinStringCollection_Test()
        {
            IEnumerable<string> input = null;
            input.Join(",").Should().BeEmpty();

            input = new List<string>() { "a", "b", "c" };
            input.Join(",").Should().Be("a,b,c");
        }

        enum TestEnum { A, B, C };

        [Fact]
        public void StringExtensions_ToEnumTest()
        {
            "A".ToEnum<TestEnum>().Should().Be(TestEnum.A);
            "B".ToEnum<TestEnum>().Should().Be(TestEnum.B);
            "C".ToEnum<TestEnum>().Should().Be(TestEnum.C);
        }

        [Fact]
        public void StringExtensions_ToEnumThrowOnBadValueTest()
        {
            Assert.Throws<ArgumentException>(() => "D".ToEnum<TestEnum>());
        }

        [Fact]
        public void StringExtensions_ToEnumThrowsOnBadTypeTest()
        {
            Assert.Throws<InvalidOperationException>(() => "A".ToEnum<int>());
        }

        [Fact]
        public void StringExtensions_Left()
        {
            "asd".Left(3).Should().Be("asd");
            string.Empty.Left(3).Should().BeEmpty();

            ((string)null).Left(4).Should().BeNull();

            "asdfghj".Left(3).Should().Be("asd");
            "asdfghj".Left(0).Should().BeEmpty();
        }

        [Fact]
        public void StringExtensions_Left_ThrowsOnNegativeSize()
        {
            Assert.Throws<ArgumentException>(() => "asd".Left(-1));
        }

        [Fact]
        public void StringExtensions_Right()
        {
            "asd".Right(3).Should().Be("asd");
            string.Empty.Right(3).Should().BeEmpty();

            ((string)null).Right(4).Should().BeNull();

            "asdfghj".Right(3).Should().Be("ghj");
            "asdfghj".Right(0).Should().BeEmpty();
        }

        [Fact]
        public void StringExtensions_Rght_ThrowsOnNegativeSize()
        {
            Assert.Throws<ArgumentException>(() => "asd".Right(-1));
        }

        [Fact]
        public void StringExtensions_Collection_Contains()
        {
            string[] source = { "abc", "ABC", "cde", null };

            source.CaseInsensitiveContains("abc").Should().BeTrue();
            source.CaseInsensitiveContains("ABC").Should().BeTrue();
            source.CaseInsensitiveContains("cde").Should().BeTrue();
            source.CaseInsensitiveContains("CDE").Should().BeTrue();

            source.CaseInsensitiveContains("ab").Should().BeFalse();
            source.CaseInsensitiveContains("cd").Should().BeFalse();

            source.CaseSensitiveContains("AB").Should().BeFalse();
            source.CaseSensitiveContains("cd").Should().BeFalse();

            source.CaseSensitiveContains("ABC").Should().BeTrue();
            source.CaseSensitiveContains("cde").Should().BeTrue();
            source.CaseSensitiveContains("CDE").Should().BeFalse();
        }

        [Fact]
        public void StringExtensions_RightAtWord_NegativeLength()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.RightAtWord(-1));
        }

        [Fact]
        public void StringExtensions_RightAtWord()
        {
            const string input = "aaa asd";

            string.Empty.RightAtWord(0).Should().BeEmpty();
            ((string)null).RightAtWord(10).Should().BeEmpty();

            input.RightAtWord(0).Should().BeEmpty();
            input.RightAtWord(1).Should().BeEmpty();
            input.RightAtWord(2).Should().BeEmpty();
            input.RightAtWord(3).Should().Be("asd");
            input.RightAtWord(4).Should().Be("asd");
            input.RightAtWord(5).Should().Be("asd");
            input.RightAtWord(6).Should().Be("asd");
            input.RightAtWord(7).Should().Be("aaa asd");
            input.RightAtWord(8).Should().Be("aaa asd");
        }

        [Fact]
        public void StringExtensions_LeftAtWord_NegativeLength()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.LeftAtWord(-1));
        }

        [Fact]
        public void StringExtensions_LeftAtWord()
        {
            const string input = "asd aaa";

            input.LeftAtWord(0).Should().BeEmpty();
            input.LeftAtWord(1).Should().BeEmpty();
            input.LeftAtWord(2).Should().BeEmpty();
            input.LeftAtWord(3).Should().Be("asd");
            input.LeftAtWord(4).Should().Be("asd");
            input.LeftAtWord(5).Should().Be("asd");
            input.LeftAtWord(6).Should().Be("asd");
            input.LeftAtWord(7).Should().Be("asd aaa");
            input.LeftAtWord(8).Should().Be("asd aaa");
        }

        [Fact]
        public void StringExtensions_LastWords_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.LastWords(-1));
        }

        [Fact]
        public void StringExtensions_LastWords()
        {
            const string input = "abc cde fgh";

            input.LastWords(0).Should().BeEmpty();
            input.LastWords(1).Should().Be("fgh");
            input.LastWords(2).Should().Be("cde fgh");
            input.LastWords(3).Should().Be(input);
            input.LastWords(4).Should().Be(input);
        }

        [Fact]
        public void StringExtensions_FirstWords_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.FirstWords(-1));
        }

        [Fact]
        public void StringExtensions_FirstWords()
        {
            const string input = "abc cde fgh";

            input.FirstWords(0).Should().BeEmpty();
            input.FirstWords(1).Should().Be("abc");
            input.FirstWords(2).Should().Be("abc cde");
            input.FirstWords(3).Should().Be(input);
            input.FirstWords(4).Should().Be(input);
        }

        [Fact]
        public void StringExtensions_PreviousLines_NegativePosition()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.PreviousLines(-1, 0));
        }

        [Fact]
        public void StringExtensions_PreviousLines_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.PreviousLines(0, -1));
        }

        [Fact]
        public void StringExtensions_PreviousLines_StartAtZero()
        {
            const string test = "sample";
            string result = test.PreviousLines(0, 10);
            result.Should().BeEmpty();
        }

        [Fact]
        public void StringExtensions_PreviousLines_StartAtOne()
        {
            const string test = "sample";
            string result = test.PreviousLines(1, 10);
            const string expected = "s";
            result.Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_PrevousLines()
        {
            const string input = "l1\nl2\nl3\nl4\nl5";
            int idx = input.IndexOf("l3") + 1;

            ((string)null).PreviousLines(0, 0).Should().BeEmpty();

            input.PreviousLines(0, 0).Should().BeEmpty();

            input.PreviousLines(idx, 0).Should().Be("l3");
            input.PreviousLines(idx, 1).Should().Be("l2\nl3");
            input.PreviousLines(idx, 2).Should().Be("l1\nl2\nl3");
            input.PreviousLines(idx, 3).Should().Be("l1\nl2\nl3");
        }

        [Fact]
        public void StringExtensions_NextLines_NegativePosition()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.NextLines(-1, 0));
        }

        [Fact]
        public void StringExtensions_NextLines_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.NextLines(0, -1));
        }

        [Fact]
        public void StringExtensions_NextLines_Test()
        {
            const string input = "aaa\nbbb";
            input.NextLines(5, 10).Should().Be("bb");
        }

        [Fact]
        public void StringExtensions_NextLines()
        {
            const string input = "l1\nl2\nl3\nl4\nl5";
            int idx = input.IndexOf("l3") + 1;

            ((string)null).NextLines(0, 0).Should().BeEmpty();

            input.NextLines(0, 0).Should().Be("l1");

            input.NextLines(idx, 0).Should().Be("3");
            input.NextLines(idx, 1).Should().Be("3\nl4");
            input.NextLines(idx, 2).Should().Be("3\nl4\nl5");
            input.NextLines(idx, 3).Should().Be("3\nl4\nl5");
        }

        [Fact]
        public void StringExtensions_FirstLines_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.FirstLines(-1));
        }

        [Fact]
        public void StringExtensions_FirstLines()
        {
            const string input = "l1\nl2\nl3\nl4\nl5";

            ((string)null).FirstLines(0).Should().BeEmpty();

            input.FirstLines(0).Should().BeEmpty();
            input.FirstLines(1).Should().Be("l1");
            input.FirstLines(2).Should().Be("l1\nl2");
            input.FirstLines(3).Should().Be("l1\nl2\nl3");
            input.FirstLines(4).Should().Be("l1\nl2\nl3\nl4");
            input.FirstLines(5).Should().Be("l1\nl2\nl3\nl4\nl5");
            input.FirstLines(6).Should().Be("l1\nl2\nl3\nl4\nl5");
        }

        [Fact]
        public void StringExtensions_LastLines_NegativeCount()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.LastLines(-1));
        }

        [Fact]
        public void StringExtensions_LastLines()
        {
            const string input = "l1\nl2\nl3\nl4\nl5";

            ((string)null).LastLines(0).Should().BeEmpty();

            input.LastLines(0).Should().BeEmpty();
            input.LastLines(1).Should().Be("l5");
            input.LastLines(2).Should().Be("l4\nl5");
            input.LastLines(3).Should().Be("l3\nl4\nl5");
            input.LastLines(4).Should().Be("l2\nl3\nl4\nl5");
            input.LastLines(5).Should().Be("l1\nl2\nl3\nl4\nl5");
            input.LastLines(6).Should().Be("l1\nl2\nl3\nl4\nl5");
        }

        [Fact]
        public void StringExtensions_AllIndexesOf()
        {
            const string input = "asd dsa asd";
            IEnumerable<int> expected = new int[] { 0, 8 };
            input.AllIndexesOf("asd").Should().Equal(expected);
        }

        [Fact]
        public void StringExtensions_AllIndexesOf_tags()
        {
            const string input = "asd dsa asd";
            IEnumerable<string> tags = new[] { "asd", "dsa" };

            var expected = new[] 
            {
                Tuple.Create(0, "asd"),
                Tuple.Create(4, "dsa"),
                Tuple.Create(8, "asd")
            };

            input.AllIndexesOf(tags).Should().Equal(expected);
        }

        [Fact]
        public void StringExtensions_Highlight_tags()
        {
            const string input = "asd dsa asd";
            IEnumerable<string> tags = new[] { "asd", "dsa" };

            const string expected = "*asd* *dsa* *asd*";

            input.Highlight("*{0}*", tags).Should().Be(expected);
        }

        [Fact]
        public void StringExtensions_Join_EmptyCollection()
        {
            string[] input = { };
            const string empty = "empty";

            input.Join(",", empty).Should().Be(empty);
        }

        [Fact]
        public void StringExtentions_StripHtml()
        {
            const string input = "<test/>a<test>a</test> &nbsp;&amp;&gt;&lt;&quot;&hellip;";
            string strip = input.StripHTML();
            const string expected = "aa  &><\"…";
            strip.Should().Be(expected);
        }
    }
}
