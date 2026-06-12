using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for RystrapPage.xaml
    /// </summary>
    public partial class RystrapPage
    {
        public RystrapPage()
        {
            DataContext = new RystrapViewModel();
            InitializeComponent();
        }

        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/ndaju/Rystrap/issues/new/choose",
                UseShellExecute = true
            });
        }
    }
}
