namespace Recognos.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Helper extension methods for expressing functional algorithms
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Enumerates over a collection in batches of a specified <paramref name="size"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection</typeparam>
        /// <param name="source">The collection on which to enumerate</param>
        /// <param name="size">Size of the desired batches</param>
        /// <returns>Enumerable collection of enumerable batches</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Interface makes sense for batching")]
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            Check.NotNull(source, nameof(source));
            Check.AbsolutePositive(size, nameof(size));

            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
