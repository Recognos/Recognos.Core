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
            Check.NotEmpty(format, "format");

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}
