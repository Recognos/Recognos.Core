using FluentAssertions;using Xunit;
using Recognos.Core;

namespace Recognos.Test.Core
{
    
    public class SaltedHashTests
    {
        private string password = "test";
        private string good = "test";
        private string bad = "bad";

        [Fact]
        public void SaltedHash_CanCreateHash()
        {
            SaltedHash hash = new SaltedHash();
            string hashed = hash.GenerateHash(password);
            hashed.Should().NotBeEmpty();
        }

        [Fact]
        public void SaltedHash_CanVerifyCreatedHash()
        {
            SaltedHash hash = new SaltedHash();
            string hashed = hash.GenerateHash(password);

            bool result = hash.VerifyHash(password, hashed);

            result.Should().BeTrue();
        }

        [Fact]
        public void SaltedHash_CanVerifyBadPass()
        {
            SaltedHash hash = new SaltedHash();
            string hashed = hash.GenerateHash(good);

            bool result = hash.VerifyHash(bad, hashed);

            result.Should().BeFalse();
        }
    }
}
