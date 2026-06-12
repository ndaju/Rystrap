using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;

using Rystrap.Models.Persistable;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class BanHistoryDialog
    {
        public ObservableCollection<BanEvent> Events { get; } = new();
        public string EventCount => $"{Events.Count} event{(Events.Count == 1 ? "" : "s")} recorded";

        public BanHistoryDialog()
        {
            DataContext = this;
            InitializeComponent();
            LoadEvents();
        }

        private void LoadEvents()
        {
            string path = System.IO.Path.Combine(Paths.Base, "BanHistory.json");
            if (System.IO.File.Exists(path))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<BanEvent>>(json);
                    if (list is not null)
                    {
                        foreach (var ev in list.OrderByDescending(e => e.Timestamp))
                            Events.Add(ev);
                    }
                }
                catch { }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
