namespace Recognos.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// various conversion extensions
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        public static byte[] Compress(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return data;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The data to decompress.</param>
        /// <returns>The decompressed data.</returns>
        public static byte[] Decompress(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return data;
            }

            using (MemoryStream ms = new MemoryStream(data, false))
            {
                byte[] buf = new byte[1024];
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        bool done = false;
                        while (!done)
                        {
                            int read = zip.Read(buf, 0, buf.Length);
                            output.Write(buf, 0, read);
                            done = read < buf.Length;
                        }

                        return output.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// convert data into hexa decimal representation
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>The hexadecimal representation of the data as a string</returns>
        public static string ToHexa(this byte[] data)
        {
            Check.NotNull(data, "data");
            return data.Aggregate(
                new StringBuilder(32),
                (sb, bit) => sb.Append(bit.ToString("X2", CultureInfo.InvariantCulture))).ToString();
        }

        /// <summary>
        /// Convert from the integer value to an enumeration value
        /// </summary>
        /// <typeparam name="T">Type of the enumeration.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The enumeration value.</returns>
        [DebuggerStepThrough]
        public static T ToEnum<T>(this int value)
        {
            Check.Positive(value, "value");
            Type enumType = typeof(T);
            Check.Condition(enumType.IsEnum, "The type must be an enumeration");
            return (T)Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Converts the value of <paramref name="size"/> to human readable file size
        /// </summary>
        /// <param name="size">The size to convert.</param>
        /// <returns>Human readable file size.</returns>
        public static string ToFileSize(this int size)
        {
            return ToFileSize(Convert.ToInt64(size));
        }

        /// <summary>
        /// Converts the value of <paramref name="size"/> to human readable file size
        /// </summary>
        /// <param name="size">The size to convert.</param>
        /// <returns>Human readable file size.</returns>
        public static string ToFileSize(this long size)
        {
            Check.Positive(size, "size");

            const int ByteConversion = 1024;
            double bytes = Convert.ToDouble(size);

            // GB Range
            if (bytes >= Math.Pow(ByteConversion, 3))
            {
                return Format.Invariant("{0} GB", Math.Round(bytes / Math.Pow(ByteConversion, 3), 2));
            }

            // MB Range
            if (bytes >= Math.Pow(ByteConversion, 2))
            {
                return Format.Invariant("{0} MB", Math.Round(bytes / Math.Pow(ByteConversion, 2), 2));
            }

            // KB Range
            if (bytes >= ByteConversion)
            {
                return Format.Invariant("{0} KB", Math.Round(bytes / ByteConversion, 2));
            }

            return Format.Invariant("{0} Bytes", bytes);
        }
    }
}
