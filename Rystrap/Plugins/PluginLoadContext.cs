using System.Reflection;
using System.Runtime.Loader;

namespace Rystrap.Plugins
{
    /// <summary>
    /// Custom AssemblyLoadContext for loading plugins in isolation.
    /// Uses collectible context so plugins can be unloaded.
    /// Shared assemblies (like Rystrap core) are resolved from the default context.
    /// </summary>
    internal sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly string _pluginDirectory;

        /// <summary>
        /// Create a new PluginLoadContext for a plugin DLL.
        /// </summary>
        /// <param name="pluginPath">Full path to the plugin DLL.</param>
        public PluginLoadContext(string pluginPath)
            : base(name: Path.GetFileNameWithoutExtension(pluginPath), isCollectible: true)
        {
            _pluginDirectory = Path.GetDirectoryName(pluginPath)!;
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <summary>
        /// Resolve assembly from plugin directory, falling back to default context.
        /// </summary>
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // First try to resolve from the plugin's own directory
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath is not null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            // If the assembly is a Rystrap assembly, let the default context handle it
            // This allows plugins to share types with the host
            if (assemblyName.Name?.StartsWith("Rystrap", StringComparison.OrdinalIgnoreCase) == true)
            {
                return null; // Fall back to default AssemblyLoadContext
            }

            // For other shared assemblies (like CommunityToolkit.Mvvm), also fall back
            return null;
        }

        /// <summary>
        /// Resolve unmanaged DLL from plugin directory.
        /// </summary>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath is not null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Get the path to the plugin's assembly.
        /// </summary>
        public string PluginDirectory => _pluginDirectory;
    }
}
