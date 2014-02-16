using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Recognos.Core
{
    /// <summary>
    /// Secure password hash
    /// 
    /// Uses PBKDF2 internally, as implemented by the Rfc2998DeriveBytes class.
    /// 
    /// See http://en.wikipedia.org/wiki/PBKDF2
    /// and http://msdn.microsoft.com/en-us/library/bwx8t0yt.aspx
    /// Code adappted from stackoverflow: http://code.google.com/p/stackid/source/browse/OpenIdProvider/Current.cs#1226 
    /// </summary>
    public sealed class PBKDF2SaltedHash
    {
        private const int DefaultSaltSize = 16;
        private const int DefaultIterations = 1000;
        private const int DefaultKeySize = 24;
        private const string HashPrefix = "PBKDF2";

        /// <summary>
        /// Returns true if the hash starts with the PBKDF2 prefix
        /// </summary>
        /// <param name="hash">hash to check for PBKDF2 prefix</param>
        /// <returns>true if hash starts with PBKDF2</returns>
        public static bool IsPBKDF2Hash(string hash)
        {
            return hash.StartsWith(HashPrefix);
        }

        /// <summary>
        /// Generate a salted hash for a password.
        /// The returned string contains is formated like this:
        /// PBKDF2{nuber of iterations in hexa}.{salt length in hexa}.{salt}{key size in hexa}.{key}
        /// </summary>
        /// <param name="password">Passord to hash</param>
        /// <returns>Formated password hash</returns>
        public static string GenerateHash(string password)
        {
            Check.NotEmpty(password, "password");

            byte[] pass = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt(DefaultSaltSize);
            byte[] hash = ComputeHash(pass, salt, DefaultKeySize);

            var stringSalt = Convert.ToBase64String(salt);
            var stringKey = Convert.ToBase64String(hash);

            return Format.Invariant("{0}{1:X}.{2:X}.{3}{4}", HashPrefix, DefaultIterations, stringSalt.Length, stringSalt, stringKey);
        }

        /// <summary>
        /// Verifies the hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>True if the hash matches the hashed password.</returns>
        public static bool VerifyHash(string password, string hash)
        {
            Check.NotEmpty(password, "password");
            Check.NotEmpty(hash, "hash");

            if (hash.Substring(0, HashPrefix.Length) != HashPrefix)
            {
                return false;
            }

            int iterationsStart = HashPrefix.Length;
            string iterationsString = hash.Substring(iterationsStart, hash.IndexOf(".", iterationsStart) - iterationsStart);
            int iteations = int.Parse(iterationsString, NumberStyles.HexNumber);

            int saltSizeStart = iterationsStart + iterationsString.Length + 1;
            string saltSizeString = hash.Substring(saltSizeStart, hash.IndexOf(".", saltSizeStart) - saltSizeStart);
            int saltSize = int.Parse(saltSizeString, NumberStyles.HexNumber);

            int saltStart = saltSizeStart + saltSizeString.Length + 1;
            string salt = hash.Substring(saltStart, saltSize);
            byte[] saltBytes = Convert.FromBase64String(salt);

            string key = hash.Substring(saltStart + saltSize);
            byte[] keyBytes = Convert.FromBase64String(key);
            int keySize = keyBytes.Length;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordHash = ComputeHash(passwordBytes, saltBytes, keySize);

            if (keySize != passwordHash.Length)
            {
                return false;
            }

            for (int i = 0; i < keySize; i++)
            {
                if (keyBytes[i] != passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compute the actual hash for the <paramref name="password"/> and <paramref name="salt"/> 
        /// using <paramref name="keySize"/> as the key size in bytes.
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">Salt to use in the hasing process</param>
        /// <param name="keySize">Size of the key to compute</param>
        /// <returns></returns>
        private static byte[] ComputeHash(byte[] password, byte[] salt, int keySize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, DefaultIterations))
            {
                return pbkdf2.GetBytes(keySize);
            }
        }

        /// <summary>
        /// Generates the salt.
        /// </summary>
        /// <returns>Byte array containing the salt.</returns>
        private static byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            return salt;
        }
    }
}
