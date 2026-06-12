namespace Rystrap.Exceptions
{
    /// <summary>
    /// Exception thrown by plugin operations.
    /// Includes the plugin ID for easier debugging.
    /// </summary>
    public class PluginException : Exception
    {
        /// <summary>The ID of the plugin that caused the exception.</summary>
        public string? PluginId { get; }

        public PluginException(string message)
            : base(message)
        {
        }

        public PluginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PluginException(string pluginId, string message)
            : base($"[{pluginId}] {message}")
        {
            PluginId = pluginId;
        }

        public PluginException(string pluginId, string message, Exception innerException)
            : base($"[{pluginId}] {message}", innerException)
        {
            PluginId = pluginId;
        }
    }
}
