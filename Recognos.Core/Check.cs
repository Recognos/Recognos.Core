namespace Recognos.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class to check methods arguments
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// regular expression for email validation
        /// </summary>
        private static readonly Regex emailRegex =
            new Regex(
                @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// verify that argument is not null
        /// </summary>
        /// <param name="argument">argument to check</param>
        /// <param name="name">Original name of the argument</param>
        /// <typeparam name="T">Type of the argument</typeparam>
        [DebuggerStepThrough]
        public static void NotNull<T>(T argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// verify that argument is not null or empty
        /// </summary>
        /// <param name="argument">argument to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void NotEmpty(string argument, string name)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException("Argument can't be empty", name);
            }
        }

        /// <summary>
        /// Verify that argument is not empty.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="name">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void NotEmpty(Guid argument, string name)
        {
            if (argument == default(Guid))
            {
                throw new ArgumentException("Argument can't be empty", name);
            }
        }

        /// <summary>
        /// Verify that argument is not empty.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="name">The name of the argument.</param>
        [DebuggerStepThrough]
        public static void NotEmpty(DateTime argument, string name)
        {
            if (argument == default(DateTime))
            {
                throw new ArgumentException("Argument can't be empty", name);
            }
        }

        /// <summary>
        /// Check a enumerable to not be empty
        /// </summary>
        /// <typeparam name="T">Type of elements in collection</typeparam>
        /// <param name="argument">The collection</param>
        /// <param name="name">Name of the collection</param>
        [DebuggerStepThrough]
        public static void NotEmpty<T>(IEnumerable<T> argument, string name)
        {
            Check.NotNull(argument, name);
            if (!argument.Any())
            {
                throw new ArgumentException("Argument can't be empty", name);
            }
        }

        /// <summary>
        /// verify that argument is >= 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void Positive(int value, string name)
        {
            if (value < 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument is >= 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void Positive(long value, string name)
        {
            if (value < 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument is >= 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void Positive(decimal value, string name)
        {
            if (value < 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument is > 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void AbsolutePositive(int value, string name)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument is > 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void AbsolutePositive(long value, string name)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument is > 0
        /// </summary>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void AbsolutePositive(decimal value, string name)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Argument must be positive", name);
            }
        }

        /// <summary>
        /// verify that argument matches a regular expression
        /// </summary>
        /// <param name="regex">The pattern to match against.</param>
        /// <param name="value">value to check</param>
        /// <param name="name">Original name of the argument</param>
        [DebuggerStepThrough]
        public static void RegexMatch(string regex, string value, string name)
        {
            Check.NotEmpty(regex, "pattern");
            Check.NotEmpty(value, name);

            if (!Regex.IsMatch(value, regex))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument must match {0}", regex), name);
            }
        }

        /// <summary>
        /// verify that condition is true
        /// </summary>
        /// <param name="expression">Expression to check</param>
        [DebuggerStepThrough]
        public static void Condition(bool expression)
        {
            Condition(expression, "Expected condition failed");
        }

        /// <summary>
        /// verify that condition is true
        /// </summary>
        /// <param name="expression">Expression to check</param>
        /// <param name="message">Message to put in exception if expression is false</param>
        /// <param name="args">Arguments to format message with</param>
        [DebuggerStepThrough]
        public static void Condition(bool expression, string message, params object[] args)
        {
            if (!expression)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, args));
            }
        }

        /// <summary>
        /// Determines whether intance is instance of type T.
        /// </summary>
        /// <typeparam name="T">Type to check against.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name of the variable.</param>
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The interface is very clear this way.")]
        public static void IsInstanceOf<T>(object instance, string name)
        {
            Condition(
                instance is T,
                string.Format(CultureInfo.InvariantCulture, "The {0} instance is not of type {1}", name, typeof(T).Name));
        }

        /// <summary>
        /// Verifies that all interface members of the instance are not null.
        /// </summary>
        /// <typeparam name="T">Type of the instance</typeparam>
        /// <param name="instance">The instance to check.</param>
        public static void InjectedMembers<T>(T instance)
        {
            Check.NotNull(instance, "instance");
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            if (fields == null)
            {
                return;
            }

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsInterface)
                {
                    continue;
                }

                object injected = field.GetValue(instance);
                Check.Condition(injected != null, string.Format(CultureInfo.InvariantCulture, "Injected member {0} is null", field.Name));
            }
        }

        /// <summary>
        /// Verify if an email address is valid
        /// </summary>
        /// <param name="email">The email value.</param>
        /// <param name="name">The name of the variable.</param>
        public static void ValidEmail(string email, string name)
        {
            Check.NotEmpty(email, "email");
            Check.Condition(emailRegex.IsMatch(email), "{0} is not a valid email address", name);
        }
    }
}
