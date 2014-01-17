namespace Recognos.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for object type
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an instance of an object to an enumerable containing that object.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>Enumerable containing the object.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T instance)
        {
            yield return instance;
        }

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
