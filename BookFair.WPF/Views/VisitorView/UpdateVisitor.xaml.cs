using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BookFair.WPF.Views.VisitorView;

namespace BookFair.WPF.Views.VisitorView
{
    public partial class UpdateVisitor : Window, INotifyPropertyChanged
    {
        private readonly VisitorController _visitorController;
        private readonly AddressController _addressController;
        private readonly BookController _bookController;
        private readonly AuthorController _authorController;
        private readonly BuyingController _buyingController;
        private readonly Visitor _originalVisitor;

        public VisitorDTO Visitor { get; set; }
        public AddressDTO Address { get; set; }

        public ObservableCollection<PurchasedBookRow> PurchasedBooks { get; } = new();

        public ObservableCollection<WishlistBookRow> WishlistBooks { get; } = new();

        private WishlistBookRow? _selectedWishlistBook;
        public WishlistBookRow? SelectedWishlistBook
        {
            get => _selectedWishlistBook;
            set { _selectedWishlistBook = value; OnPropertyChanged(); }
        }

        private PurchasedBookRow? _selectedPurchase;
        public PurchasedBookRow? SelectedPurchase
        {
            get => _selectedPurchase;
            set { _selectedPurchase = value; OnPropertyChanged(); }
        }

        private string _averageRatingText = string.Format(Properties.Resources.Stat_AverageRating, 0);
        public string AverageRatingText
        {
            get => _averageRatingText;
            set { _averageRatingText = value; OnPropertyChanged(); }
        }

        private string _totalSpentText = string.Format(Properties.Resources.Stat_TotalSpent, 0);
        public string TotalSpentText
        {
            get => _totalSpentText;
            set { _totalSpentText = value; OnPropertyChanged(); }
        }
        public UpdateVisitor(
            VisitorController visitorController,
            AddressController addressController,
            BookController bookController,
            AuthorController authorController,
            BuyingController buyingController,
            Visitor model)
        {
            InitializeComponent();

            _visitorController = visitorController ?? throw new ArgumentNullException(nameof(visitorController));
            _addressController = addressController ?? throw new ArgumentNullException(nameof(addressController));
            _bookController = bookController ?? throw new ArgumentNullException(nameof(bookController));
            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _buyingController = buyingController ?? throw new ArgumentNullException(nameof(buyingController));

            _originalVisitor = model ?? throw new ArgumentNullException(nameof(model));


            Visitor = new VisitorDTO(model);
            Address = new AddressDTO(model.Address ?? new Address());

            DataContext = this;

            Loaded += (_, __) =>
            {
                RecalcSave();
                LoadPurchasedBooks();
                LoadWishlistBooks();

            };

        }


        private void WishlistDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = WishlistBooksGrid.SelectedItems;
            if (selected == null || selected.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectBookInTable, Properties.Resources.Msg_InfoTitle,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // potvrda pre brisanja
            var result = MessageBox.Show(
                string.Format(Properties.Resources.Msg_WishlistConfirmDelete, selected.Count),
                Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            _originalVisitor.Wishlist ??= new System.Collections.Generic.List<int>();
            foreach (var item in selected)
            {
                if (item is WishlistBookRow row)
                    _originalVisitor.Wishlist.Remove(row.BookId);
            }

            _visitorController.UpdateVisitor(_originalVisitor);
            LoadWishlistBooks();
        }

        private void WishlistPurchase_Click(object sender, RoutedEventArgs e)
        {
            
                var selected = WishlistBooksGrid.SelectedItems;
                if (selected == null || selected.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.Msg_WishlistSelectBook, Properties.Resources.Msg_InfoTitle,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 8.18: Kupovina je za jednu izabranu knjigu
                if (selected.Count != 1 || selected[0] is not WishlistBookRow row)
                {
                    MessageBox.Show(Properties.Resources.Msg_SelectBookInTable, Properties.Resources.Msg_InfoTitle,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Dijalog za unos kupovine (centiran u odnosu na ovaj prozor)
                var dlg = new PurchaseBookDialog(row.ISBN, row.Name)
                {
                    Owner = this
                };

                bool? ok = dlg.ShowDialog();
                if (ok != true)
                    return;

                int rating = dlg.Rating;
                DateTime buyingDate = dlg.BuyingDate;

                // 1) Upis u buyings.csv
                var buying = new Buying
                {
                    VisitorId = _originalVisitor.Id,
                    BookId = row.BookId,
                    BuyingDate = buyingDate,
                    Rating = rating,
                    Comment = string.Empty
                };
                _buyingController.AddBuying(buying);

                // 2) Prebacivanje: iz Wishlist -> u BoughtBooks
                _originalVisitor.Wishlist ??= new System.Collections.Generic.List<int>();
                _originalVisitor.BoughtBooks ??= new System.Collections.Generic.List<int>();

                _originalVisitor.Wishlist.Remove(row.BookId);
                if (!_originalVisitor.BoughtBooks.Contains(row.BookId))
                    _originalVisitor.BoughtBooks.Add(row.BookId);

                _visitorController.UpdateVisitor(_originalVisitor);

                // 3) Dodaj kupca u knjigu (BuyerIds)
                var book = _bookController.GetBookById(row.BookId);
                if (book != null)
                {
                    book.BuyerIds ??= new System.Collections.Generic.List<int>();
                    if (!book.BuyerIds.Contains(_originalVisitor.Id))
                        book.BuyerIds.Add(_originalVisitor.Id);

                    _bookController.UpdateBook(book);
                }

                // 4) Osveži tabele (Želje + Kupljene)
                LoadWishlistBooks();
                LoadPurchasedBooks();
            

        }

        private void ShowAuthors_Click(object sender, RoutedEventArgs e)
        {
            var visitorName = $"{Visitor.Name} {Visitor.Surname}";
            var win = new VisitorAuthors(_authorController, _bookController, _visitorController, _originalVisitor.Id, visitorName);
            win.Owner = this;
            win.ShowDialog();
        }

        private void LoadWishlistBooks()
        {
            WishlistBooks.Clear();

            if (_originalVisitor.Wishlist == null || _originalVisitor.Wishlist.Count == 0)
                return;

            foreach (int bookId in _originalVisitor.Wishlist)
            {
                var book = _bookController.GetBookById(bookId);
                if (book == null) continue;

                string author = "";
                if (book.AuthorIds != null && book.AuthorIds.Count > 0)
                {
                    author = string.Join(", ",
                        book.AuthorIds.Select(id =>
                        {
                            var a = _authorController.GetAuthorById(id);
                            return a != null ? $"{a.Name} {a.Surname}" : $"#{id}";
                        }));
                }

                WishlistBooks.Add(new WishlistBookRow
                {
                    BookId = book.Id,
                    ISBN = book.ISBN,
                    Name = book.Name,
                    Author = author,
                    Publisher = book.Publisher,
                    Price = book.Price
                });
            }
        }

        private void WishlistAdd_Click(object sender, RoutedEventArgs e)
        {
            // 1) Učitaj sve knjige
            var allBooks = _bookController.GetAllBooks();

            // 2) Wishlist / BoughtBooks mogu biti null
            _originalVisitor.Wishlist ??= new System.Collections.Generic.List<int>();
            _originalVisitor.BoughtBooks ??= new System.Collections.Generic.List<int>();

            // 3) Prikaži samo knjige koje nisu kupljene i nisu već na wishlist
            var eligible = allBooks
                .Where(b => !_originalVisitor.BoughtBooks.Contains(b.Id) &&
                            !_originalVisitor.Wishlist.Contains(b.Id))
                .Select(b => new WishlistPickItem
                {
                    BookId = b.Id,
                    Display = $"{b.ISBN} - {b.Name}"
                })
                .ToList();

            if (eligible.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_NoAvailableWishlistBooks,
                    Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 4) Otvori dijalog (modalno, centriran na UpdateVisitor)
            var dlg = new AddWishlistBookDialog(eligible);
            dlg.Owner = this;

            bool? ok = dlg.ShowDialog();
            if (ok != true || dlg.SelectedItems.Count == 0)
                return;

            // 5) Dodaj sve izabrane knjige u wishlist (bez duplikata)
            foreach (var selectedItem in dlg.SelectedItems)
            {
                if (!_originalVisitor.Wishlist.Contains(selectedItem.BookId))
                    _originalVisitor.Wishlist.Add(selectedItem.BookId);
            }

            // 6) Snimi posetioca i osveži tabelu
            _visitorController.UpdateVisitor(_originalVisitor);
            LoadWishlistBooks();
        }




        private void LoadPurchasedBooks()
        {
            PurchasedBooks.Clear();

            var buyings = _buyingController.GetBuyingsByVisitor(_originalVisitor.Id);

            foreach (var b in buyings.OrderByDescending(x => x.BuyingDate))
            {
                var book = _bookController.GetBookById(b.BookId);
                if (book == null) continue;

                PurchasedBooks.Add(new PurchasedBookRow
                {
                    BuyingId = b.Id,
                    BookId = b.BookId,
                    ISBN = book.ISBN,
                    Name = book.Name,
                    Date = b.BuyingDate,
                    Rating = b.Rating,
                    Price = book.Price
                });
            }

            RecalcStats();
        }

        private void RecalcStats()
        {
            if (PurchasedBooks.Count == 0)
            {
                AverageRatingText = string.Format(Properties.Resources.Stat_AverageRating, 0);
                TotalSpentText = string.Format(Properties.Resources.Stat_TotalSpent, 0);
                return;
            }

            var avg = PurchasedBooks.Average(x => x.Rating);
            var total = PurchasedBooks.Sum(x => x.Price);

            AverageRatingText = string.Format(Properties.Resources.Stat_AverageRating, avg);
            TotalSpentText = string.Format(Properties.Resources.Stat_TotalSpent, total);
        }



        private void CancelPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPurchase == null)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectPurchase, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                Properties.Resources.Msg_ConfirmCancelPurchase,
                Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            // 1) Obrisi kupovinu iz buyings.csv
            _buyingController.RemoveBuyingById(SelectedPurchase.BuyingId);

            // 2) Ukloni jednu instancu knjige iz kupljenih (BoughtBooks)
            _originalVisitor.BoughtBooks.Remove(SelectedPurchase.BookId);

            // 8.14: prebaci knjigu u listu želja (Wishlist) (bez duplikata)
            _originalVisitor.Wishlist ??= new System.Collections.Generic.List<int>();
            if (!_originalVisitor.Wishlist.Contains(SelectedPurchase.BookId))
                _originalVisitor.Wishlist.Add(SelectedPurchase.BookId);

            // 3) Ukloni visitorId iz knjige (BuyerIds) i snimi knjigu
            var book = _bookController.GetBookById(SelectedPurchase.BookId);
            if (book != null)
            {
                book.BuyerIds.Remove(_originalVisitor.Id);
                _bookController.UpdateBook(book);
            }

            // 4) Snimi posetioca (sa izmenjenim BoughtBooks i Wishlist)
            _visitorController.UpdateVisitor(_originalVisitor);

            // 5) Refresh (da nestane iz tabele kupljenih)
            LoadPurchasedBooks();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => RecalcSave();

        // ComboBox.SelectionChanged
        private void OnAnyTextChanged(object sender, SelectionChangedEventArgs e) => RecalcSave();

        private void RecalcSave()
        {
            if (BtnSave == null) return;
            var visitorOk = string.IsNullOrEmpty(Visitor?.IsValid);
            var addressOk = string.IsNullOrEmpty(Address?.IsValid);
            BtnSave.IsEnabled = visitorOk && addressOk;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vErr = Visitor?.IsValid ?? "";
            var aErr = Address?.IsValid ?? "";
            if (!string.IsNullOrEmpty(vErr) || !string.IsNullOrEmpty(aErr))
            {
                MessageBox.Show($"{vErr}\n{aErr}", Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updated = Visitor.ToVisitor();
            var addr = Address.ToAddress();
            addr.Id = Address.Id;

            var savedAddr = _addressController.UpdateAddress(addr);
            updated.Address = savedAddr;

            updated.BoughtBooks = _originalVisitor.BoughtBooks;
            updated.Wishlist = _originalVisitor.Wishlist;
            updated.AverageRating = _originalVisitor.AverageRating;


            _visitorController.UpdateVisitor(updated);

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public class WishlistBookRow
        {
            public int BookId { get; set; }
            public string ISBN { get; set; } = "";
            public string Name { get; set; } = "";
            public string Author { get; set; } = "";
            public string Publisher { get; set; } = "";
            public decimal Price { get; set; }
        }



        public class PurchasedBookRow
        {
            public int BuyingId { get; set; }
            public int BookId { get; set; }
            public string ISBN { get; set; } = "";
            public string Name { get; set; } = "";
            public DateTime Date { get; set; }
            public int Rating { get; set; }
            public decimal Price { get; set; }
        }
    }
}
