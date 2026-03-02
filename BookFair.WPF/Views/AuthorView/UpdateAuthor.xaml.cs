using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
using BookFair.WPF.Views.VisitorView; // for AddWishlistBookDialog and WishlistPickItem
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BookFair.WPF.Views.AuthorView
{
    public partial class UpdateAuthor : Window, INotifyPropertyChanged
    {
        private readonly AuthorController _authorController;
        private readonly AddressController _addressController;
        private readonly BookController _bookController;
        private readonly VisitorController _visitorController;

        public AuthorDTO Author { get; set; }
        public AddressDTO Address { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<AuthorBookRow> AuthorBooks { get; } = new();
        public int BooksTotalCount => AuthorBooks.Count;

        private AuthorBookRow? _selectedAuthorBook;
        public AuthorBookRow? SelectedAuthorBook
        {
            get => _selectedAuthorBook;
            set { _selectedAuthorBook = value; OnPropertyChanged(); }
        }

        public UpdateAuthor(AuthorController authorController, AddressController addressController, BookController bookController, VisitorController visitorController, Author model)
        {
            InitializeComponent();

            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _addressController = addressController ?? throw new ArgumentNullException(nameof(addressController));
            _bookController = bookController ?? throw new ArgumentNullException(nameof(bookController));
            _visitorController = visitorController ?? throw new ArgumentNullException(nameof(visitorController));

            Author = new AuthorDTO(model);
            Address = new AddressDTO(model.Address ?? new Address());

            DataContext = this;

            Loaded += (_, __) =>
            {
                RecalcSave();
                LoadAuthorBooks();
            };
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => RecalcSave();

        private void RecalcSave()
        {
            if (BtnSave == null) return;
            var authorOk = string.IsNullOrEmpty(Author?.IsValid);
            var addressOk = string.IsNullOrEmpty(Address?.IsValid);
            BtnSave.IsEnabled = authorOk && addressOk;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var aErr = Author?.IsValid ?? "";
            var addrErr = Address?.IsValid ?? "";
            if (!string.IsNullOrEmpty(aErr) || !string.IsNullOrEmpty(addrErr))
            {
                MessageBox.Show($"{aErr}\n{addrErr}", Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updated = Author.ToAuthor();
            var addr = Address.ToAddress();
            addr.Id = Address.Id;

            var savedAddr = _addressController.UpdateAddress(addr);
            updated.Address = savedAddr;

            _authorController.UpdateAuthor(updated);

            DialogResult = true;
            Close();
        }

        private void RemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var selected = AuthorBooksGrid.SelectedItems;
            if (selected == null || selected.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectBookInTable, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var res = MessageBox.Show(string.Format(Properties.Resources.Msg_ConfirmRemoveAuthorBooks, selected.Count), Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            foreach (var item in selected)
            {
                if (item is AuthorBookRow row)
                {
                    var book = _bookController.GetBookById(row.BookId);
                    if (book != null && book.AuthorIds != null)
                    {
                        book.AuthorIds.Remove(Author.Id);
                        _bookController.UpdateBook(book);
                    }
                }
            }
            LoadAuthorBooks();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void ShowVisitors_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AuthorVisitors(_bookController, _visitorController, Author.Id, $"{Author.Name} {Author.Surname}")
            {
                Owner = this
            };
            dlg.ShowDialog();
        }

        private void LoadAuthorBooks()
        {
            AuthorBooks.Clear();
            var all = _bookController.GetAllBooks();
            if (all == null) { OnPropertyChanged(nameof(BooksTotalCount)); return; }
            foreach (var b in all)
            {
                if (b.AuthorIds != null && b.AuthorIds.Contains(Author.Id))
                {
                    AuthorBooks.Add(new AuthorBookRow
                    {
                        BookId = b.Id,
                        ISBN = b.ISBN,
                        Name = b.Name,
                        YearOfRelease = b.YearOfRelease,
                        Genre = b.Genre
                    });
                }
            }
            OnPropertyChanged(nameof(BooksTotalCount));
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            // prepare list of books not yet authored by current author
            var all = _bookController.GetAllBooks();
            var items = new System.Collections.Generic.List<WishlistPickItem>();
            foreach (var b in all)
            {
                if (b.AuthorIds == null || !b.AuthorIds.Contains(Author.Id))
                {
                    items.Add(new WishlistPickItem { BookId = b.Id, Display = $"{b.ISBN} - {b.Name}" });
                }
            }
            if (items.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_NoAvailableBooksToAdd, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new AddWishlistBookDialog(items);
            dlg.Owner = this;
            bool? ok = dlg.ShowDialog();
            if (ok != true || dlg.SelectedItems.Count == 0) return;

            foreach (var selectedItem in dlg.SelectedItems)
            {
                int bookId = selectedItem.BookId;
                var book = _bookController.GetBookById(bookId);
                if (book != null)
                {
                    book.AuthorIds ??= new System.Collections.Generic.List<int>();
                    if (!book.AuthorIds.Contains(Author.Id))
                        book.AuthorIds.Add(Author.Id);
                    _bookController.UpdateBook(book);
                }
            }
            LoadAuthorBooks();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class AuthorBookRow
        {
            public int BookId { get; set; }
            public string ISBN { get; set; } = "";
            public string Name { get; set; } = "";
            public int YearOfRelease { get; set; }
            public string Genre { get; set; } = "";
        }
    }
}
