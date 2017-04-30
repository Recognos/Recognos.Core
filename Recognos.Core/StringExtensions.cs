namespace Recognos.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Various extension methods for string operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Regex pattern for matching tags.
        /// </summary>
        /// <remarks>
        /// We consider a tag to have a max 1000 chars inside opening and closing chars to avoid <see cref="OutOfMemoryException"/> Exceptions. 
        /// This limit accounts for tag attributes which on generated documents can be quite large.
        /// </remarks>
        private const string TagsPattern = @"<(.|\n){0,1000}?>";

        /// <summary>
        /// The regex pattern for a guid.
        /// </summary>
        private const string GuidPattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

        /// <summary>
        /// Regular expression to match a guid.
        /// </summary>
        private static readonly Regex guidExpression =
            new Regex(GuidPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

        /// <summary>
        /// Regular expression to match and remove tags.
        /// </summary>
        private static readonly Regex tagsExpression =
            new Regex(TagsPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

        /// <summary>
        /// Characters that represent word boundaries.
        /// </summary>
        private static readonly char[] wordSeparators = new[] { ' ', '\n' };

        /// <summary>
        /// Determines whether this string and a specified System.String object have the same value ignoring their casing.
        /// </summary>
        /// <remarks>
        /// if source or other is null returns false
        /// </remarks>
        /// <param name="source">The source string.</param>
        /// <param name="other">The other string.</param>
        /// <returns>true if the value of the value parameter is the same as this string; otherwise, false</returns>
        [DebuggerStepThrough]
        public static bool CaseInsensitiveEquals(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return string.Equals(source, other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether this string and a specified System.String 
        /// object have the same value taking their casing into account.
        /// </summary>
        /// <remarks>
        /// if source or other is null returns false
        /// </remarks>
        /// <param name="source">The source string.</param>
        /// <param name="other">The other string.</param>
        /// <returns>true if the value of the value parameter is the same as this string; otherwise, false</returns>
        [DebuggerStepThrough]
        public static bool CaseSensitiveEquals(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return string.Equals(source, other, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the source starts with other in a case sensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source starts with other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseSensitiveStartsWith(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.StartsWith(other, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the source ends with other in a case sensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source ends with other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseSensitiveEndsWith(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.EndsWith(other, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the source starts with other in a case insensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source starts with other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseInsensitiveStartsWith(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.StartsWith(other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the source ends with other in a case insensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source ends with other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseInsensitiveEndsWith(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.EndsWith(other, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the source contains other in a case sensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source contains other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseSensitiveContains(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.IndexOf(other, StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// Determines whether the source contains other in a case sensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source contains other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseSensitiveContains(this IEnumerable<string> source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.Any(s => s.CaseSensitiveEquals(other));
        }

        /// <summary>
        /// Determines whether the source contains other in a case insensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source contains other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseInsensitiveContains(this string source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.IndexOf(other, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Determines whether the source contains other in a case insensitive way.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns>true is the source contains other, otherwise false.</returns>
        [DebuggerStepThrough]
        public static bool CaseInsensitiveContains(this IEnumerable<string> source, string other)
        {
            if (source == null || other == null)
            {
                return false;
            }

            return source.Any(s => s.CaseInsensitiveEquals(other));
        }

        /// <summary>
        /// Strips the tags from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>A string without tags.</returns>
        [DebuggerStepThrough]
        public static string StripTags(this string source)
        {
            return source.StripTags(false);
        }

        /// <summary>
        /// Strips the tags from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="replaceWithSpaces">if set to <c>true</c> the tags shold be replaced with spaces.</param>
        /// <returns>A string without tags.</returns>
        public static string StripTags(this string source, bool replaceWithSpaces)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            if (!replaceWithSpaces)
            {
                return tagsExpression.Replace(source, string.Empty);
            }

            return tagsExpression.Matches(source).Cast<Match>()
                .Select(m => m.Value).Distinct()
                .Aggregate(new StringBuilder(source), (b, s) => b.Replace(s, new string(' ', s.Length)))
                .ToString();
        }

        /// <summary>
        /// Strips all HTML tags from a string and replaces the tags with the specified replacement
        /// </summary>
        /// <param name="html">The HTML string.</param>
        /// <returns>The resulting string.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HTML", Justification = "Name is an acronym")]
        public static string StripHTML(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }

            string stripped = tagsExpression.Replace(html, string.Empty).Trim();

            return WebUtility.HtmlDecode(stripped);
        }

        /// <summary>
        /// Joins the specified collection into a string.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined string</returns>
        [DebuggerStepThrough]
        public static string Join(this IEnumerable<string> collection, string separator)
        {
            if (collection == null)
            {
                return string.Empty;
            }
            return string.Join(separator, collection);
        }

        /// <summary>
        /// Joins the specified collection into a string.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="emptyValue">String to return if the collection is empty.</param>
        /// <returns>
        /// The joined string
        /// </returns>
        [DebuggerStepThrough]
        public static string Join(this IEnumerable<string> collection, string separator, string emptyValue)
        {
            if (collection == null || !collection.Any())
            {
                return emptyValue;
            }
            return string.Join(separator, collection);
        }

        /// <summary>
        /// Convert from the string value to an enumeration value
        /// </summary>
        /// <typeparam name="T">Type of the enumeration.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The enumeration value.</returns>
        public static T ToEnum<T>(this string value) where T : struct
        {
            Check.NotEmpty(value, nameof(value));
            Check.Condition(typeof(T).IsEnum, Format.Invariant("The type {0} must be an enum.", typeof(T).Name));

            T result;

            if (Enum.TryParse(value, out result))
            {
                return result;
            }

            throw new ArgumentException(Format.Invariant("value is not a value of the {0} enum", typeof(T).Name));
        }

        /// <summary>
        /// Computes the SHA1 hash.
        /// </summary>
        /// <param name="text">The text for witch to compute the hash.</param>
        /// <returns>The SHA1 hash.</returns>
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SHA", Justification = "Name is an acronym")]
        public static string ComputeSHA1Hash(this string text)
        {
            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return stream.ComputeSHA1Hash();
            }
        }

        /// <summary>
        /// Determines whether the specified expression is GUID.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// <c>true</c> if the specified expression is GUID; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsGuid(this string expression)
        {
            return expression != null && guidExpression.IsMatch(expression);
        }

        /// <summary>
        /// compress input string using GZip
        /// </summary>
        /// <param name="text">Text to compress</param>
        /// <returns>The compressed string</returns>
        public static string GzipCompress(this string text)
        {
            Check.NotNull(text, nameof(text));

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                ms.Position = 0;

                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);

                byte[] gzippedBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzippedBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzippedBuffer, 0, 4);
                return Convert.ToBase64String(gzippedBuffer);
            }
        }

        /// <summary>
        /// compress input string using GZip
        /// </summary>
        /// <param name="text">Text to compress</param>
        /// <returns>The compressed string</returns>
        public static async Task<string> GzipCompressAsync(this string text)
        {
            Check.NotNull(text, nameof(text));

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    await zip.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                }

                ms.Position = 0;

                byte[] compressed = new byte[ms.Length];
                await ms.ReadAsync(compressed, 0, compressed.Length).ConfigureAwait(false);

                byte[] gzippedBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzippedBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzippedBuffer, 0, 4);
                return Convert.ToBase64String(gzippedBuffer);
            }
        }

        /// <summary>
        /// Decompress a GZip compressed string
        /// </summary>
        /// <param name="compressedText">Compressed text to decompress</param>
        /// <returns>The decompressed string</returns>
        public static string GzipDecompress(this string compressedText)
        {
            Check.NotNull(compressedText, nameof(compressedText));

            byte[] gzippedBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzippedBuffer, 0);
                ms.Write(gzippedBuffer, 4, gzippedBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// Decompress a GZip compressed string
        /// </summary>
        /// <param name="compressedText">Compressed text to decompress</param>
        /// <returns>The decompressed string</returns>
        public static async Task<string> GzipDecompressAsync(this string compressedText)
        {
            Check.NotNull(compressedText, nameof(compressedText));

            byte[] gzippedBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzippedBuffer, 0);
                ms.Write(gzippedBuffer, 4, gzippedBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    await zip.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// Returns the left part of a string. 
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="size">The size of the string to return.</param>
        /// <returns>String containing the left part of the original string</returns>
        public static string Left(this string target, int size)
        {
            Check.Positive(size, nameof(size));
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            if (target.Length < size)
            {
                size = target.Length;
            }

            return target.Substring(0, size);
        }

        /// <summary>
        /// Returns the right part of a string. 
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="size">The size of the string to return.</param>
        /// <returns>String containing the right part of the original string</returns>
        public static string Right(this string target, int size)
        {
            Check.Positive(size, nameof(size));
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            if (target.Length < size)
            {
                size = target.Length;
            }

            return target.Substring(target.Length - size);
        }

        /// <summary>
        /// Returns the right part of a string of a max <paramref name="length"/> truncating at word boundary.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="length">The max length of the result.</param>
        /// <param name="prefix">The prefix to prepped to the result if the string has been truncated.</param>
        /// <returns>The truncated string.</returns>
        public static string RightAtWord(this string input, int length, string prefix)
        {
            Check.Positive(length, nameof(length));

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string result = input.Right(length);
            if (length < input.Length && !string.IsNullOrEmpty(prefix))
            {
                return string.Concat(prefix, result);
            }

            return result;
        }

        /// <summary>
        /// Returns the right part of a string of a max <paramref name="length"/> truncating at word boundary.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="length">The max length of the result.</param>
        /// <returns>The truncated string.</returns>
        public static string RightAtWord(this string input, int length)
        {
            Check.Positive(length, nameof(length));

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (length >= input.Length)
            {
                return input;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            int index = input.Length;
            int lastIndex = index;
            int size = 0;
            while (size <= length && size < (input.Length - 1))
            {
                lastIndex = index;
                index = input.LastIndexOfAny(wordSeparators, index - 1);

                if (index <= 0)
                {
                    index = 0;
                }

                size = input.Length - index - 1;
            }

            if (lastIndex >= input.Length)
            {
                return string.Empty;
            }

            return input.Substring(lastIndex + 1);
        }

        /// <summary>
        /// Returns the left part of a string of a max <paramref name="length"/> truncating at word boundary.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="length">The max length of the result.</param>
        /// <param name="suffix">The suffix to append to the result if the string has been truncated.</param>
        /// <returns>The truncated string.</returns>
        public static string LeftAtWord(this string input, int length, string suffix)
        {
            Check.Positive(length, nameof(length));

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string result = input.LeftAtWord(length);
            if (length < input.Length && !string.IsNullOrEmpty(suffix))
            {
                return string.Concat(result, suffix);
            }

            return result;
        }

        /// <summary>
        /// Returns the left part of a string of a max <paramref name="length"/> truncating at word boundary.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="length">The max length of the result.</param>
        /// <returns>The truncated string.</returns>
        public static string LeftAtWord(this string input, int length)
        {
            Check.Positive(length, nameof(length));

            if (length >= input.Length)
            {
                return input;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            int index = 0;
            int lastindex = 0;
            while (index <= length && index <= input.Length)
            {
                lastindex = index;
                index = input.IndexOfAny(wordSeparators, index + 1);

                if (index <= 0)
                {
                    index = input.Length;
                }
            }

            return input.Substring(0, lastindex);
        }

        /// <summary>
        /// Returns the last <paramref name="wordCount"/> words of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="wordCount">The the number of words to return.</param>
        /// <returns>String containing the words.</returns>
        public static string LastWords(this string input, int wordCount)
        {
            Check.Positive(wordCount, nameof(wordCount));
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (wordCount == 0)
            {
                return string.Empty;
            }

            int count = 0;
            int index = input.Length - 1;
            while (count < wordCount)
            {
                index = input.LastIndexOfAny(wordSeparators, index - 1);
                if (index <= 0)
                {
                    index = 0;
                    break;
                }

                count++;
            }

            int start = index > 0 ? index + 1 : 0;

            return input.Substring(start);
        }

        /// <summary>
        /// Returns the first <paramref name="wordCount"/> words of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="wordCount">The the number of words to return.</param>
        /// <returns>String containing the words.</returns>
        public static string FirstWords(this string input, int wordCount)
        {
            Check.Positive(wordCount, nameof(wordCount));
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (wordCount == 0)
            {
                return string.Empty;
            }

            int count = 0;
            int index = 0;
            while (count < wordCount)
            {
                index = input.IndexOfAny(wordSeparators, index + 1);
                if (index <= 0)
                {
                    index = 0;
                    break;
                }

                count++;
            }

            int length = index > 0 ? index : input.Length;

            return input.Substring(0, length);
        }

        /// <summary>
        /// Returns the previous <paramref name="numberOfLines"/> lines of a string starting from <paramref name="position"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="position">The position to begin from.</param>
        /// <param name="numberOfLines">The the number of lines to return.</param>
        /// <returns>String containing the previous lines.</returns>
        public static string PreviousLines(this string input, int position, int numberOfLines)
        {
            Check.Positive(position, nameof(position));
            Check.Positive(numberOfLines, nameof(numberOfLines));
            Check.Condition(position == 0 || position < input.Length, "Position must be less than the string length");

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            int line = input.LastIndexOf('\n', position);

            for (int i = 0; i < numberOfLines && line > 0; i++)
            {
                line = input.LastIndexOf('\n', line - 1);
                if (line <= 0)
                {
                    line = 0;
                    break;
                }
            }

            int start = line == 0 ? line : line + 1;
            int size = position - (line < 0 ? 0 : line);

            if (line == 0)
            {
                size++;
            }

            return input.Substring(start, size);
        }

        /// <summary>
        /// Returns the next <paramref name="numberOfLines"/> lines of a string starting from <paramref name="position"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="position">The position to begin from.</param>
        /// <param name="numberOfLines">The the number of lines to return.</param>
        /// <returns>
        /// String containing the next lines.
        /// </returns>
        public static string NextLines(this string input, int position, int numberOfLines)
        {
            Check.Positive(position, nameof(position));
            Check.Positive(numberOfLines, nameof(numberOfLines));
            Check.Condition(position == 0 || position < input.Length, "Position must be less than the string length");

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            int line = input.IndexOf('\n', position);

            // no more lines
            if (line == -1)
            {
                return input.Substring(position);
            }

            for (int i = 0; i < numberOfLines; i++)
            {
                line = input.IndexOf('\n', line + 1);
                if (line + 1 == input.Length || line < 0)
                {
                    line = 0;
                    break;
                }
            }

            int start = position;
            int size = line == 0 ? input.Length - position : line - position;

            return input.Substring(start, size);
        }

        /// <summary>
        /// Returns the first <paramref name="numberOfLines"/> lines of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="numberOfLines">The the number of lines to return.</param>
        /// <returns>
        /// String containing the first lines.
        /// </returns>
        public static string FirstLines(this string input, int numberOfLines)
        {
            Check.Positive(numberOfLines, nameof(numberOfLines));

            if (numberOfLines == 0)
            {
                return string.Empty;
            }

            return input.NextLines(0, numberOfLines - 1);
        }

        /// <summary>
        /// Returns the last <paramref name="numberOfLines"/> lines of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="numberOfLines">The the number of lines to return.</param>
        /// <returns>
        /// String containing the last lines.
        /// </returns>
        public static string LastLines(this string input, int numberOfLines)
        {
            Check.Positive(numberOfLines, nameof(numberOfLines));

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (numberOfLines == 0)
            {
                return string.Empty;
            }

            return input.PreviousLines(input.Length - 1, numberOfLines - 1);
        }

        /// <summary>
        /// Safe implementation for substring. Is start and length are out of boundaries they are adjusted.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <returns>The selected substring.</returns>
        public static string SafeSubstring(this string input, int start, int length)
        {
            Check.Positive(start, nameof(start));
            Check.Positive(length, nameof(length));

            if (string.IsNullOrEmpty(input) || length == 0)
            {
                return string.Empty;
            }

            if (start >= input.Length)
            {
                return string.Empty;
            }

            int final = length;
            if (start + length > input.Length)
            {
                final--;
            }

            return input.Substring(start, final);
        }

        /// <summary>
        /// Highlights the specified indexes in the input string using the specified format.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns>The highlighted text.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static string Highlight(this string input, string format, IEnumerable<Tuple<int, int>> indexes)
        {
            Check.NotEmpty(format, nameof(format));
            Check.NotNull(indexes, nameof(indexes));

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // normalize
            List<Tuple<int, int>> indexList = indexes
                .Where(idx => idx.Item1 < idx.Item2 && idx.Item1 < input.Length)
                .Select(idx => idx.Item2 <= input.Length ? idx : Tuple.Create(idx.Item1, input.Length))
                .OrderBy(idx => idx.Item1)
                .ToList();

            int i = 1;

            while (i < indexList.Count)
            {
                Tuple<int, int> prev = indexList[i - 1];
                Tuple<int, int> current = indexList[i];
                if (current.Item1 <= prev.Item2)
                {
                    if (current.Item2 > prev.Item2)
                    {
                        prev = Tuple.Create(prev.Item1, current.Item2);
                        indexList[i - 1] = prev;
                    }

                    indexList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            StringBuilder sb = new StringBuilder();

            int end = 0;

            foreach (Tuple<int, int> idx in indexList)
            {
                sb.Append(input.SafeSubstring(end, idx.Item1 - end));
                sb.Append(Format.Invariant(format, input.SafeSubstring(idx.Item1, idx.Item2 - idx.Item1 + 1)));
                end = idx.Item2 + 1;
            }

            sb.Append(input.SafeSubstring(end, input.Length - end + 1));

            return sb.ToString();
        }

        /// <summary>
        /// Highlights the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="format">The format.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>The highlighted string</returns>
        public static string Highlight(this string input, string format, IEnumerable<string> tags)
        {
            return input.Highlight(format, input.AllIndexesOf(tags).Select(p => Tuple.Create(p.Item1, p.Item1 + p.Item2.Length - 1)));
        }

        /// <summary>
        /// Returns all the indexes where the tag is found in the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="tag">The tag to search.</param>
        /// <returns>A collection of indexes.</returns>
        public static IEnumerable<int> AllIndexesOf(this string input, string tag)
        {
            foreach (Match match in Regex.Matches(input, tag, RegexOptions.IgnoreCase))
            {
                yield return match.Index;
            }
        }

        /// <summary>
        /// Returns all the indexes where the tags are found in the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="tags">The tags to search.</param>
        /// <returns>A collection of pairs of indexes and tags.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IEnumerable<Tuple<int, string>> AllIndexesOf(this string input, IEnumerable<string> tags)
        {
            foreach (Match match in Regex.Matches(input, tags.Join("|"), RegexOptions.IgnoreCase))
            {
                yield return Tuple.Create(match.Index, match.Value);
            }
        }

        /// <summary>
        /// Splits the input into words.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Enumerable of words in the string.</returns>
        public static IEnumerable<string> Words(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                yield break;
            }

            foreach (Match match in Regex.Matches(input, @"\S+"))
            {
                yield return match.Value;
            }
        }

        /// <summary>
        /// Computes the Levenshtein distance of two strings
        /// </summary>
        /// <param name="first">First string.</param>
        /// <param name="second">The second string.</param>
        /// <returns>The Levenshtein distance of the two strings</returns>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body", Justification = "The array does not waste space")]
        public static int Levenshtein(this string first, string second)
        {
            if (string.IsNullOrEmpty(first))
            {
                if (!string.IsNullOrEmpty(second))
                {
                    return second.Length;
                }

                return 0;
            }

            if (string.IsNullOrEmpty(second))
            {
                if (!string.IsNullOrEmpty(first))
                {
                    return first.Length;
                }

                return 0;
            }

            var upperBound0 = first.Length;
            var upperBound1 = second.Length;

            int[,] d = new int[first.Length + 1, second.Length + 1];

            for (int i = 0; i <= upperBound0; i += 1)
            {
                d[i, 0] = i;
            }

            for (int i = 0; i <= upperBound1; i += 1)
            {
                d[0, i] = i;
            }

            for (int i = 1; i <= upperBound0; i += 1)
            {
                for (int j = 1; j <= upperBound1; j += 1)
                {
                    var cost = Convert.ToInt32(first[i - 1] != second[j - 1]);

                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[upperBound0, upperBound1];
        }
    }
}
