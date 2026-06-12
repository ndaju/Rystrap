using System.Collections.ObjectModel;

namespace Rystrap.Models
{
    /// <summary>
    /// Persisted state for the plugin system.
    /// Stored in PluginState.json in the Rystrap base directory.
    /// </summary>
    public class PluginState
    {
        /// <summary>Whether the plugin system is enabled globally.</summary>
        public bool PluginsEnabled { get; set; } = true;

        /// <summary>Collection of individual plugin states.</summary>
        public ObservableCollection<PluginStateEntry> Plugins { get; set; } = new();
    }

    /// <summary>
    /// State for an individual plugin.
    /// </summary>
    public class PluginStateEntry
    {
        /// <summary>Plugin identifier (matches PluginInfo.Id).</summary>
        public string PluginId { get; set; } = "";

        /// <summary>Whether this specific plugin is enabled.</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>When the plugin was last loaded.</summary>
        public DateTime? LastLoadedAt { get; set; }

        /// <summary>When the plugin was last enabled.</summary>
        public DateTime? LastEnabledAt { get; set; }

        /// <summary>Plugin-specific settings stored as JSON elements.</summary>
        public Dictionary<string, JsonElement> Settings { get; set; } = new();

        /// <summary>Last error message if plugin failed.</summary>
        public string? LastError { get; set; }

        /// <summary>
        /// Get or create a state entry for a plugin ID.
        /// </summary>
        public static PluginStateEntry GetOrCreate(ObservableCollection<PluginStateEntry> plugins, string pluginId)
        {
            var existing = plugins.FirstOrDefault(p => p.PluginId == pluginId);
            if (existing is not null)
                return existing;

            var entry = new PluginStateEntry { PluginId = pluginId };
            plugins.Add(entry);
            return entry;
        }

        /// <summary>
        /// Get the state entry for a plugin ID, or null if not found.
        /// </summary>
        public static PluginStateEntry? Get(ObservableCollection<PluginStateEntry> plugins, string pluginId)
        {
            return plugins.FirstOrDefault(p => p.PluginId == pluginId);
        }
    }
}
