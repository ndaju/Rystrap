namespace Rystrap.Plugins
{
    /// <summary>
    /// Logger interface for plugins.
    /// Logs are prefixed with the plugin ID for easy filtering.
    /// </summary>
    public interface IPluginLogger
    {
        /// <summary>Log an informational message.</summary>
        void WriteLine(string message);

        /// <summary>Log an informational message with category.</summary>
        void WriteLine(string category, string message);

        /// <summary>Log an error message.</summary>
        void WriteError(string message);

        /// <summary>Log an error message with category.</summary>
        void WriteError(string category, string message);

        /// <summary>Log a warning message.</summary>
        void WriteWarning(string message);

        /// <summary>Log a warning message with category.</summary>
        void WriteWarning(string category, string message);

        /// <summary>Log an exception with full stack trace.</summary>
        void WriteException(string message, Exception exception);
    }
}
