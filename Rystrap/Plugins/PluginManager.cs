using System.Collections.Concurrent;
using System.Reflection;

using Rystrap.Exceptions;
using Rystrap.Models;
using Rystrap.Plugins.Events;

namespace Rystrap.Plugins
{
    /// <summary>
    /// Manages plugin discovery, loading, lifecycle, and event distribution.
    /// This is the central coordinator for the plugin system.
    /// </summary>
    public sealed class PluginManager : IDisposable
    {
        private const string LOG_IDENT = "PluginManager";

        private readonly ConcurrentDictionary<string, PluginEntry> _loadedPlugins = new();
        private readonly List<PluginInfo> _availablePlugins = new();
        private readonly PluginEventBus _eventBus = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly PluginState _pluginState;

        private string _pluginsDirectory = "";
        private bool _isInitialized;
        private bool _disposed;

        /// <summary>List of currently loaded plugins.</summary>
        public IReadOnlyList<PluginInfo> LoadedPlugins =>
            _loadedPlugins.Values.Select(e => e.Info).ToList().AsReadOnly();

        /// <summary>List of available plugins (found on disk).</summary>
        public IReadOnlyList<PluginInfo> AvailablePlugins => _availablePlugins.AsReadOnly();

        /// <summary>Event bus for subscribing to plugin events.</summary>
        public IPluginEventBus EventBus => _eventBus;

        /// <summary>Whether the plugin system is enabled.</summary>
        public bool IsEnabled => _pluginState.PluginsEnabled;

        /// <summary>Number of loaded plugins.</summary>
        public int LoadedCount => _loadedPlugins.Count;

        public PluginManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _pluginState = new PluginState();
        }

        /// <summary>
        /// Initialize the plugin system and discover available plugins.
        /// </summary>
        /// <param name="pluginsDirectory">Directory to scan for plugins.</param>
        public async Task InitializeAsync(string pluginsDirectory)
        {
            if (_isInitialized)
            {
                App.Logger.WriteLine(LOG_IDENT, "Plugin system already initialized");
                return;
            }

            const string METHOD = "InitializeAsync";
            App.Logger.WriteLine(LOG_IDENT, $"Initializing plugin system from '{pluginsDirectory}'");

            _pluginsDirectory = pluginsDirectory;

            // Ensure plugins directory exists
            Directory.CreateDirectory(_pluginsDirectory);

            // Load plugin state
            await LoadPluginStateAsync();

            if (!_pluginState.PluginsEnabled)
            {
                App.Logger.WriteLine(LOG_IDENT, "Plugin system is disabled in settings");
                return;
            }

            // Discover available plugins
            DiscoverPlugins();

            // Load all discovered plugins
            await LoadAllPluginsAsync();

            _isInitialized = true;

            App.Logger.WriteLine(LOG_IDENT, $"Plugin system initialized. {_loadedPlugins.Count} plugin(s) loaded.");
        }

        /// <summary>
        /// Scan the plugins directory for plugin DLLs.
        /// </summary>
        private void DiscoverPlugins()
        {
            const string METHOD = "DiscoverPlugins";
            App.Logger.WriteLine(LOG_IDENT, $"Scanning '{_pluginsDirectory}' for plugins...");

            _availablePlugins.Clear();

            if (!Directory.Exists(_pluginsDirectory))
            {
                App.Logger.WriteLine(LOG_IDENT, "Plugins directory does not exist");
                return;
            }

            string[] pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (string pluginPath in pluginFiles)
            {
                try
                {
                    var pluginInfo = InspectPluginAssembly(pluginPath);
                    if (pluginInfo is not null)
                    {
                        _availablePlugins.Add(pluginInfo);
                        App.Logger.WriteLine(LOG_IDENT, $"Discovered plugin: {pluginInfo.Name} v{pluginInfo.Version} ({pluginInfo.Id})");
                    }
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to inspect plugin assembly: {pluginPath}");
                }
            }

            App.Logger.WriteLine(LOG_IDENT, $"Found {_availablePlugins.Count} plugin(s)");
        }

        /// <summary>
        /// Inspect an assembly to check if it contains a valid plugin.
        /// </summary>
        private PluginInfo? InspectPluginAssembly(string pluginPath)
        {
            // Load assembly for inspection (non-collectible, just for metadata)
            var assembly = Assembly.LoadFrom(pluginPath);

            // Find types implementing IRystrapPlugin
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t =>
                    typeof(IRystrapPlugin).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface);

            if (pluginType is null)
                return null;

            // Create temporary instance to get metadata
            if (Activator.CreateInstance(pluginType) is not IRystrapPlugin plugin)
                return null;

            var info = PluginInfo.FromPlugin(plugin, pluginPath);

            // Check version compatibility
            if (plugin.MinimumHostVersion is not null)
            {
                var hostVersion = Utilities.ParseVersionSafe(App.Version);
                if (hostVersion is not null && hostVersion < plugin.MinimumHostVersion)
                {
                    App.Logger.WriteLine(LOG_IDENT,
                        $"Plugin {info.Id} requires Rystrap v{plugin.MinimumHostVersion}, but current is v{App.Version}");
                    info.LastError = $"Requires Rystrap v{plugin.MinimumHostVersion}";
                }
            }

            // Load persisted state
            var stateEntry = PluginStateEntry.Get(_pluginState.Plugins, info.Id);
            if (stateEntry is not null)
            {
                info.IsEnabled = stateEntry.IsEnabled;
                info.LastLoadedAt = stateEntry.LastLoadedAt;
            }

            plugin.Dispose();
            return info;
        }

        /// <summary>
        /// Load all discovered plugins that are enabled.
        /// </summary>
        public async Task LoadAllPluginsAsync()
        {
            const string METHOD = "LoadAllPluginsAsync";

            foreach (var pluginInfo in _availablePlugins)
            {
                if (!pluginInfo.IsEnabled || pluginInfo.LastError is not null)
                    continue;

                if (_loadedPlugins.ContainsKey(pluginInfo.Id))
                    continue;

                try
                {
                    await LoadPluginAsync(pluginInfo.PluginPath);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to load plugin {pluginInfo.Id}");

                    pluginInfo.LastError = ex.Message;

                    var stateEntry = PluginStateEntry.GetOrCreate(_pluginState.Plugins, pluginInfo.Id);
                    stateEntry.LastError = ex.Message;
                }
            }

            await SavePluginStateAsync();
        }

        /// <summary>
        /// Load a single plugin from its DLL path.
        /// </summary>
        public async Task LoadPluginAsync(string pluginPath)
        {
            const string METHOD = "LoadPluginAsync";
            App.Logger.WriteLine(LOG_IDENT, $"Loading plugin from '{pluginPath}'");

            if (!File.Exists(pluginPath))
                throw new FileNotFoundException($"Plugin file not found: {pluginPath}");

            // Create isolated load context
            var loadContext = new PluginLoadContext(pluginPath);

            Assembly assembly;
            try
            {
                assembly = loadContext.LoadFromAssemblyPath(pluginPath);
            }
            catch (Exception ex)
            {
                loadContext.Unload();
                throw new PluginException($"Failed to load assembly: {ex.Message}", ex);
            }

            // Find plugin type
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t =>
                    typeof(IRystrapPlugin).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface);

            if (pluginType is null)
            {
                loadContext.Unload();
                throw new PluginException($"No IRystrapPlugin implementation found in {pluginPath}");
            }

            // Create plugin instance
            IRystrapPlugin plugin;
            try
            {
                plugin = (IRystrapPlugin)Activator.CreateInstance(pluginType)!;
            }
            catch (Exception ex)
            {
                loadContext.Unload();
                throw new PluginException($"Failed to create plugin instance: {ex.Message}", ex);
            }

            // Create plugin context
            var pluginDir = Path.GetDirectoryName(pluginPath)!;
            var context = new PluginContext(plugin.Id, pluginDir, _eventBus, _serviceProvider);

            // Initialize plugin
            try
            {
                await plugin.InitializeAsync(context);
            }
            catch (Exception ex)
            {
                plugin.Dispose();
                loadContext.Unload();
                throw new PluginException(plugin.Id, $"Initialization failed: {ex.Message}", ex);
            }

            // Create entry and store
            var entry = new PluginEntry
            {
                Plugin = plugin,
                Info = PluginInfo.FromPlugin(plugin, pluginPath),
                LoadContext = loadContext,
                Context = context,
                IsEnabled = true,
                LoadedAt = DateTime.UtcNow
            };

            // Update available plugins list
            var existingInfo = _availablePlugins.FirstOrDefault(p => p.Id == plugin.Id);
            if (existingInfo is not null)
            {
                existingInfo.LastLoadedAt = DateTime.UtcNow;
                existingInfo.LastError = null;
            }

            _loadedPlugins[plugin.Id] = entry;

            // Update state
            var stateEntry = PluginStateEntry.GetOrCreate(_pluginState.Plugins, plugin.Id);
            stateEntry.LastLoadedAt = DateTime.UtcNow;
            stateEntry.LastError = null;

            // Enable plugin
            await EnablePluginAsync(plugin.Id);

            // Publish plugin loaded event
            await _eventBus.PublishAsync(new PluginLoadedEvent
            {
                PluginId = plugin.Id,
                PluginName = plugin.Name
            });

            App.Logger.WriteLine(LOG_IDENT, $"Successfully loaded plugin: {plugin.Name} v{plugin.Version}");

            await SavePluginStateAsync();
        }

        /// <summary>
        /// Unload a specific plugin by ID.
        /// </summary>
        public async Task UnloadPluginAsync(string pluginId)
        {
            const string METHOD = "UnloadPluginAsync";

            if (!_loadedPlugins.TryRemove(pluginId, out var entry))
            {
                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} is not loaded");
                return;
            }

            App.Logger.WriteLine(LOG_IDENT, $"Unloading plugin: {pluginId}");

            try
            {
                // Disable first
                if (entry.IsEnabled)
                {
                    await entry.Plugin.DisableAsync();
                }

                // Dispose
                entry.Plugin.Dispose();

                // Unload assembly context
                entry.LoadContext.Unload();

                // Update state
                var stateEntry = PluginStateEntry.Get(_pluginState.Plugins, pluginId);
                if (stateEntry is not null)
                {
                    stateEntry.IsEnabled = false;
                }

                // Update available plugins list
                var info = _availablePlugins.FirstOrDefault(p => p.Id == pluginId);
                if (info is not null)
                {
                    info.IsEnabled = false;
                }

                // Publish event
                await _eventBus.PublishAsync(new PluginUnloadedEvent
                {
                    PluginId = pluginId
                });

                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} unloaded");
                await SavePluginStateAsync();
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Enable a plugin by ID.
        /// </summary>
        public async Task EnablePluginAsync(string pluginId)
        {
            const string METHOD = "EnablePluginAsync";

            if (!_loadedPlugins.TryGetValue(pluginId, out var entry))
            {
                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} is not loaded");
                return;
            }

            if (entry.IsEnabled)
                return;

            App.Logger.WriteLine(LOG_IDENT, $"Enabling plugin: {pluginId}");

            try
            {
                await entry.Plugin.EnableAsync();
                entry.IsEnabled = true;
                entry.LastEnabledAt = DateTime.UtcNow;

                // Update state
                var stateEntry = PluginStateEntry.GetOrCreate(_pluginState.Plugins, pluginId);
                stateEntry.IsEnabled = true;
                stateEntry.LastEnabledAt = DateTime.UtcNow;

                // Update available plugins list
                var info = _availablePlugins.FirstOrDefault(p => p.Id == pluginId);
                if (info is not null)
                {
                    info.IsEnabled = true;
                }

                // Publish event
                await _eventBus.PublishAsync(new PluginEnabledEvent
                {
                    PluginId = pluginId
                });

                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} enabled");
                await SavePluginStateAsync();
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);

                await _eventBus.PublishAsync(new PluginErrorEvent
                {
                    PluginId = pluginId,
                    ErrorMessage = $"Failed to enable: {ex.Message}",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// Disable a plugin by ID.
        /// </summary>
        public async Task DisablePluginAsync(string pluginId)
        {
            const string METHOD = "DisablePluginAsync";

            if (!_loadedPlugins.TryGetValue(pluginId, out var entry))
            {
                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} is not loaded");
                return;
            }

            if (!entry.IsEnabled)
                return;

            App.Logger.WriteLine(LOG_IDENT, $"Disabling plugin: {pluginId}");

            try
            {
                await entry.Plugin.DisableAsync();
                entry.IsEnabled = false;

                // Update state
                var stateEntry = PluginStateEntry.Get(_pluginState.Plugins, pluginId);
                if (stateEntry is not null)
                {
                    stateEntry.IsEnabled = false;
                }

                // Update available plugins list
                var info = _availablePlugins.FirstOrDefault(p => p.Id == pluginId);
                if (info is not null)
                {
                    info.IsEnabled = false;
                }

                // Publish event
                await _eventBus.PublishAsync(new PluginDisabledEvent
                {
                    PluginId = pluginId
                });

                App.Logger.WriteLine(LOG_IDENT, $"Plugin {pluginId} disabled");
                await SavePluginStateAsync();
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);

                await _eventBus.PublishAsync(new PluginErrorEvent
                {
                    PluginId = pluginId,
                    ErrorMessage = $"Failed to disable: {ex.Message}",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// Toggle a plugin's enabled state.
        /// </summary>
        public async Task TogglePluginAsync(string pluginId)
        {
            if (_loadedPlugins.TryGetValue(pluginId, out var entry) && entry.IsEnabled)
            {
                await DisablePluginAsync(pluginId);
            }
            else
            {
                await EnablePluginAsync(pluginId);
            }
        }

        /// <summary>
        /// Get a loaded plugin by ID.
        /// </summary>
        public IRystrapPlugin? GetPlugin(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var entry) ? entry.Plugin : null;
        }

        /// <summary>
        /// Get plugin info by ID.
        /// </summary>
        public PluginInfo? GetPluginInfo(string pluginId)
        {
            return _availablePlugins.FirstOrDefault(p => p.Id == pluginId);
        }

        /// <summary>
        /// Check if a plugin is loaded.
        /// </summary>
        public bool IsPluginLoaded(string pluginId) => _loadedPlugins.ContainsKey(pluginId);

        /// <summary>
        /// Check if a plugin is enabled.
        /// </summary>
        public bool IsPluginEnabled(string pluginId)
        {
            return _loadedPlugins.TryGetValue(pluginId, out var entry) && entry.IsEnabled;
        }

        /// <summary>
        /// Reload all plugins (unload then load again).
        /// </summary>
        public async Task ReloadAllPluginsAsync()
        {
            App.Logger.WriteLine(LOG_IDENT, "Reloading all plugins...");

            await UnloadAllPluginsAsync();
            DiscoverPlugins();
            await LoadAllPluginsAsync();

            App.Logger.WriteLine(LOG_IDENT, "All plugins reloaded");
        }

        /// <summary>
        /// Unload all plugins.
        /// </summary>
        public async Task UnloadAllPluginsAsync()
        {
            var pluginIds = _loadedPlugins.Keys.ToList();

            foreach (string pluginId in pluginIds)
            {
                await UnloadPluginAsync(pluginId);
            }

            _eventBus.ClearAll();
        }

        /// <summary>
        /// Enable the entire plugin system.
        /// </summary>
        public async Task EnablePluginSystemAsync()
        {
            if (_pluginState.PluginsEnabled)
                return;

            _pluginState.PluginsEnabled = true;
            await SavePluginStateAsync();

            App.Logger.WriteLine(LOG_IDENT, "Plugin system enabled");
        }

        /// <summary>
        /// Disable the entire plugin system.
        /// </summary>
        public async Task DisablePluginSystemAsync()
        {
            if (!_pluginState.PluginsEnabled)
                return;

            await UnloadAllPluginsAsync();

            _pluginState.PluginsEnabled = false;
            await SavePluginStateAsync();

            App.Logger.WriteLine(LOG_IDENT, "Plugin system disabled");
        }

        /// <summary>
        /// Load plugin state from disk.
        /// </summary>
        private async Task LoadPluginStateAsync()
        {
            string stateFile = Path.Combine(Paths.Base, "PluginState.json");

            try
            {
                if (File.Exists(stateFile))
                {
                    string json = await File.ReadAllTextAsync(stateFile);
                    var state = JsonSerializer.Deserialize<PluginState>(json);

                    if (state is not null)
                    {
                        _pluginState.PluginsEnabled = state.PluginsEnabled;
                        _pluginState.Plugins = state.Plugins;
                    }
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                App.Logger.WriteLine(LOG_IDENT, "Failed to load plugin state, using defaults");
            }
        }

        /// <summary>
        /// Save plugin state to disk.
        /// </summary>
        private async Task SavePluginStateAsync()
        {
            string stateFile = Path.Combine(Paths.Base, "PluginState.json");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(stateFile)!);
                string json = JsonSerializer.Serialize(_pluginState, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(stateFile, json);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Dispose all loaded plugins and clean up resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            App.Logger.WriteLine(LOG_IDENT, "Disposing plugin manager...");

            // Synchronously unload all plugins
            foreach (var entry in _loadedPlugins.Values)
            {
                try
                {
                    if (entry.IsEnabled)
                    {
                        entry.Plugin.DisableAsync().GetAwaiter().GetResult();
                    }
                    entry.Plugin.Dispose();
                    entry.LoadContext.Unload();
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }

            _loadedPlugins.Clear();
            _eventBus.ClearAll();

            App.Logger.WriteLine(LOG_IDENT, "Plugin manager disposed");
        }
    }

    /// <summary>
    /// Internal class to hold loaded plugin data.
    /// </summary>
    internal sealed class PluginEntry
    {
        /// <summary>The plugin instance.</summary>
        public IRystrapPlugin Plugin { get; init; } = null!;

        /// <summary>Plugin metadata.</summary>
        public PluginInfo Info { get; init; } = null!;

        /// <summary>Assembly load context for isolation.</summary>
        public PluginLoadContext LoadContext { get; init; } = null!;

        /// <summary>Plugin context providing host access.</summary>
        public IPluginContext Context { get; init; } = null!;

        /// <summary>Whether the plugin is currently enabled.</summary>
        public bool IsEnabled { get; set; }

        /// <summary>When the plugin was loaded.</summary>
        public DateTime? LoadedAt { get; set; }

        /// <summary>When the plugin was last enabled.</summary>
        public DateTime? LastEnabledAt { get; set; }

        /// <summary>Last error if plugin failed.</summary>
        public Exception? LastError { get; set; }
    }
}
