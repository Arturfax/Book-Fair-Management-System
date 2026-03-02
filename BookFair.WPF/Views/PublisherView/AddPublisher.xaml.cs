using BookFair.Core.Controllers;
using BookFair.WPF.DTO;
using System;
using System.Windows;

namespace BookFair.WPF.Views.PublisherView
{
    public partial class AddPublisher : Window
    {
        private readonly PublisherController _publisherController;

        public PublisherDTO Publisher { get; set; }

        public AddPublisher(PublisherController publisherController)
        {
            InitializeComponent();

            _publisherController = publisherController ?? throw new ArgumentNullException(nameof(publisherController));

            Publisher = new PublisherDTO();
            DataContext = this;

            Loaded += (_, __) => UpdateConfirm();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => UpdateConfirm();

        private void UpdateConfirm()
        {
            if (Confirm == null) return;
            Confirm.IsEnabled = string.IsNullOrEmpty(Publisher?.IsValid);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Publisher.IsValid))
            {
                MessageBox.Show(Publisher.IsValid, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _publisherController.AddPublisher(Publisher.ToPublisher());
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
