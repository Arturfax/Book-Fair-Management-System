using System.Collections.Generic;
using System.Windows;

namespace BookFair.WPF.Views.VisitorView
{
    public partial class AddWishlistBookDialog : Window
    {
        public List<WishlistPickItem> SelectedItems { get; private set; } = new();

        public AddWishlistBookDialog(List<WishlistPickItem> items)
        {
            InitializeComponent();
            LstBooks.ItemsSource = items;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectedItems = new List<WishlistPickItem>();
            foreach (var item in LstBooks.SelectedItems)
            {
                if (item is WishlistPickItem picked)
                    SelectedItems.Add(picked);
            }

            if (SelectedItems.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectBookFromList, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DialogResult = true;
        }
    }

    public class WishlistPickItem
    {
        public int BookId { get; set; }
        public string Display { get; set; } = "";
    }
}
