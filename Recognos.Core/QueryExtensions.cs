namespace Recognos.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Helper extension methods for expressing linq queries.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="apply"/> parameter is true. 
        /// </summary>
        /// <typeparam name="T">Type of objects in the query</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="apply">if set to <c>true</c> applies the filter to the query.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IQueryable<T> ApplyIf<T>(this IQueryable<T> query, bool apply, Expression<Func<T, bool>> filter)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(filter, nameof(filter));

            return apply ? query.Where(filter) : query;
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="value"/> parameter is not null.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of the object to check if is null.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotNull<T, U>(this IQueryable<T> query, U value, Expression<Func<T, bool>> filter)
            where U : class
        {
            return query.ApplyIf(value != null, filter);
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="value"/> parameter is not empty.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IQueryable<T> ApplyIfNotEmpty<T>(this IQueryable<T> query, string value, Expression<Func<T, bool>> filter)
        {
            return query.ApplyIf(!string.IsNullOrWhiteSpace(value), filter);
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="values"/> parameter is not empty.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of objects in the arrat.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotEmpty<T, U>(this IQueryable<T> query, U[] values, Expression<Func<T, bool>> filter)
        {
            return query.ApplyIf(values != null && values.Any(), filter);
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="value"/> parameter has a value.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of value object.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The new query.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotEmpty<T, U>(this IQueryable<T> query, U value, Expression<Func<T, bool>> filter)
            where U : struct
        {
            return query.ApplyIf(!value.IsDefault(), filter);
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="value"/> parameter has a value.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of value object.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The new query.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotEmpty<T, U>(this IQueryable<T> query, U? value, Expression<Func<T, bool>> filter)
            where U : struct
        {
            return query.ApplyIf(value.HasValue, filter);
        }

        /// <summary>
        /// Applies the <paramref name="filter"/> to the <paramref name="query"/> if the <paramref name="values"/> parameter is not empty.
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of objects in the arrat.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotEmpty<T, U>(this IQueryable<T> query, IEnumerable<U> values, Expression<Func<T, bool>> filter)
        {
            return query.ApplyIf(values != null && values.Any(), filter);
        }

        /// <summary>
        /// Depending on the values on the <paramref name="values"/> array, applies no filer, <paramref name="filterSingleValue"/>
        /// or <paramref name="filterMultipleValues"/> to the <paramref name="query"/>
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <param name="filterSingleValue">The filter single value.</param>
        /// <param name="filterMultipleValues">The filter multi value.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IQueryable<T> ApplyIfNotEmpty<T>(
            this IQueryable<T> query,
            string[] values,
            Expression<Func<T, bool>> filterSingleValue,
            Expression<Func<T, bool>> filterMultipleValues)
        {
            if ( values == null || values.Length == 0)
            {
                return query;
            }

            if (values.Length == 1)
            {
                return query.ApplyIfNotEmpty(values[0], filterSingleValue);
            }

            return query.Where(filterMultipleValues);
        }

        /// <summary>
        /// Depending on the values on the <paramref name="values"/> array, applies no filer, <paramref name="filterConditionTrue"/>
        /// or <paramref name="filterConditionFalse"/> to the <paramref name="query"/>
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="U">Type of objects in the values</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <param name="condition">if set to <c>true</c> apply the filterConditionTrue.</param>
        /// <param name="filterConditionTrue">The filter to apply when the condition is true.</param>
        /// <param name="filterConditionFalse">The filter to apply when the condition" is false.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "U is also accepted for short types")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U", Justification = "U is also accepted for short types")]
        public static IQueryable<T> ApplyIfNotEmptyWithCondition<T, U>(
            this IQueryable<T> query,
            U[] values,
            bool condition,
            Expression<Func<T, bool>> filterConditionTrue,
            Expression<Func<T, bool>> filterConditionFalse)
        {
            if (values == null || values.Length == 0)
            {
                return query;
            }

            if (condition)
            {
                return query.Where(filterConditionTrue);
            }

            return query.Where(filterConditionFalse);
        }

        /// <summary>
        /// Depending on the <paramref name="value"/> parameter, applies no filer, <paramref name="filterConditionTrue"/>
        /// or <paramref name="filterConditionFalse"/> to the <paramref name="query"/>
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        /// <param name="condition">if set to <c>true</c> applies filterConditionTrue.</param>
        /// <param name="filterConditionTrue">The filter condition true.</param>
        /// <param name="filterConditionFalse">The filter condition false.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IQueryable<T> ApplyIfNotEmptyWithCondition<T>(
            this IQueryable<T> query,
            string value,
            bool condition,
            Expression<Func<T, bool>> filterConditionTrue,
            Expression<Func<T, bool>> filterConditionFalse)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return query;
            }

            if (condition)
            {
                return query.Where(filterConditionTrue);
            }

            return query.Where(filterConditionFalse);
        }

        /// <summary>
        /// Returns a new query that has an order by clause applied
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="descending">if set to <c>true</c> order descending.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(keySelector, nameof(keySelector));

            if (descending)
            {
                return query.OrderByDescending(keySelector);
            }

            return query.OrderBy(keySelector);
        }

        /// <summary>
        /// Returns a new query that has an order by clause applied
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, ListSortDirection direction)
        {
            Check.NotNull(query, nameof(query));
            Check.NotNull(keySelector, nameof(keySelector));

            return query.OrderBy(keySelector, direction == ListSortDirection.Descending);
        }

        /// <summary>
        /// Returns a new query that has an order by clause applied
        /// </summary>
        /// <typeparam name="T">Type of objects in the query.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="sortDescriptor">The sort descriptor.</param>
        /// <returns>The new query.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The interface is clear")]
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, ICollection<Tuple<Expression<Func<T, TKey>>, ListSortDirection>> sortDescriptor)
        {
            Check.NotNull(query, nameof(query));
            Check.NotEmpty(sortDescriptor, nameof(sortDescriptor));

            var first = sortDescriptor.First();

            IOrderedQueryable<T> result = query.OrderBy(first.Item1, first.Item2);

            foreach (var sort in sortDescriptor.Skip(1))
            {
                result = result.OrderBy(sort.Item1, sort.Item2);
            }

            return result;
        }
    }
}
