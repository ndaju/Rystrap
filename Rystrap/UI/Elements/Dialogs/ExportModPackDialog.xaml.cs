using System.Windows;
using Rystrap.UI.Elements.Base;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class ExportModPackDialog : WpfUiWindow
    {
        public string PackName { get; private set; } = "";
        public string Author { get; private set; } = "";
        public string Description { get; private set; } = "";

        public ExportModPackDialog()
        {
            InitializeComponent();
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PackNameBox.Text))
            {
                PackNameBox.Focus();
                return;
            }

            PackName = PackNameBox.Text.Trim();
            Author = string.IsNullOrWhiteSpace(AuthorBox.Text) ? "Unknown" : AuthorBox.Text.Trim();
            Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? "Exported with Rystrap" : DescriptionBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}
