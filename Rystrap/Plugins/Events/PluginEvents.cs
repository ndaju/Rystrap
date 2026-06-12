namespace Rystrap.Plugins.Events
{
    #region Host Lifecycle Events

    /// <summary>Raised when the host application is starting up.</summary>
    public sealed record HostStartingEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
    }

    /// <summary>Raised when the host application is ready (plugins loaded).</summary>
    public sealed record HostReadyEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
    }

    /// <summary>Raised when the host application is shutting down.</summary>
    public sealed record HostShuttingDownEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
    }

    #endregion

    #region Launch Events

    /// <summary>Raised before Roblox is launched.</summary>
    public sealed record RobloxLaunchingEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string LaunchCommandLine { get; init; } = "";
        public bool IsStudioLaunch { get; init; }
    }

    /// <summary>Raised after Roblox is launched successfully.</summary>
    public sealed record RobloxLaunchedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public int ProcessId { get; init; }
        public bool IsStudioLaunch { get; init; }
    }

    /// <summary>Raised when Roblox launch fails.</summary>
    public sealed record RobloxLaunchFailedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string Reason { get; init; } = "";
        public Exception? Exception { get; init; }
    }

    #endregion

    #region Mod Events

    /// <summary>Raised before modifications are applied.</summary>
    public sealed record ModApplyingEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string VersionGuid { get; init; } = "";
    }

    /// <summary>Raised after modifications are applied successfully.</summary>
    public sealed record ModAppliedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string VersionGuid { get; init; } = "";
        public int FilesModified { get; init; }
    }

    /// <summary>Raised when a mod file is removed.</summary>
    public sealed record ModRemovedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string RelativePath { get; init; } = "";
    }

    #endregion

    #region Settings Events

    /// <summary>Raised when a setting is changed.</summary>
    public sealed record SettingsChangedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PropertyName { get; init; } = "";
        public object? OldValue { get; init; }
        public object? NewValue { get; init; }
    }

    #endregion

    #region Plugin Lifecycle Events

    /// <summary>Raised when a plugin is loaded.</summary>
    public sealed record PluginLoadedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PluginId { get; init; } = "";
        public string PluginName { get; init; } = "";
    }

    /// <summary>Raised when a plugin is unloaded.</summary>
    public sealed record PluginUnloadedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PluginId { get; init; } = "";
    }

    /// <summary>Raised when a plugin is enabled.</summary>
    public sealed record PluginEnabledEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PluginId { get; init; } = "";
    }

    /// <summary>Raised when a plugin is disabled.</summary>
    public sealed record PluginDisabledEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PluginId { get; init; } = "";
    }

    /// <summary>Raised when a plugin encounters an error.</summary>
    public sealed record PluginErrorEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public string PluginId { get; init; } = "";
        public string ErrorMessage { get; init; } = "";
        public Exception? Exception { get; init; }
    }

    #endregion

    #region Activity Events

    /// <summary>Raised when the activity watcher detects game join.</summary>
    public sealed record GameJoiningEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public long PlaceId { get; init; }
        public string JobId { get; init; } = "";
    }

    /// <summary>Raised when the player joins a game.</summary>
    public sealed record GameJoinedEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public long PlaceId { get; init; }
        public long UniverseId { get; init; }
    }

    /// <summary>Raised when the player leaves a game.</summary>
    public sealed record GameLeftEvent : IPluginEvent
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? SourcePluginId { get; init; }
        public long PlaceId { get; init; }
        public TimeSpan Duration { get; init; }
    }

    #endregion
}
