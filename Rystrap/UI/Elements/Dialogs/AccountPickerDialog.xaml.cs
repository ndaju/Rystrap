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

        public AccountPickerDialog(ObservableCollection<AccountProfile> accounts, string? selectedAccountId = null)
        {
            InitializeComponent();

            AccountListBox.ItemsSource = accounts;

            if (!string.IsNullOrEmpty(selectedAccountId))
                AccountListBox.SelectedItem = accounts.FirstOrDefault(x => x.Id == selectedAccountId);

            if (AccountListBox.SelectedItem is null)
                AccountListBox.SelectedItem = accounts.FirstOrDefault(x => x.IsFavorite);

            if (AccountListBox.SelectedItem is null && accounts.Count == 1)
                AccountListBox.SelectedIndex = 0;

            SelectButton.IsEnabled = AccountListBox.SelectedItem is not null;

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
