using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Recognos.Core.Tests
{
    public class PBKDF2SaltedHashTests
    {
        [Fact]
        public void GeneratedHashShoudStartAlgorithmPrefix()
        {
            var hash = PBKDF2SaltedHash.GenerateHash("test");
            hash.Should().StartWith("PBKDF2");
        }

        [Fact]
        public void GeneratedHashShoudContainIterationsCount()
        {
            var hash = PBKDF2SaltedHash.GenerateHash("test");
            int algo = "PBKDF2".Length;
            int iterations = int.Parse(hash.Substring(algo, hash.IndexOf(".", algo) - algo), NumberStyles.HexNumber);
            iterations.Should().Be(1000);
        }

        [Fact]
        public void GeneratedHashShoudContainSaltLength()
        {
            var hash = PBKDF2SaltedHash.GenerateHash("test");
            int algo = "PBKDF2".Length;
            int algoAndIterations = hash.IndexOf(".", algo) + 1;
            string saltLengthString = hash.Substring(algoAndIterations, hash.IndexOf(".", algoAndIterations) - algoAndIterations);
            int saltLength = int.Parse(saltLengthString, NumberStyles.HexNumber);
            saltLength.Should().BeGreaterThan(16);
        }

        [Fact]
        public void VerifiesGeneratedHash()
        {
            var hash = PBKDF2SaltedHash.GenerateHash("test");
            PBKDF2SaltedHash.VerifyHash("test", hash).Should().BeTrue();
            PBKDF2SaltedHash.VerifyHash("test1", hash).Should().BeFalse();

            var weak = SaltedHash.Generate("test");
            var strong = SaltedHash.Generate("test");
        }
    }
}
