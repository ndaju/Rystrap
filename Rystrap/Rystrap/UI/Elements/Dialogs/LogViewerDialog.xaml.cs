using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class LogViewerDialog : INotifyPropertyChanged
    {
        public class LogFileEntry
        {
            public string FileName { get; set; } = "";
            public string LastModified { get; set; } = "";
            public string SizeKB { get; set; } = "0";
            public string FullPath { get; set; } = "";
        }

        public ObservableCollection<LogFileEntry> LogFiles { get; } = new();

        public string StatusText => $"{LogFiles.Count} log file{(LogFiles.Count == 1 ? "" : "s")} found";

        public event PropertyChangedEventHandler? PropertyChanged;

        private static string LogsDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Roblox", "logs");

        public LogViewerDialog()
        {
            DataContext = this;
            InitializeComponent();
            RefreshLogs();
            LogListView.SelectionChanged += (_, _) =>
                OpenButton.IsEnabled = LogListView.SelectedItem != null;
        }

        private void RefreshLogs()
        {
            LogFiles.Clear();
            if (!Directory.Exists(LogsDir)) return;

            foreach (var file in Directory.GetFiles(LogsDir, "*.log")
                         .OrderByDescending(f => File.GetLastWriteTime(f)))
            {
                var info = new FileInfo(file);
                LogFiles.Add(new LogFileEntry
                {
                    FileName = info.Name,
                    LastModified = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    SizeKB = (info.Length / 1024.0).ToString("F1"),
                    FullPath = info.FullName
                });
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }

        private void OpenSelectedLog()
        {
            if (LogListView.SelectedItem is LogFileEntry entry && File.Exists(entry.FullPath))
            {
                try { Process.Start("notepad.exe", entry.FullPath); }
                catch { Frontend.ShowMessageBox("Could not open the log file.", MessageBoxImage.Error); }
            }
        }

        private void ClearAllLogs()
        {
            var result = Frontend.ShowMessageBox(
                "Delete all Roblox log files?\n\nThis cannot be undone.",
                MessageBoxImage.Warning, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            int deleted = 0;
            if (Directory.Exists(LogsDir))
            {
                foreach (var file in Directory.GetFiles(LogsDir, "*.log"))
                {
                    try { File.Delete(file); deleted++; }
                    catch { }
                }
            }
            RefreshLogs();
            Frontend.ShowMessageBox($"{deleted} log file{(deleted == 1 ? "" : "s")} deleted.", MessageBoxImage.Information);
        }

        private void LogListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenSelectedLog();
        private void OpenButton_Click(object sender, RoutedEventArgs e) => OpenSelectedLog();
        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearAllLogs();
        private void RefreshButton_Click(object sender, RoutedEventArgs e) => RefreshLogs();
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
