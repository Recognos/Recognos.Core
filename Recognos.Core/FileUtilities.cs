namespace Recognos.Core
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Utilities for handling files
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Compute the SHA1 hash of a stream.
        /// </summary>
        /// <param name="stream">Stream for witch to compute the hash.</param>
        /// <returns>The hexadecimal hash.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA", Justification = "Name is an acronym")]
        public static string ComputeSHA1Hash(this Stream stream)
        {
            Check.NotNull(stream, "stream");
            return SHA1.Create().ComputeHash(stream).ToHexa();
        }

        /// <summary>
        /// Computes the SHA1 hash of a file.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <returns>The hexadecimal hash.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA", Justification = "Name is an acronym")]
        public static string ComputeSHA1Hash(string pathToFile)
        {
            Check.NotEmpty(pathToFile, "pathToFile");

            using (Stream stream = File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return stream.ComputeSHA1Hash();
            }
        }

        /// <summary>
        /// Saves a stream to a file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="pathToFile">The path to file.</param>
        public static void SaveToFile(this Stream stream, string pathToFile)
        {
            Check.NotNull(stream, "stream");
            Check.NotEmpty(pathToFile, "pathToFile");
            using (Stream output = File.Open(pathToFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(output);
            }
        }

        /// <summary>
        /// Saves a stream to another stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        public static void SaveToStream(this Stream stream, Stream outputStream)
        {
            Check.NotNull(stream, "stream");
            Check.NotNull(outputStream, "outputStream");
            stream.CopyTo(outputStream);
        }

        /// <summary>
        /// Reads the content from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The content as a string.</returns>
        public static string ReadContent(this Stream stream)
        {
            Check.NotNull(stream, "stream");

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the content from the stream as binary.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>byte array with the stream content.</returns>
        public static byte[] ReadBinaryContent(this Stream stream)
        {
            Check.NotNull(stream, "stream");

            MemoryStream memStream = stream as MemoryStream;
            if (memStream != null)
            {
                return memStream.ToArray();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Reads the content from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The content as a string.</returns>
        public static string ReadContent(this Stream stream, Encoding encoding)
        {
            Check.NotNull(stream, "stream");
            Check.NotNull(encoding, "encoding");

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
