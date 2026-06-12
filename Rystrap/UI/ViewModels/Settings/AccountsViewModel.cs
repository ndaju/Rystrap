using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Rystrap.AppData;
using Rystrap.Models.Persistable;

namespace Rystrap.UI.ViewModels.Settings
{
    public class AccountsViewModel : NotifyPropertyChangedViewModel
    {
        private static readonly HttpClient _http = new();

        public ObservableCollection<AccountProfile> Accounts { get; } = new();

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        private AccountProfile? _selectedAccount;
        public AccountProfile? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(CanFetchAvatar));
                if (value is not null)
                {
                    EditName = value.Name;
                    EditRobloxUsername = value.RobloxUsername;
                    EditRobloxCookie = value.RobloxCookie;
                    EditNotes = value.Notes;
                    EditVolume = value.Volume;
                }
            }
        }
        public bool HasSelection => SelectedAccount is not null;
        public bool CanFetchAvatar => SelectedAccount is not null && !string.IsNullOrEmpty(SelectedAccount.RobloxUsername);

        private string _editName = "";
        public string EditName
        {
            get => _editName;
            set { _editName = value; OnPropertyChanged(nameof(EditName)); }
        }

        private string _editRobloxUsername = "";
        public string EditRobloxUsername
        {
            get => _editRobloxUsername;
            set { _editRobloxUsername = value; OnPropertyChanged(nameof(EditRobloxUsername)); }
        }

        private string _editRobloxCookie = "";
        public string EditRobloxCookie
        {
            get => _editRobloxCookie;
            set { _editRobloxCookie = value; OnPropertyChanged(nameof(EditRobloxCookie)); }
        }

        private string _editNotes = "";
        public string EditNotes
        {
            get => _editNotes;
            set { _editNotes = value; OnPropertyChanged(nameof(EditNotes)); }
        }

        private int _editVolume = 100;
        public int EditVolume
        {
            get => _editVolume;
            set { _editVolume = value; OnPropertyChanged(nameof(EditVolume)); }
        }

        public AccountsViewModel()
        {
            LoadAccounts();
        }

        private string GetFilePath() => System.IO.Path.Combine(Paths.Base, "Accounts.json");
        private string GetAvatarDir() => System.IO.Path.Combine(Paths.Base, "avatars");

        private void LoadAccounts()
        {
            Accounts.Clear();
            string path = GetFilePath();
            if (System.IO.File.Exists(path))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<AccountProfile>>(json);
                    if (list is not null)
                        foreach (var a in list) Accounts.Add(a);
                }
                catch { }
            }
            UpdateStatus();
        }

        private void SaveAccounts()
        {
            string path = GetFilePath();
            string json = JsonSerializer.Serialize(Accounts.ToList());
            System.IO.File.WriteAllText(path, json);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            int favs = Accounts.Count(a => a.IsFavorite);
            string favPart = favs > 0 ? $", {favs} favorited" : "";
            StatusText = $"{Accounts.Count} account(s){favPart}";
        }

        private void AddAccount()
        {
            var account = new AccountProfile { Name = $"Account {Accounts.Count + 1}" };
            Accounts.Add(account);
            SelectedAccount = account;
            SaveAccounts();
        }

        private void DuplicateAccount()
        {
            if (SelectedAccount is null) return;
            var copy = new AccountProfile
            {
                Name = $"{SelectedAccount.Name} (copy)",
                RobloxUsername = SelectedAccount.RobloxUsername,
                RobloxCookie = SelectedAccount.RobloxCookie,
                Notes = SelectedAccount.Notes,
            };
            Accounts.Add(copy);
            SelectedAccount = copy;
            SaveAccounts();
        }

        private void RemoveAccount()
        {
            if (SelectedAccount is null) return;
            var result = Frontend.ShowMessageBox($"Remove account \"{SelectedAccount.DisplayText}\"?", MessageBoxImage.Question, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            if (SelectedAccount.HasAvatar)
            {
                try { System.IO.File.Delete(SelectedAccount.AvatarPath); } catch { }
            }
            Accounts.Remove(SelectedAccount);
            SelectedAccount = Accounts.LastOrDefault();
            SaveAccounts();
        }

        private void SaveSelectedAccount()
        {
            if (SelectedAccount is null) return;
            SelectedAccount.Name = EditName;
            SelectedAccount.RobloxUsername = EditRobloxUsername;
            SelectedAccount.RobloxCookie = EditRobloxCookie;
            SelectedAccount.Notes = EditNotes;
            SelectedAccount.Volume = EditVolume;
            SaveAccounts();
        }

        public void ToggleFavorite(AccountProfile account)
        {
            account.IsFavorite = !account.IsFavorite;
            SaveAccounts();
        }

        private async void FetchAvatar()
        {
            if (SelectedAccount is null || string.IsNullOrEmpty(SelectedAccount.RobloxUsername)) return;

            try
            {
                var payload = JsonSerializer.Serialize(new { usernames = new[] { SelectedAccount.RobloxUsername }, excludeBannedUsers = true });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync("https://users.roblox.com/v1/usernames/users", content);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data")[0];
                long userId = data.GetProperty("id").GetInt64();
                string displayName = data.GetProperty("displayName").GetString() ?? "";
                SelectedAccount.RobloxId = userId.ToString();

                if (string.IsNullOrEmpty(EditName) || EditName.StartsWith("Account "))
                {
                    EditName = displayName;
                    SelectedAccount.Name = displayName;
                }

                var thumbResp = await _http.GetAsync($"https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds={userId}&size=48x48&format=Png&isCircular=false");
                thumbResp.EnsureSuccessStatusCode();
                var thumbJson = await thumbResp.Content.ReadAsStringAsync();
                using var thumbDoc = JsonDocument.Parse(thumbJson);
                string imageUrl = thumbDoc.RootElement.GetProperty("data")[0].GetProperty("imageUrl").GetString() ?? "";

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imgResp = await _http.GetAsync(imageUrl);
                    imgResp.EnsureSuccessStatusCode();
                    var imgBytes = await imgResp.Content.ReadAsByteArrayAsync();

                    System.IO.Directory.CreateDirectory(GetAvatarDir());
                    string avatarFile = System.IO.Path.Combine(GetAvatarDir(), $"{SelectedAccount.Id}.png");
                    System.IO.File.WriteAllBytes(avatarFile, imgBytes);
                    SelectedAccount.AvatarPath = avatarFile;
                }

                SaveAccounts();
                Frontend.ShowMessageBox($"Fetched avatar for {displayName}.", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Failed to fetch avatar: {ex.Message}", MessageBoxImage.Error);
            }
        }

        private void MoveUp()
        {
            if (SelectedAccount is null) return;
            int idx = Accounts.IndexOf(SelectedAccount);
            if (idx <= 0) return;
            Accounts.Move(idx, idx - 1);
            SaveAccounts();
        }

        private void MoveDown()
        {
            if (SelectedAccount is null) return;
            int idx = Accounts.IndexOf(SelectedAccount);
            if (idx < 0 || idx >= Accounts.Count - 1) return;
            Accounts.Move(idx, idx + 1);
            SaveAccounts();
        }

        private void QuickSwitch()
        {
            if (SelectedAccount is null) return;
            Utilities.ShellExecute("roblox://placeId=1");
        }

        private void ExportAccounts()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Rystrap accounts (*.Rystrap-accounts)|*.Rystrap-accounts|JSON (*.json)|*.json",
                FileName = "Accounts.json"
            };
            if (dialog.ShowDialog() != true) return;
            string json = JsonSerializer.Serialize(Accounts.ToList());
            System.IO.File.WriteAllText(dialog.FileName, json);
            Frontend.ShowMessageBox($"Exported {Accounts.Count} account(s).", MessageBoxImage.Information);
        }

        private void ImportAccounts()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Rystrap accounts (*.Rystrap-accounts)|*.Rystrap-accounts|JSON (*.json)|*.json"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                string json = System.IO.File.ReadAllText(dialog.FileName);
                var list = JsonSerializer.Deserialize<List<AccountProfile>>(json);
                if (list is null || list.Count == 0)
                {
                    Frontend.ShowMessageBox("No accounts found in file.", MessageBoxImage.Warning);
                    return;
                }
                foreach (var a in list) Accounts.Add(a);
                SaveAccounts();
                Frontend.ShowMessageBox($"Imported {list.Count} account(s).", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Failed to import: {ex.Message}", MessageBoxImage.Error);
            }
        }

        public ICommand AddAccountCommand => new RelayCommand(AddAccount);
        public ICommand DuplicateAccountCommand => new RelayCommand(DuplicateAccount);
        public ICommand RemoveAccountCommand => new RelayCommand(RemoveAccount);
        public ICommand SaveSelectedCommand => new RelayCommand(SaveSelectedAccount);
        public ICommand FetchAvatarCommand => new RelayCommand(FetchAvatar);
        public ICommand MoveUpCommand => new RelayCommand(MoveUp);
        public ICommand MoveDownCommand => new RelayCommand(MoveDown);
        public ICommand QuickSwitchCommand => new RelayCommand(QuickSwitch);
        public ICommand ExportAccountsCommand => new RelayCommand(ExportAccounts);
        public ICommand ImportAccountsCommand => new RelayCommand(ImportAccounts);
    }
}
