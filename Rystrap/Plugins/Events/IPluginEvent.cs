namespace Rystrap.Plugins.Events
{
    /// <summary>
    /// Base interface for all plugin events.
    /// All event classes must implement this interface.
    /// </summary>
    public interface IPluginEvent
    {
        /// <summary>When the event occurred.</summary>
        DateTime Timestamp { get; }

        /// <summary>Source plugin ID (null for host-generated events).</summary>
        string? SourcePluginId { get; }
    }
}
