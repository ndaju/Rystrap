using System.Collections.ObjectModel;

namespace Rystrap.Models.Discord
{
    public enum DiscordActivityType
    {
        Playing = 0,
        Streaming = 1,
        Listening = 2,
        Watching = 3,
        Competing = 5
    }

    public enum TimestampMode
    {
        None = 0,
        Elapsed = 1,
        Remaining = 2
    }

    public class EnhancedPresenceConfig
    {
        public bool EnableCustomStatus { get; set; } = false;

        public string CustomStatusMessage { get; set; } = string.Empty;

        public DiscordActivityType ActivityType { get; set; } = DiscordActivityType.Playing;

        public TimestampMode TimestampDisplay { get; set; } = TimestampMode.Elapsed;

        public bool ShowPartySize { get; set; } = false;

        public int PartyCurrentSize { get; set; } = 0;

        public int PartyMaxSize { get; set; } = 0;

        public bool EnableCustomButtons { get; set; } = false;

        public ObservableCollection<DiscordButtonConfig> CustomButtons { get; set; } = new();

        public string? SelectedTemplate { get; set; } = null;
    }

    public class DiscordButtonConfig
    {
        public string Label { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }

    public static class DiscordStatusTemplates
    {
        public static readonly List<StatusTemplate> Templates = new()
        {
            new StatusTemplate
            {
                Id = "playing",
                Name = "Playing",
                ActivityType = DiscordActivityType.Playing,
                Details = "Playing {game}",
                State = "In server by {creator}"
            },
            new StatusTemplate
            {
                Id = "streaming",
                Name = "Streaming",
                ActivityType = DiscordActivityType.Streaming,
                Details = "Streaming {game}",
                State = "Live on stream",
                StreamingUrl = "https://twitch.tv/"
            },
            new StatusTemplate
            {
                Id = "listening",
                Name = "Listening",
                ActivityType = DiscordActivityType.Listening,
                Details = "Listening to {game}",
                State = "Enjoying the vibes"
            },
            new StatusTemplate
            {
                Id = "watching",
                Name = "Watching",
                ActivityType = DiscordActivityType.Watching,
                Details = "Watching {game}",
                State = "Spectating the action"
            },
            new StatusTemplate
            {
                Id = "competing",
                Name = "Competing",
                ActivityType = DiscordActivityType.Competing,
                Details = "Competing in {game}",
                State = "In a match"
            },
            new StatusTemplate
            {
                Id = "chill",
                Name = "Chilling",
                ActivityType = DiscordActivityType.Playing,
                Details = "Just vibing in {game}",
                State = "Relaxing in a server",
                TimestampMode = TimestampMode.None
            },
            new StatusTemplate
            {
                Id = "grinding",
                Name = "Grinding",
                ActivityType = DiscordActivityType.Playing,
                Details = "Grinding {game}",
                State = "by {creator}",
                TimestampMode = TimestampMode.Elapsed
            },
            new StatusTemplate
            {
                Id = "speedrun",
                Name = "Speedrun",
                ActivityType = DiscordActivityType.Competing,
                Details = "Speedrunning {game}",
                State = "Race against time",
                TimestampMode = TimestampMode.Elapsed
            }
        };

        public static StatusTemplate? GetById(string id)
        {
            return Templates.FirstOrDefault(t => t.Id == id);
        }
    }

    public class StatusTemplate
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DiscordActivityType ActivityType { get; set; } = DiscordActivityType.Playing;

        public string Details { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public TimestampMode TimestampMode { get; set; } = TimestampMode.Elapsed;

        public string? StreamingUrl { get; set; } = null;

        public bool ShowPartySize { get; set; } = false;
    }
}
