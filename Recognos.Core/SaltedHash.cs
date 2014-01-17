namespace Recognos.Core
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Implementation for hashing a salted password and verifying the hash.
    /// </summary>
    public class SaltedHash
    {
        /// <summary>
        /// The hash algorithm implementation to use.
        /// </summary>
        private readonly HashAlgorithm hashAlgorithm;

        /// <summary>
        /// Length of the salt.
        /// </summary>
        private readonly int saltLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltedHash"/> class.
        /// </summary>
        /// <param name="hashAlgorithm">A <see cref="HashAlgorithm"/> HashAlgorihm which is derived from HashAlgorithm. C# provides
        /// the following classes: SHA1Managed,SHA256Managed, SHA384Managed, SHA512Managed and MD5CryptoServiceProvider</param>
        /// <param name="saltLength">Length of the salt.</param>
        public SaltedHash(HashAlgorithm hashAlgorithm, int saltLength)
        {
            Check.NotNull(hashAlgorithm, "hashAlgorithm");
            Check.Positive(saltLength, "saltLength");
            this.hashAlgorithm = hashAlgorithm;
            this.saltLength = saltLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltedHash"/> class.
        /// </summary>
        /// <remarks>
        /// Default constructor which initializes the SaltedHash with the SHA256Managed algorithm
        /// and a Salt of 4 bytes ( or 4*8 = 32 bits)
        /// </remarks>
        public SaltedHash()
            : this(new SHA256Managed(), 4)
        {
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hash.s</returns>
        public static string Generate(string password)
        {
            return new SaltedHash().GenerateHash(password);
        }

        /// <summary>
        /// Verifies the hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>True if the hash matches the hashed password.</returns>
        public static bool Verify(string password, string hash)
        {
            return new SaltedHash().VerifyHash(password, hash);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hash.s</returns>
        public string GenerateHash(string password)
        {
            Check.NotNull(password, "password");
            byte[] pass = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt();
            byte[] hash = ComputeHash(pass, salt);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies the hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>True if the hash matches the hashed password.</returns>
        public bool VerifyHash(string password, string hash)
        {
            Check.NotNull(password, "password");
            Check.NotEmpty(hash, "hash");

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashAndSaltBytes = Convert.FromBase64String(hash);

            Check.Condition(hashAndSaltBytes.Length > this.saltLength, "Hash length to small");

            byte[] salt = new byte[this.saltLength];
            Array.Copy(hashAndSaltBytes, salt, this.saltLength);

            byte[] hashBytes = ComputeHash(passwordBytes, salt);

            if (hashAndSaltBytes.Length != hashBytes.Length)
            {
                return false;
            }

            for (int i = 0; i < hashBytes.Length; ++i)
            {
                if (hashBytes[i] != hashAndSaltBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The actual hash calculation is shared by both GetHashAndSalt and the VerifyHash functions
        /// </summary>
        /// <param name="password">A byte array of the Data to Hash</param>
        /// <param name="salt">A byte array of the Salt to add to the Hash</param>
        /// <returns>A byte array with the calculated hash</returns>
        private byte[] ComputeHash(byte[] password, byte[] salt)
        {
            byte[] dataAndSalt = new byte[password.Length + saltLength];
            Array.Copy(password, dataAndSalt, password.Length);
            Array.Copy(salt, 0, dataAndSalt, password.Length, saltLength);

            byte[] hash = hashAlgorithm.ComputeHash(dataAndSalt);

            byte[] saltAndHash = new byte[saltLength + hash.Length];
            Array.Copy(salt, saltAndHash, salt.Length);
            Array.Copy(hash, 0, saltAndHash, salt.Length, hash.Length);

            return saltAndHash;
        }

        /// <summary>
        /// Generates the salt.
        /// </summary>
        /// <returns>Byte array containing the salt.</returns>
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[saltLength];

            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
