using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Rystrap.Models.Persistable;

namespace Rystrap.UI.Elements.Dialogs
{
    public partial class AccountPickerDialog
    {
        public AccountProfile? SelectedAccount { get; private set; }

        public AccountPickerDialog(ObservableCollection<AccountProfile> accounts)
        {
            InitializeComponent();

            AccountListBox.ItemsSource = accounts;

            if (accounts.Count == 1)
                AccountListBox.SelectedIndex = 0;

            AccountListBox.SelectionChanged += (_, _) =>
            {
                SelectButton.IsEnabled = AccountListBox.SelectedItem is not null;
            };
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAccount = AccountListBox.SelectedItem as AccountProfile;
            DialogResult = true;
            Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
