namespace Rystrap.Plugins
{
    /// <summary>
    /// Main interface that all Rystrap plugins must implement.
    /// Plugins are loaded from DLLs in the Plugins directory.
    /// </summary>
    public interface IRystrapPlugin : IDisposable
    {
        /// <summary>Unique identifier for the plugin (e.g., "com.author.pluginname").</summary>
        string Id { get; }

        /// <summary>Display name of the plugin.</summary>
        string Name { get; }

        /// <summary>Plugin author name.</summary>
        string Author { get; }

        /// <summary>Plugin version (SemVer format recommended).</summary>
        Version Version { get; }

        /// <summary>Brief description of what the plugin does.</summary>
        string Description { get; }

        /// <summary>Minimum Rystrap version required by this plugin.</summary>
        Version? MinimumHostVersion { get; }

        /// <summary>
        /// Initialize the plugin with the provided context.
        /// Called once when the plugin is first loaded.
        /// </summary>
        /// <param name="context">Context providing access to host functionality.</param>
        Task InitializeAsync(IPluginContext context);

        /// <summary>
        /// Called when the plugin is enabled.
        /// Set up event subscriptions and background tasks here.
        /// </summary>
        Task EnableAsync();

        /// <summary>
        /// Called when the plugin is disabled.
        /// Clean up event subscriptions and cancel background tasks here.
        /// </summary>
        Task DisableAsync();

        /// <summary>
        /// Get the settings view model for this plugin.
        /// Return null if the plugin has no configurable settings.
        /// </summary>
        /// <returns>ViewModel for settings page, or null if no settings.</returns>
        object? GetSettingsViewModel();
    }
}
