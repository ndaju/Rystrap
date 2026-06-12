using System.Collections.ObjectModel;
using System.Windows;

using Rystrap.Models.Persistable;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class GameSettingsDialog
    {
        public ObservableCollection<GameSetting> Entries { get; } = new();
        public string EntryCount => $"{Entries.Count} game{(Entries.Count == 1 ? "" : "s")} configured";

        public GameSettingsDialog()
        {
            DataContext = this;
            InitializeComponent();
            LoadEntries();
        }

        private void LoadEntries()
        {
            var db = GameSettingsDatabase.Load();
            foreach (var entry in db.Entries)
                Entries.Add(entry);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
