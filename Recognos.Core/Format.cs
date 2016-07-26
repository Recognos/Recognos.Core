namespace Recognos.Core
{
    using System.Globalization;

    /// <summary>
    /// Helper class to format strings with InvariantCulture
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Format a string with invariant culture.
        /// </summary>
        /// <param name="format">The format of the string.</param>
        /// <param name="args">The args to format with.</param>
        /// <returns>The formated string.</returns>
        public static string Invariant(string format, params object[] args)
        {
            Check.NotEmpty(format, nameof(format));

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Format a string with invariant culture.
        /// </summary>
        /// <param name="format">The format of the string.</param>
        /// <param name="arg0">The argument to format with.</param>
        /// <returns>The formated string.</returns>
        public static string Invariant(string format, object arg0)
        {
            Check.NotEmpty(format, nameof(format));

            return string.Format(CultureInfo.InvariantCulture, format, arg0);
        }

        /// <summary>
        /// Format a string with invariant culture.
        /// </summary>
        /// <param name="format">The format of the string.</param>
        /// <param name="arg0">The argument to format with.</param>
        /// <param name="arg1">The argument to format with.</param>
        /// <returns>The formated string.</returns>
        public static string Invariant(string format, object arg0, object arg1)
        {
            Check.NotEmpty(format, nameof(format));

            return string.Format(CultureInfo.InvariantCulture, format, arg0, arg1);
        }

        /// <summary>
        /// Format a string with invariant culture.
        /// </summary>
        /// <param name="format">The format of the string.</param>
        /// <param name="arg0">The argument to format with.</param>
        /// <param name="arg1">The argument to format with.</param>
        /// <param name="arg2">The argument to format with.</param>
        /// <returns>The formated string.</returns>
        public static string Invariant(string format, object arg0, object arg1, object arg2)
        {
            Check.NotEmpty(format, nameof(format));

            return string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2);
        }
    }
}
