using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BookFair.WPF.Views.BookView
{
    public partial class ChooseAuthorDialog : Window
    {
        public class AuthorListItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
        }

        public int SelectedAuthorId { get; private set; }
        public string SelectedAuthorDisplayName { get; private set; } = "";

        public ChooseAuthorDialog(List<AuthorListItem> authors)
        {
            InitializeComponent();
            AuthorsList.ItemsSource = authors ?? new List<AuthorListItem>();
        }

        private void AuthorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnConfirm.IsEnabled = AuthorsList.SelectedItem != null;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsList.SelectedItem is not AuthorListItem a)
                return;

            SelectedAuthorId = a.Id;
            SelectedAuthorDisplayName = a.DisplayName;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
