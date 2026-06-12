using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Rystrap.Models.Discord;

namespace Rystrap.UI.ViewModels.Settings
{
    public class DiscordSettingsViewModel : NotifyPropertyChangedViewModel
    {
        public ICommand ApplyTemplateCommand => new RelayCommand<StatusTemplate>(ApplyTemplate);
        public ICommand AddCustomButtonCommand => new RelayCommand(AddCustomButton);
        public ICommand RemoveCustomButtonCommand => new RelayCommand<DiscordButtonConfig>(RemoveCustomButton);

        public List<StatusTemplate> AvailableTemplates => DiscordStatusTemplates.Templates;

        // ===== Discord Rich Presence (moved from Integrations) =====

        public bool DiscordActivityEnabled
        {
            get => App.Settings.Prop.UseDiscordRichPresence;
            set
            {
                App.Settings.Prop.UseDiscordRichPresence = value;
                OnPropertyChanged(nameof(DiscordActivityEnabled));
                if (!value)
                {
                    DiscordActivityJoinEnabled = value;
                    DiscordAccountOnProfile = value;
                    OnPropertyChanged(nameof(DiscordActivityJoinEnabled));
                    OnPropertyChanged(nameof(DiscordAccountOnProfile));
                }
            }
        }

        public bool DiscordActivityJoinEnabled
        {
            get => !App.Settings.Prop.HideRPCButtons;
            set { App.Settings.Prop.HideRPCButtons = !value; OnPropertyChanged(nameof(DiscordActivityJoinEnabled)); }
        }

        public bool DiscordAccountOnProfile
        {
            get => App.Settings.Prop.ShowAccountOnRichPresence;
            set { App.Settings.Prop.ShowAccountOnRichPresence = value; OnPropertyChanged(nameof(DiscordAccountOnProfile)); }
        }

        // ===== Enhanced Discord RPC =====

        public bool EnableCustomStatus
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.EnableCustomStatus;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.EnableCustomStatus = value;
                OnPropertyChanged(nameof(EnableCustomStatus));
                OnPropertyChanged(nameof(IsCustomStatusEnabled));
            }
        }

        public bool IsCustomStatusEnabled => EnableCustomStatus;

        public string CustomStatusMessage
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.CustomStatusMessage;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.CustomStatusMessage = value;
                OnPropertyChanged(nameof(CustomStatusMessage));
            }
        }

        public DiscordActivityType SelectedActivityType
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.ActivityType;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.ActivityType = value;
                OnPropertyChanged(nameof(SelectedActivityType));
            }
        }

        public TimestampMode SelectedTimestampMode
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.TimestampDisplay;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.TimestampDisplay = value;
                OnPropertyChanged(nameof(SelectedTimestampMode));
            }
        }

        public bool ShowPartySize
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.ShowPartySize;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.ShowPartySize = value;
                OnPropertyChanged(nameof(ShowPartySize));
                OnPropertyChanged(nameof(IsPartySizeEnabled));
            }
        }

        public bool IsPartySizeEnabled => ShowPartySize;

        public int PartyCurrentSize
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.PartyCurrentSize;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.PartyCurrentSize = value;
                OnPropertyChanged(nameof(PartyCurrentSize));
            }
        }

        public int PartyMaxSize
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.PartyMaxSize;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.PartyMaxSize = value;
                OnPropertyChanged(nameof(PartyMaxSize));
            }
        }

        public bool EnableCustomButtons
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.EnableCustomButtons;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.EnableCustomButtons = value;
                OnPropertyChanged(nameof(EnableCustomButtons));
                OnPropertyChanged(nameof(IsCustomButtonsEnabled));
            }
        }

        public bool IsCustomButtonsEnabled => EnableCustomButtons;

        public ObservableCollection<DiscordButtonConfig> CustomButtons
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.CustomButtons;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.CustomButtons = value;
                OnPropertyChanged(nameof(CustomButtons));
            }
        }

        public string? SelectedTemplateId
        {
            get => App.Settings.Prop.EnhancedDiscordPresence.SelectedTemplate;
            set
            {
                App.Settings.Prop.EnhancedDiscordPresence.SelectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplateId));
            }
        }

        private void ApplyTemplate(StatusTemplate? template)
        {
            if (template == null) return;

            EnableCustomStatus = true;
            SelectedActivityType = template.ActivityType;
            CustomStatusMessage = template.Details;
            SelectedTimestampMode = template.TimestampMode;
            ShowPartySize = template.ShowPartySize;
            SelectedTemplateId = template.Id;

            OnPropertyChanged(nameof(EnableCustomStatus));
            OnPropertyChanged(nameof(SelectedActivityType));
            OnPropertyChanged(nameof(CustomStatusMessage));
            OnPropertyChanged(nameof(SelectedTimestampMode));
            OnPropertyChanged(nameof(ShowPartySize));
            OnPropertyChanged(nameof(SelectedTemplateId));
        }

        private void AddCustomButton()
        {
            CustomButtons.Add(new DiscordButtonConfig
            {
                Label = "Visit Website",
                Url = "https://"
            });

            OnPropertyChanged(nameof(CustomButtons));
        }

        private void RemoveCustomButton(DiscordButtonConfig? button)
        {
            if (button == null) return;

            CustomButtons.Remove(button);
            OnPropertyChanged(nameof(CustomButtons));
        }
    }
}
