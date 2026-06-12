namespace Rystrap.Plugins
{
    /// <summary>
    /// Concrete implementation of IPluginContext for plugins.
    /// Provides access to host functionality in a controlled manner.
    /// </summary>
    internal sealed class PluginContext : IPluginContext
    {
        private readonly string _pluginId;
        private readonly IServiceProvider _serviceProvider;

        /// <inheritdoc/>
        public IPluginEventBus Events { get; }

        /// <inheritdoc/>
        public IPluginSettings Settings { get; }

        /// <inheritdoc/>
        public IPluginLogger Logger { get; }

        /// <inheritdoc/>
        public IPluginPaths Paths { get; }

        /// <inheritdoc/>
        public HttpClient HttpClient { get; }

        public PluginContext(
            string pluginId,
            string pluginDirectory,
            PluginEventBus eventBus,
            IServiceProvider serviceProvider)
        {
            _pluginId = pluginId;
            _serviceProvider = serviceProvider;

            Events = eventBus;
            Settings = new PluginSettings(pluginDirectory);
            Logger = new PluginLogger(pluginId);
            Paths = new PluginPaths(pluginDirectory);
            HttpClient = App.HttpClient;
        }

        /// <inheritdoc/>
        public T? GetService<T>() where T : class
        {
            return _serviceProvider.GetService(typeof(T)) as T;
        }
    }

    /// <summary>
    /// Plugin-specific settings implementation.
    /// Stores settings as a JSON file in the plugin's data directory.
    /// </summary>
    internal sealed class PluginSettings : IPluginSettings
    {
        private readonly string _settingsFilePath;
        private Dictionary<string, JsonElement> _settings = new();
        private bool _isDirty;

        public string SettingsFilePath => _settingsFilePath;

        public PluginSettings(string pluginDirectory)
        {
            string dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.ProjectName,
                "Plugins",
                Path.GetFileName(pluginDirectory)
            );
            _settingsFilePath = Path.Combine(dataDir, "settings.json");
        }

        public T? GetValue<T>(string key, T? defaultValue = default)
        {
            if (_settings.TryGetValue(key, out var element))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(element.GetRawText());
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            var element = JsonSerializer.SerializeToElement(value);
            _settings[key] = element;
            _isDirty = true;
        }

        public bool HasValue(string key) => _settings.ContainsKey(key);

        public bool RemoveValue(string key)
        {
            if (_settings.Remove(key))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetKeys() => _settings.Keys;

        public async Task SaveAsync()
        {
            if (!_isDirty)
                return;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath)!);
                string json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_settingsFilePath, json);
                _isDirty = false;
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("PluginSettings::SaveAsync", ex);
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = await File.ReadAllTextAsync(_settingsFilePath);
                    _settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
                    _isDirty = false;
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("PluginSettings::LoadAsync", ex);
            }
        }
    }

    /// <summary>
    /// Plugin-specific logger implementation.
    /// Prefixes all log messages with the plugin ID.
    /// </summary>
    internal sealed class PluginLogger : IPluginLogger
    {
        private readonly string _pluginId;

        public PluginLogger(string pluginId)
        {
            _pluginId = pluginId;
        }

        public void WriteLine(string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}", message);
        }

        public void WriteLine(string category, string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}::{category}", message);
        }

        public void WriteError(string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}::ERROR", message);
        }

        public void WriteError(string category, string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}::{category}::ERROR", message);
        }

        public void WriteWarning(string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}::WARNING", message);
        }

        public void WriteWarning(string category, string message)
        {
            App.Logger.WriteLine($"Plugin::{_pluginId}::{category}::WARNING", message);
        }

        public void WriteException(string message, Exception exception)
        {
            App.Logger.WriteException($"Plugin::{_pluginId}", exception);
            App.Logger.WriteLine($"Plugin::{_pluginId}::EXCEPTION", message);
        }
    }

    /// <summary>
    /// Plugin-specific paths implementation.
    /// Provides access to plugin directories and host directories.
    /// </summary>
    internal sealed class PluginPaths : IPluginPaths
    {
        private readonly string _pluginDirectory;
        private readonly string _dataDirectory;
        private readonly string _cacheDirectory;

        public string PluginDirectory => _pluginDirectory;
        public string HostBaseDirectory => Paths.Base;
        public string HostModificationsDirectory => Paths.Modifications;

        public string DataDirectory => _dataDirectory;
        public string CacheDirectory => _cacheDirectory;

        public PluginPaths(string pluginDirectory)
        {
            _pluginDirectory = pluginDirectory;

            string pluginName = Path.GetFileName(pluginDirectory);
            string baseAppData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                App.ProjectName,
                "Plugins"
            );

            _dataDirectory = Path.Combine(baseAppData, pluginName, "Data");
            _cacheDirectory = Path.Combine(baseAppData, pluginName, "Cache");

            // Ensure directories exist
            Directory.CreateDirectory(_dataDirectory);
            Directory.CreateDirectory(_cacheDirectory);
        }

        public string GetDataPath(string relativePath)
        {
            return Path.Combine(_dataDirectory, relativePath);
        }

        public string GetCachePath(string relativePath)
        {
            return Path.Combine(_cacheDirectory, relativePath);
        }
    }
}
