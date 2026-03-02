using System;
using System.Windows;
using System.Windows.Controls;

namespace BookFair.WPF.Views.VisitorView
{
    public partial class PurchaseBookDialog : Window
    {
        public string ISBN { get; }
        public string BookName { get; }

        public int Rating { get; private set; }
        public DateTime BuyingDate { get; private set; }

        public PurchaseBookDialog(string isbn, string name)
        {
            InitializeComponent();

            ISBN = isbn;
            BookName = name;

            DataContext = this;

            // Default datum: danas
            BuyingDatePicker.SelectedDate = DateTime.Today;

            RecalcConfirm();
        }

        private void OnInputChanged(object sender, SelectionChangedEventArgs e) => RecalcConfirm();
        private void OnDateChanged(object sender, SelectionChangedEventArgs e) => RecalcConfirm();

        private void RecalcConfirm()
        {
            bool ratingOk = RatingCombo.SelectedItem is ComboBoxItem;
            bool dateOk = BuyingDatePicker.SelectedDate.HasValue;

            BtnConfirm.IsEnabled = ratingOk && dateOk;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (RatingCombo.SelectedItem is not ComboBoxItem ratingItem ||
                !int.TryParse(ratingItem.Content?.ToString(), out int rating) ||
                rating < 5 || rating > 10)
            {
                MessageBox.Show(
                    Properties.Resources.Msg_RatingRange,
                    Properties.Resources.Msg_ValidationTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!BuyingDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    Properties.Resources.Msg_BuyingDateRequired,
                    Properties.Resources.Msg_ValidationTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Rating = rating;
            BuyingDate = BuyingDatePicker.SelectedDate.Value;

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
