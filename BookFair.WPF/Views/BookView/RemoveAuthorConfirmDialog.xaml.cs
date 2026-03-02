using System.Windows;

namespace BookFair.WPF.Views.BookView
{
    public partial class RemoveAuthorConfirmDialog : Window
    {
        public RemoveAuthorConfirmDialog()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
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
