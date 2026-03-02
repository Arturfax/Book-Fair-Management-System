using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
using System;
using System.Windows;

namespace BookFair.WPF.Views.PublisherView
{
    public partial class UpdatePublisher : Window
    {
        private readonly PublisherController _publisherController;
        private readonly Publisher _original;

        public PublisherDTO Publisher { get; set; }

        public UpdatePublisher(PublisherController publisherController, Publisher publisher)
        {
            InitializeComponent();

            _publisherController = publisherController ?? throw new ArgumentNullException(nameof(publisherController));
            _original = publisher ?? throw new ArgumentNullException(nameof(publisher));

            Publisher = new PublisherDTO(publisher);
            DataContext = this;

            Loaded += (_, __) => RecalcSave();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => RecalcSave();

        private void RecalcSave()
        {
            if (BtnSave == null) return;
            BtnSave.IsEnabled = string.IsNullOrEmpty(Publisher?.IsValid);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Publisher.IsValid))
            {
                MessageBox.Show(Publisher.IsValid, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updated = Publisher.ToPublisher(_original);
            _publisherController.UpdatePublisher(updated);

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
