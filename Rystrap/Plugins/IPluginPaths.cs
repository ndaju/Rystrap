namespace Rystrap.Plugins
{
    /// <summary>
    /// Path utilities for plugins.
    /// Provides access to plugin-specific directories and host directories.
    /// </summary>
    public interface IPluginPaths
    {
        /// <summary>Root directory where the plugin DLL is located.</summary>
        string PluginDirectory { get; }

        /// <summary>Writable data directory for the plugin (persists across updates).</summary>
        string DataDirectory { get; }

        /// <summary>Cache directory for the plugin (can be cleared).</summary>
        string CacheDirectory { get; }

        /// <summary>Rystrap base installation directory (read-only).</summary>
        string HostBaseDirectory { get; }

        /// <summary>Rystrap modifications directory.</summary>
        string HostModificationsDirectory { get; }

        /// <summary>
        /// Get a path relative to the plugin's data directory.
        /// </summary>
        /// <param name="relativePath">Relative path.</param>
        /// <returns>Full path in the data directory.</returns>
        string GetDataPath(string relativePath);

        /// <summary>
        /// Get a path relative to the plugin's cache directory.
        /// </summary>
        /// <param name="relativePath">Relative path.</param>
        /// <returns>Full path in the cache directory.</returns>
        string GetCachePath(string relativePath);
    }
}
