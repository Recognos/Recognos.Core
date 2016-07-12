using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Recognos.Core;
using Xunit;
using System.Threading.Tasks;

namespace Recognos.Test.Core
{
    public class DataExtensionsTest
    {
        [Fact]
        public void DataExtensions_Compress_Decompress()
        {
            const string sample = "This is a test class for ConversionExtensionsTest and is intended to contain all ConversionExtensionsTest Unit Tests";
            byte[] data = Encoding.UTF8.GetBytes(sample);
            byte[] compressed = data.Compress();
            byte[] decompressed = compressed.Decompress();
            string output = Encoding.UTF8.GetString(decompressed);

            decompressed.Should().Equal(data);
            output.Should().Be(sample);
        }

        [Fact]
        public async Task DataExtensions_Compress_Decompress_Async()
        {
            const string sample = "This is a test class for ConversionExtensionsTest and is intended to contain all ConversionExtensionsTest Unit Tests";
            byte[] data = Encoding.UTF8.GetBytes(sample);
            byte[] compressed = await data.CompressAsync().ConfigureAwait(false);
            byte[] decompressed = await compressed.DecompressAsync().ConfigureAwait(false);
            string output = Encoding.UTF8.GetString(decompressed);

            decompressed.Should().Equal(data);
            output.Should().Be(sample);
        }

        [Fact]
        public void DataExtensions_ToHexaTest()
        {
            byte[] data = { 0, 1, 15, 255 };
            const string expected = "00010FFF";

            DataExtensions.ToHexa(data).Should().Be(expected);
        }

        [Fact]
        public void DataExtensions_ToHexaTestThrowOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => DataExtensions.ToHexa(null));
        }

        [Fact]
        public void DataExtensions_ToFileSizeThrowsOnNegativeInput()
        {
            Assert.Throws<ArgumentException>(() => (-1).ToFileSize());
        }

        [Fact]
        public void DataExtensions_ToFileSizeConvertToHumanReadable()
        {
            100.ToFileSize().Should().Be("100 Bytes");
            80530636.ToFileSize().Should().Be("76.8 MB");
        }

        enum TestEnum { A, B, C };

        [Fact]
        public void DataExtensions_ToEnumTest()
        {
            0.ToEnum<TestEnum>().Should().Be(TestEnum.A);
            1.ToEnum<TestEnum>().Should().Be(TestEnum.B);
            2.ToEnum<TestEnum>().Should().Be(TestEnum.C);
        }

        [Fact]
        public void DataExtensions_ToEnumThrowOnBadValueTest()
        {
            Assert.Throws<ArgumentException>(() => (-1).ToEnum<TestEnum>());
        }

        [Fact]
        public void DataExtensions_ToEnumThrowsOnBadTypeTest()
        {
            Assert.Throws<InvalidOperationException>(() => 0.ToEnum<int>());
        }

        [Fact]
        public void FileUtilities_ReadBinaryContent()
        {
            byte[] data = Encoding.UTF8.GetBytes("test data");

            byte[] result;
            using (MemoryStream input = new MemoryStream(data))
            {
                result = input.ReadBinaryContent();
            }

            result.Should().Equal(data);
        }

        [Fact]
        public async Task FileUtilities_ReadBinaryContentAsync()
        {
            byte[] data = Encoding.UTF8.GetBytes("test data");

            byte[] result;
            using (MemoryStream input = new MemoryStream(data))
            {
                result = await input.ReadBinaryContentAsync().ConfigureAwait(false);
            }

            result.Should().Equal(data);
        }
    }
}
