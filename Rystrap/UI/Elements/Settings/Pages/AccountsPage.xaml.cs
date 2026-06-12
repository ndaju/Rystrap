using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
using Rystrap.Models.Persistable;
using Rystrap.UI.ViewModels.Settings;

namespace Rystrap.UI.Elements.Settings.Pages
{
    public partial class AccountsPage
    {
        public AccountsPage()
        {
            DataContext = new AccountsViewModel();
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Star_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as System.Windows.FrameworkElement;
            var account = element?.DataContext as AccountProfile;
            var vm = DataContext as AccountsViewModel;
            if (account is not null && vm is not null)
            {
                vm.ToggleFavorite(account);
            }
        }
    }
}
