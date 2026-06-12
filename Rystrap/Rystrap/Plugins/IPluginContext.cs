namespace Rystrap.Plugins
{
    /// <summary>
    /// Context provided to plugins for interacting with the Rystrap host.
    /// Each plugin receives its own context instance.
    /// </summary>
    public interface IPluginContext
    {
        /// <summary>Event bus for subscribing to host events.</summary>
        IPluginEventBus Events { get; }

        /// <summary>Access to plugin-specific settings.</summary>
        IPluginSettings Settings { get; }

        /// <summary>Logger for plugin-specific logging.</summary>
        IPluginLogger Logger { get; }

        /// <summary>Path utilities for the plugin.</summary>
        IPluginPaths Paths { get; }

        /// <summary>HTTP client for web requests (shared with host).</summary>
        HttpClient HttpClient { get; }

        /// <summary>Get a service from the host dependency injection container.</summary>
        /// <typeparam name="T">Service type to resolve.</typeparam>
        /// <returns>Service instance or null if not registered.</returns>
        T? GetService<T>() where T : class;
    }
}
