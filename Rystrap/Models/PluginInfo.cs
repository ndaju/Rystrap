namespace Rystrap.Models
{
    /// <summary>
    /// Metadata about a plugin, used for discovery and marketplace display.
    /// </summary>
    public class PluginInfo
    {
        /// <summary>Unique identifier for the plugin (e.g., "com.author.pluginname").</summary>
        public string Id { get; set; } = "";

        /// <summary>Display name of the plugin.</summary>
        public string Name { get; set; } = "";

        /// <summary>Plugin author name.</summary>
        public string Author { get; set; } = "";

        /// <summary>Plugin version.</summary>
        public Version Version { get; set; } = new(1, 0, 0);

        /// <summary>Brief description of what the plugin does.</summary>
        public string Description { get; set; } = "";

        /// <summary>Minimum Rystrap version required.</summary>
        public Version? MinimumHostVersion { get; set; }

        /// <summary>Path to the plugin DLL file.</summary>
        public string PluginPath { get; set; } = "";

        /// <summary>Whether the plugin is currently enabled.</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>When the plugin was last loaded into memory.</summary>
        public DateTime? LastLoadedAt { get; set; }

        /// <summary>Last error message if plugin failed to load.</summary>
        public string? LastError { get; set; }

        /// <summary>Tags for categorization (e.g., "ui", "integration", "utility").</summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>Plugin homepage or repository URL.</summary>
        public string? Homepage { get; set; }

        /// <summary>URL to the plugin's icon image.</summary>
        public string? IconUrl { get; set; }

        /// <summary>License identifier (e.g., "MIT", "GPL-3.0").</summary>
        public string? License { get; set; }

        /// <summary>Dependencies on other plugins (list of plugin IDs).</summary>
        public List<string> Dependencies { get; set; } = new();

        /// <summary>
        /// Create a PluginInfo from an IRystrapPlugin instance.
        /// </summary>
        public static PluginInfo FromPlugin(IRystrapPlugin plugin, string pluginPath = "")
        {
            return new PluginInfo
            {
                Id = plugin.Id,
                Name = plugin.Name,
                Author = plugin.Author,
                Version = plugin.Version,
                Description = plugin.Description,
                MinimumHostVersion = plugin.MinimumHostVersion,
                PluginPath = pluginPath
            };
        }
    }
}
