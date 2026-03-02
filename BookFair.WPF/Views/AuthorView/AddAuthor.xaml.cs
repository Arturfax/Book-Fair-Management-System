using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
using System;
using System.Windows;

namespace BookFair.WPF.Views.AuthorView
{
    public partial class AddAuthor : Window
    {
        private readonly AuthorController _authorController;
        private readonly AddressController _addressController;

        public AuthorDTO Author { get; set; }
        public AddressDTO Address { get; set; }

        public AddAuthor(AuthorController authorController, AddressController addressController)
        {
            InitializeComponent();

            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _addressController = addressController ?? throw new ArgumentNullException(nameof(addressController));

            Author = new AuthorDTO();
            Address = new AddressDTO();

            DataContext = this;

            Loaded += (_, __) => UpdateConfirm();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => UpdateConfirm();

        private void UpdateConfirm()
        {
            if (Confirm == null) return;

            bool authorOk = string.IsNullOrEmpty(Author?.IsValid);
            bool addressOk = string.IsNullOrEmpty(Address?.IsValid);

            Confirm.IsEnabled = authorOk && addressOk;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Author.IsValid) || !string.IsNullOrEmpty(Address.IsValid))
            {
                var aErr = string.IsNullOrEmpty(Author.IsValid) ? "" : Author.IsValid + "\n";
                var addrErr = string.IsNullOrEmpty(Address.IsValid) ? "" : Address.IsValid;
                MessageBox.Show(aErr + addrErr, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addr = _addressController.AddAddress(Address.ToAddress());
            var author = Author.ToAuthor();
            author.Address = addr;

            _authorController.AddAuthor(author);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
