using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Rystrap.Models.Persistable
{
    public class AccountProfile : INotifyPropertyChanged
    {
        private string _name = "Default";
        private string _robloxUsername = "";
        private string _robloxCookie = "";
        private bool _isFavorite;
        private string _notes = "";

        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayText)); }
        }

        public string RobloxUsername
        {
            get => _robloxUsername;
            set { _robloxUsername = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayText)); OnPropertyChanged(nameof(Initial)); }
        }

        public string RobloxCookie
        {
            get => _robloxCookie;
            set { _robloxCookie = value; OnPropertyChanged(); }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(nameof(IsFavorite)); OnPropertyChanged(nameof(FavoriteStar)); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public string RobloxId { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public int Volume { get; set; } = 100;
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [JsonIgnore]
        public string DisplayText => string.IsNullOrEmpty(RobloxUsername) ? Name : $"{Name} ({RobloxUsername})";

        [JsonIgnore]
        public string Initial => string.IsNullOrEmpty(RobloxUsername) ? "?" : RobloxUsername[..1].ToUpper();

        [JsonIgnore]
        public bool HasAvatar => !string.IsNullOrEmpty(AvatarPath) && System.IO.File.Exists(AvatarPath);

        [JsonIgnore]
        public string FavoriteStar => IsFavorite ? "★" : "☆";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
