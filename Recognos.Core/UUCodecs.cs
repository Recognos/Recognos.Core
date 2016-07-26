namespace Recognos.Core
{
    using System;
    using System.Text;

    /// <summary>
    /// Utility class to UUEncode and UUDecode
    /// </summary>
    /// <remarks>
    /// This class has been taken from
    /// http://geekswithblogs.net/kobush/articles/63486.aspx
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    public static class UUCodecs
    {
        /// <summary>
        /// byte mapping used for encoding
        /// </summary>
        private static readonly byte[] UUEncMap = new byte[]
        {
          0x60, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
          0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
          0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
          0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
          0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47,
          0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
          0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57,
          0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F
        };

        /// <summary>
        /// byte mapping used for decoding
        /// </summary>
        private static readonly byte[] UUDecMap = new byte[]
        {
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
          0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
          0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
          0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
          0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
          0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
          0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
          0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        /// <summary>
        /// UUDecode a stream
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public static void UUDecode(System.IO.Stream input, System.IO.Stream output)
        {
            Check.NotNull(input, nameof(input));
            Check.NotNull(output, nameof(output));

            long len = input.Length;
            if (len == 0)
            {
                return;
            }

            long didx = 0;
            int nextByte = input.ReadByte();
            while (nextByte >= 0)
            {
                // get line length (in number of encoded octets)
                int lineLen = UUDecMap[nextByte];

                // ascii printable to 0-63 and 4-byte to 3-byte conversion
                long end = didx + lineLen;
                byte a, b, c;
                if (end > 2)
                {
                    while (didx < end - 2)
                    {
                        a = UUDecMap[input.ReadByte()];
                        b = UUDecMap[input.ReadByte()];
                        c = UUDecMap[input.ReadByte()];
                        var d = UUDecMap[input.ReadByte()];

                        output.WriteByte((byte)(((a << 2) & 255) | ((b >> 4) & 3)));
                        output.WriteByte((byte)(((b << 4) & 255) | ((c >> 2) & 15)));
                        output.WriteByte((byte)(((c << 6) & 255) | (d & 63)));
                        didx += 3;
                    }
                }

                if (didx < end)
                {
                    a = UUDecMap[input.ReadByte()];
                    b = UUDecMap[input.ReadByte()];
                    output.WriteByte((byte)(((a << 2) & 255) | ((b >> 4) & 3)));
                    didx++;
                }

                if (didx < end)
                {
                    b = UUDecMap[input.ReadByte()];
                    c = UUDecMap[input.ReadByte()];
                    output.WriteByte((byte)(((b << 4) & 255) | ((c >> 2) & 15)));
                    didx++;
                }

                // skip padding
                do
                {
                    nextByte = input.ReadByte();
                }
                while (nextByte >= 0 && nextByte != '\n' && nextByte != '\r');

                // skip end of line
                do
                {
                    nextByte = input.ReadByte();
                }
                while (nextByte >= 0 && (nextByte == '\n' || nextByte == '\r'));
            }
        }

        /// <summary>
        /// UUencode a stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public static void UUEncode(System.IO.Stream input, System.IO.Stream output)
        {
            Check.NotNull(input, nameof(input));
            Check.NotNull(output, nameof(output));

            long len = input.Length;
            if (len == 0)
            {
                return;
            }

            int sidx = 0;
            const int line_len = 45;
            byte[] nl = Encoding.ASCII.GetBytes(Environment.NewLine);

            byte a, b, c;

            // split into lines, adding line-length and line terminator
            while (sidx + line_len < len)
            {
                // line length
                output.WriteByte(UUEncMap[line_len]);

                // 3-byte to 4-byte conversion + 0-63 to ascii printable conversion
                for (int end = sidx + line_len; sidx < end; sidx += 3)
                {
                    a = (byte)input.ReadByte();
                    b = (byte)input.ReadByte();
                    c = (byte)input.ReadByte();

                    output.WriteByte(UUEncMap[(a >> 2) & 63]);
                    output.WriteByte(UUEncMap[(b >> 4) & 15 | (a << 4) & 63]);
                    output.WriteByte(UUEncMap[(c >> 6) & 3 | (b << 2) & 63]);
                    output.WriteByte(UUEncMap[c & 63]);
                }

                // line terminator
                for (int idx = 0; idx < nl.Length; idx++)
                {
                    output.WriteByte(nl[idx]);
                }
            }

            // line length
            output.WriteByte(UUEncMap[len - sidx]);

            // 3-byte to 4-byte conversion + 0-63 to ascii printable conversion
            while (sidx + 2 < len)
            {
                a = (byte)input.ReadByte();
                b = (byte)input.ReadByte();
                c = (byte)input.ReadByte();

                output.WriteByte(UUEncMap[(a >> 2) & 63]);
                output.WriteByte(UUEncMap[(b >> 4) & 15 | (a << 4) & 63]);
                output.WriteByte(UUEncMap[(c >> 6) & 3 | (b << 2) & 63]);
                output.WriteByte(UUEncMap[c & 63]);
                sidx += 3;
            }

            if (sidx < len - 1)
            {
                a = (byte)input.ReadByte();
                b = (byte)input.ReadByte();

                output.WriteByte(UUEncMap[(a >> 2) & 63]);
                output.WriteByte(UUEncMap[(b >> 4) & 15 | (a << 4) & 63]);
                output.WriteByte(UUEncMap[(b << 2) & 63]);
                output.WriteByte(UUEncMap[0]);
            }
            else if (sidx < len)
            {
                a = (byte)input.ReadByte();

                output.WriteByte(UUEncMap[(a >> 2) & 63]);
                output.WriteByte(UUEncMap[(a << 4) & 63]);
                output.WriteByte(UUEncMap[0]);
                output.WriteByte(UUEncMap[0]);
            }

            // line terminator
            for (int idx = 0; idx < nl.Length; idx++)
            {
                output.WriteByte(nl[idx]);
            }
        }
    }
}