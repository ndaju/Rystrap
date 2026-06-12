namespace Rystrap.Plugins
{
    /// <summary>
    /// Provides access to plugin-specific settings.
    /// Each plugin has its own isolated settings store.
    /// </summary>
    public interface IPluginSettings
    {
        /// <summary>The file path where this plugin's settings are stored.</summary>
        string SettingsFilePath { get; }

        /// <summary>
        /// Get a setting value by key.
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="key">Setting key.</param>
        /// <param name="defaultValue">Default value if key doesn't exist.</param>
        /// <returns>Setting value or default.</returns>
        T? GetValue<T>(string key, T? defaultValue = default);

        /// <summary>
        /// Set a setting value by key.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="key">Setting key.</param>
        /// <param name="value">Value to store.</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Check if a setting key exists.
        /// </summary>
        /// <param name="key">Setting key to check.</param>
        /// <returns>True if the key exists.</returns>
        bool HasValue(string key);

        /// <summary>
        /// Remove a setting by key.
        /// </summary>
        /// <param name="key">Setting key to remove.</param>
        /// <returns>True if the key was found and removed.</returns>
        bool RemoveValue(string key);

        /// <summary>
        /// Get all setting keys.
        /// </summary>
        /// <returns>Collection of all setting keys.</returns>
        IEnumerable<string> GetKeys();

        /// <summary>
        /// Save settings to disk.
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// Load settings from disk.
        /// </summary>
        Task LoadAsync();
    }
}
