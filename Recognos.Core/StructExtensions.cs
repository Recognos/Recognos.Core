namespace Recognos.Core
{
    /// <summary>
    /// Extension methods for object type
    /// </summary>
    public static class StructExtensions
    {
        /// <summary>
        /// Determines whether the specified value is the default value for the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if the specified value is the default value for the type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefault<T>(this T value)
            where T : struct
        {
            return value.Equals(default(T));
        }
    }
}
