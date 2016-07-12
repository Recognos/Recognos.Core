using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Recognos.Core
{
    /// <summary>
    /// Helper class to help ensure that all referenced assemblies are loaded.
    /// This class is useful when all referenced assemblies must be scanned for types, for registering 
    /// them in a DI container.
    /// 
    /// If an assembly is already loaded, it will not be loaded again.
    /// 
    /// http://msdn.microsoft.com/en-us/library/system.appdomain.getassemblies.aspx
    /// 
    /// The call to AppDomain.GetAssemblies only returns:
    ///  "assemblies that have been loaded into the execution context of this application domain."
    /// 
    /// The JIT might not load an assembly when the scan happens.
    /// 
    /// </summary>
    public sealed class ReferencedAssemblyLoader
    {
        private static readonly object padlock = new object();
        private readonly Predicate<string> assemblyMatch;
        private readonly IDictionary<string, Assembly> loadedAssemblies;

        private ReferencedAssemblyLoader(Assembly root, Predicate<string> assemblyMatch)
        {
            this.assemblyMatch = assemblyMatch;

            this.loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .ToDictionary(a => a.FullName, a => a);

            LoadReferencedAssemblies(root);
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// </summary>
        public static void EnsureReferencesAreLoaded()
        {
            EnsureReferencesAreLoaded(s => true);
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// </summary>
        /// <param name="root">Assembly to start the scanning from.</param>
        public static void EnsureReferencesAreLoaded(Assembly root)
        {
            EnsureReferencesAreLoaded(root, s => true);
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// Only assemblies with <paramref name="assemblyPrefix"/> prefix are considered.
        /// </summary>
        /// <param name="assemblyPrefix">Prefix for assembly name to consider.</param>
        public static void EnsureReferencesAreLoaded(string assemblyPrefix)
        {
            EnsureReferencesAreLoaded(s => string.IsNullOrEmpty(assemblyPrefix) || s.StartsWith(assemblyPrefix));
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// Only assemblies with <paramref name="assemblyPrefix"/> prefix are considered.
        /// </summary>
        /// <param name="root">Assembly to start the scanning from.</param>
        /// <param name="assemblyPrefix">Prefix for assembly name to consider.</param>
        public static void EnsureReferencesAreLoaded(Assembly root, string assemblyPrefix)
        {
            EnsureReferencesAreLoaded(root, s => string.IsNullOrEmpty(assemblyPrefix) || s.StartsWith(assemblyPrefix));
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// Only assemblies for which <paramref name="assemblyMatch"/> predicate is true are considered.
        /// </summary>
        /// <param name="assemblyMatch">Predicate to apply when considering assemblies.</param>
        public static void EnsureReferencesAreLoaded(Predicate<string> assemblyMatch)
        {
            Check.NotNull(assemblyMatch, nameof(assemblyMatch));
            lock (padlock)
            {
                new ReferencedAssemblyLoader(Assembly.GetEntryAssembly(), assemblyMatch);
            }
        }

        /// <summary>
        /// Ensures recursively that all assemblies referenced by this assembly are loaded.
        /// Only assemblies for which <paramref name="assemblyMatch"/> predicate is true are considered.
        /// </summary>
        /// <param name="root">Assembly to start the scanning from.</param>
        /// <param name="assemblyMatch">Predicate to apply when considering assemblies.</param>
        public static void EnsureReferencesAreLoaded(Assembly root, Predicate<string> assemblyMatch)
        {
            Check.NotNull(root, nameof(root));
            Check.NotNull(assemblyMatch, nameof(assemblyMatch));
            new ReferencedAssemblyLoader(root, assemblyMatch);
        }

        /// <summary>
        /// Recursively load referenced assemblies.
        /// </summary>
        /// <param name="root">Assembly for which to load referenced assemblies</param>
        private void LoadReferencedAssemblies(Assembly root)
        {
            var references = root.GetReferencedAssemblies().Where(a => this.assemblyMatch(a.FullName));

            foreach (var reference in references)
            {
                Assembly assembly;
                if (!this.loadedAssemblies.TryGetValue(reference.FullName, out assembly))
                {
                    assembly = Assembly.Load(reference);
                    this.loadedAssemblies.Add(assembly.FullName, assembly);
                }
                LoadReferencedAssemblies(assembly);
            }
        }
    }
}
