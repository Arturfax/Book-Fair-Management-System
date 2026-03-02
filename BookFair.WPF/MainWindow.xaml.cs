using BookFair.Core.Controllers;
using BookFair.Core.DAO;
using BookFair.Core.Models;
using BookFair.Core.Services;
using BookFair.WPF.Helpers;
using BookFair.WPF.Views.AuthorView;
using BookFair.WPF.Views.BookView;
using BookFair.WPF.Views.PublisherView;
using BookFair.WPF.Views.VisitorView;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BookFair.WPF
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // --- Keyboard Shortcut Commands ---
        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        // --- Controllers ---

        private readonly VisitorController _visitorController;
        private readonly BookController _bookController;
        private readonly AuthorController _authorController;
        private readonly PublisherController _publisherController;
        private readonly AddressController _addressController;
        private readonly BuyingController _buyingController;


        //    --- Observable Data (for grids) ---
        public ObservableCollection<Visitor> Visitors { get; } = new();
        public ObservableCollection<Book> Books { get; } = new();
        public ObservableCollection<Author> Authors { get; } = new();
        public ObservableCollection<Publisher> Publishers { get; } = new();

        // --- Selection ---
        public Visitor? SelectedVisitor { get; set; }
        public Book? SelectedBook { get; set; }
        public Author? SelectedAuthor { get; set; }
        public Publisher? SelectedPublisher { get; set; }

        // --- Status bar clock (minute refresh) ---
        private readonly DispatcherTimer _clock = new();
        private const string AppName = "BOOK-FAIR";

        // --- Pagination State and Cache ---
        private const int PageSize = 16;
        private int VisitorsPage = 1, BooksPage = 1, AuthorsPage = 1, PublishersPage = 1;
        private int VisitorsTotalPages = 1, BooksTotalPages = 1, AuthorsTotalPages = 1, PublishersTotalPages = 1;

        private System.Collections.Generic.List<Visitor> _visitorsBacking = new();
        private System.Collections.Generic.List<Book> _booksBacking = new();
        private System.Collections.Generic.List<Author> _authorsBacking = new();
        private System.Collections.Generic.List<Publisher> _publishersBacking = new();

        private string _visitorsSearch = "", _booksSearch = "", _authorsSearch = "", _publishersSearch = "";
        private string _visitorsSort = "Index", _booksSort = "Name", _authorsSort = "id", _publishersSort = "Name";
        private bool _visitorsDesc = false, _booksDesc = false, _authorsDesc = false, _publishersDesc = false;

        public MainWindow()
        {
            // Initialize Commands for keyboard shortcuts
            NewCommand = new RelayCommand(() => OnNewClick(this, new RoutedEventArgs()));
            SaveCommand = new RelayCommand(() => OnSaveClick(this, new RoutedEventArgs()));
            EditCommand = new RelayCommand(() => OnEditClick(this, new RoutedEventArgs()));
            DeleteCommand = new RelayCommand(() => OnDeleteClick(this, new RoutedEventArgs()));

            InitializeComponent();

            // Initialize DAOs
            var visitorDAO = new VisitorDAO();
            var bookDAO = new BookDAO();
            var authorDAO = new AuthorDAO();
            var publisherDAO = new PublisherDAO();
            var addressDAO = new AddressDAO();
            var buyingDAO = new BuyingDAO();

            // Initialize Services
            var visitorService = new VisitorService(visitorDAO, bookDAO);
            var bookService = new BookService(bookDAO);
            var authorService = new AuthorService(authorDAO);
            var publisherService = new PublisherService(publisherDAO);
            var addressService = new AddressService(addressDAO);
            var buyingService = new BuyingService(buyingDAO, visitorDAO);

            // Initialize Controllers
            _visitorController = new VisitorController(visitorService);
            _bookController = new BookController(bookService);
            _authorController = new AuthorController(authorService);
            _publisherController = new PublisherController(publisherService);
            _addressController = new AddressController(addressService);
            _buyingController = new BuyingController(buyingService);
            Width = SystemParameters.PrimaryScreenWidth * 0.75;
            Height = SystemParameters.PrimaryScreenHeight * 0.75;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;

            // Status bar: initial fill + refresh every minute
            UpdateClock();
            _clock.Interval = TimeSpan.FromMinutes(1);
            _clock.Tick += (_, __) => UpdateClock();
            _clock.Start();

            // Left text: application name + active tab
            MainTabs.SelectionChanged += OnTabChanged;
            UpdateStatusAppAndTab();

            LoadAll();
        }

        protected override void OnClosed(EventArgs e)
        {
            _clock.Stop();
            base.OnClosed(e);
        }

        // --------------- LOAD and REFRESH---------------
        private void LoadAll()
        {
            _visitorsBacking = _visitorController.ViewAllVisitors();
            _booksBacking = _bookController.ViewAllBooks();
            _authorsBacking = _authorController.ViewAllAuthors();
            _publishersBacking = _publisherController.GetAllPublishers();

            // Populate head author names for publishers
            PopulateHeadAuthorNames();

            _visitorsSearch = _booksSearch = _authorsSearch = _publishersSearch = "";
            _visitorsSort = "MembershipCardNumber"; _visitorsDesc = false; VisitorsPage = 1;
            _booksSort = "Name"; _booksDesc = false; BooksPage = 1;
            _authorsSort = "Id"; _authorsDesc = false; AuthorsPage = 1;
            _publishersSort = "Name"; _publishersDesc = false; PublishersPage = 1;

            UpdateVisitorsPage();
            UpdateBooksPage();
            UpdateAuthorsPage();
            UpdatePublishersPage();
        }

        /// <summary>
        /// Populates the HeadAuthorName for each publisher based on the HeadOfPublisherId
        /// and sets up observers to automatically update when HeadOfPublisherId changes
        /// </summary>
        private void PopulateHeadAuthorNames()
        {
            foreach (var publisher in _publishersBacking)
            {
                UpdatePublisherHeadAuthorName(publisher);

                // Subscribe to HeadOfPublisherId changes using the Observer pattern
                publisher.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Publisher.HeadOfPublisherId) && s is Publisher pub)
                    {
                        UpdatePublisherHeadAuthorName(pub);
                    }
                };
            }
        }

        /// <summary>
        /// Updates the HeadAuthorName for a specific publisher based on its HeadOfPublisherId
        /// </summary>
        private void UpdatePublisherHeadAuthorName(Publisher publisher)
        {
            if (publisher.HeadOfPublisherId > 0)
            {
                var headAuthor = _authorsBacking.FirstOrDefault(a => a.Id == publisher.HeadOfPublisherId);
                if (headAuthor != null)
                {
                    publisher.HeadAuthorName = $"{headAuthor.Name} {headAuthor.Surname}";
                }
                else
                {
                    publisher.HeadAuthorName = "Unknown";
                }
            }
            else
            {
                publisher.HeadAuthorName = string.Empty;
            }
        }

        // --- Paging methods for each tab ---
        private void UpdateVisitorsPage()
        {
            // apply complex matching rules from spec 8.19 rather than simple substring search
            var filtered = _visitorsBacking.Where(v => VisitorMatchesQuery(v, _visitorsSearch));
            var sorted = _visitorsDesc
                ? filtered.OrderByDescending(v => GetComparable(v, _visitorsSort))
                : filtered.OrderBy(v => GetComparable(v, _visitorsSort));
            var total = sorted.Count();
            VisitorsTotalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            VisitorsPage = Math.Min(VisitorsPage, VisitorsTotalPages);
            var pageItems = sorted.Skip((VisitorsPage - 1) * PageSize).Take(PageSize).ToList();
            Visitors.Clear();
            foreach (var v in pageItems) Visitors.Add(v);

            if (VisitorsPageInfo != null)
                VisitorsPageInfo.Text = $"{VisitorsPage} / {VisitorsTotalPages}";
        }

        // perform visitor matching according to specification 8.19
        private bool VisitorMatchesQuery(Visitor v, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            // split by commas, trim spaces, ignore empty pieces
            var parts = query.Split(',')
                             .Select(p => p.Trim())
                             .Where(p => p.Length > 0)
                             .ToList();

            if (parts.Count == 0)
                return true;

            string surname = (v.Surname ?? "").ToLower();
            string name = (v.Name ?? "").ToLower();
            string card = (v.MembershipCardNumber?.ToString() ?? "").ToLower();

            if (parts.Count == 1)
            {
                // only surname must contain the term
                return surname.Contains(parts[0]);
            }
            else if (parts.Count == 2)
            {
                // first in surname, second in name
                return surname.Contains(parts[0]) && name.Contains(parts[1]);
            }
            else // 3 or more
            {
                // first in membership card, second in name, third in surname
                return card.Contains(parts[0]) && name.Contains(parts[1]) && surname.Contains(parts[2]);
            }
        }

        // apply simpler matching rules for authors per spec 8.19
        private bool AuthorMatchesQuery(Author a, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            var parts = query.Split(',')
                             .Select(p => p.Trim())
                             .Where(p => p.Length > 0)
                             .ToList();

            if (parts.Count == 0)
                return true;

            string surname = (a.Surname ?? "").ToLower();
            string name = (a.Name ?? "").ToLower();

            if (parts.Count == 1)
            {
                return surname.Contains(parts[0]);
            }
            else // 2 or more
            {
                return surname.Contains(parts[0]) && name.Contains(parts[1]);
            }
        }

        private void UpdateBooksPage()
        {
            var filtered = _booksBacking.Where(b =>
                _booksSearch == "" ||
                (b.ISBN ?? "").ToLower().Contains(_booksSearch) ||
                (b.Name ?? "").ToLower().Contains(_booksSearch) ||
                ((b.Price).ToString() ?? "").ToLower().Contains(_booksSearch) ||
                ((b.YearOfRelease).ToString() ?? "").ToLower().Contains(_booksSearch) ||
                (b.Genre ?? "").ToLower().Contains(_booksSearch) ||
                Helpers.GenreLocalizer.GetLocalizedGenreName(b.Genre ?? "").ToLower().Contains(_booksSearch));
            var sorted = _booksDesc
                ? filtered.OrderByDescending(b => GetComparable(b, _booksSort))
                : filtered.OrderBy(b => GetComparable(b, _booksSort));
            var total = sorted.Count();
            BooksTotalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            BooksPage = Math.Min(BooksPage, BooksTotalPages);
            var pageItems = sorted.Skip((BooksPage - 1) * PageSize).Take(PageSize).ToList();
            Books.Clear();
            foreach (var b in pageItems) Books.Add(b);

            if (BooksPageInfo != null)
                BooksPageInfo.Text = $"{BooksPage} / {BooksTotalPages}";
        }

        private void UpdateAuthorsPage()
        {
            // filter according to spec rules for authors (one or two words)
            var filtered = _authorsBacking.Where(a => AuthorMatchesQuery(a, _authorsSearch));
            var sorted = _authorsDesc
                ? filtered.OrderByDescending(a => GetComparable(a, _authorsSort))
                : filtered.OrderBy(a => GetComparable(a, _authorsSort));
            var total = sorted.Count();
            AuthorsTotalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            AuthorsPage = Math.Min(AuthorsPage, AuthorsTotalPages);
            var pageItems = sorted.Skip((AuthorsPage - 1) * PageSize).Take(PageSize).ToList();
            Authors.Clear();
            foreach (var a in pageItems) Authors.Add(a);

            if (AuthorsPageInfo != null)
                AuthorsPageInfo.Text = $"{AuthorsPage} / {AuthorsTotalPages}";
        }

        private void UpdatePublishersPage()
        {
            var filtered = _publishersBacking.Where(p =>
                _publishersSearch == "" ||
                (p.Code ?? "").ToLower().Contains(_publishersSearch) ||
                (p.Name ?? "").ToLower().Contains(_publishersSearch));
            var sorted = _publishersDesc
                ? filtered.OrderByDescending(p => GetComparable(p, _publishersSort))
                : filtered.OrderBy(p => GetComparable(p, _publishersSort));
            var total = sorted.Count();
            PublishersTotalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            PublishersPage = Math.Min(PublishersPage, PublishersTotalPages);
            var pageItems = sorted.Skip((PublishersPage - 1) * PageSize).Take(PageSize).ToList();
            Publishers.Clear();
            foreach (var p in pageItems) Publishers.Add(p);

            if (PublishersPageInfo != null)
                PublishersPageInfo.Text = $"{PublishersPage} / {PublishersTotalPages}";
        }

        //// --- Paging controls handlers ---
        private void OnNextPageClick(object sender, RoutedEventArgs e)
        {
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            switch (tab)
            {
                case "TabVisitors":
                    if (VisitorsPage < VisitorsTotalPages) { VisitorsPage++; UpdateVisitorsPage(); }
                    break;
                case "TabBooks":
                    if (BooksPage < BooksTotalPages) { BooksPage++; UpdateBooksPage(); }
                    break;
                case "TabAuthors":
                    if (AuthorsPage < AuthorsTotalPages) { AuthorsPage++; UpdateAuthorsPage(); }
                    break;
                case "TabPublishers":
                    if (PublishersPage < PublishersTotalPages) { PublishersPage++; UpdatePublishersPage(); }
                    break;
            }
        }
        private void OnPrevPageClick(object sender, RoutedEventArgs e)
        {
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            switch (tab)
            {
                case "TabVisitors":
                    if (VisitorsPage > 1) { VisitorsPage--; UpdateVisitorsPage(); }
                    break;
                case "TabBooks":
                    if (BooksPage > 1) { BooksPage--; UpdateBooksPage(); }
                    break;
                case "TabAuthors":
                    if (AuthorsPage > 1) { AuthorsPage--; UpdateAuthorsPage(); }
                    break;
                case "TabPublishers":
                    if (PublishersPage > 1) { PublishersPage--; UpdatePublishersPage(); }
                    break;
            }
        }

        //// ------------ Search handler ----------
        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            var q = (SearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            if (string.IsNullOrWhiteSpace(q)) q = "";

            switch (tab)
            {
                case "TabVisitors":
                    // save raw lower-case query for visitor matching rules in UpdateVisitorsPage
                    _visitorsSearch = q; VisitorsPage = 1; UpdateVisitorsPage(); break;
                case "TabBooks":
                    _booksSearch = q; BooksPage = 1; UpdateBooksPage(); break;
                case "TabAuthors":
                    _authorsSearch = q; AuthorsPage = 1; UpdateAuthorsPage(); break;
                case "TabPublishers":
                    _publishersSearch = q; PublishersPage = 1; UpdatePublishersPage(); break;
            }
        }

        //// ---------------- Sorting handler -------------
        private void OnGridSorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            var sortMember = e.Column.SortMemberPath ?? e.Column.Header?.ToString() ?? "";
            var descending = e.Column.SortDirection == ListSortDirection.Ascending;
            e.Column.SortDirection = descending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            if (sender == GridVisitors)
            {
                _visitorsSort = sortMember; _visitorsDesc = descending; VisitorsPage = 1; UpdateVisitorsPage();
            }
            else if (sender == GridBooks)
            {
                _booksSort = sortMember; _booksDesc = descending; BooksPage = 1; UpdateBooksPage();
            }
            else if (sender == GridAuthors)
            {
                _authorsSort = sortMember; _authorsDesc = descending; AuthorsPage = 1; UpdateAuthorsPage();
            }
            else if (sender == GridPublishers)
            {
                _publishersSort = sortMember; _publishersDesc = descending; PublishersPage = 1; UpdatePublishersPage();
            }
        }



        // ----------- Tab change -> status ----------
        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) UpdateStatusAppAndTab();
        }

        private void UpdateStatusAppAndTab()
        {
            if (MainTabs.SelectedItem is TabItem tab)
                StatusAppAndTab.Text = $"{AppName} - {tab.Header}";
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            Date.Content = now.ToString("dd.MM.yyyy");
            Time.Content = now.ToString("HH:mm");
        }

        // ------------- OPEN/SAVE/NEW/EDIT/DELETE -------------
        private void OnOpenVisitors(object sender, RoutedEventArgs e) => MainTabs.SelectedItem = TabVisitors;
        private void OnOpenBooks(object sender, RoutedEventArgs e) => MainTabs.SelectedItem = TabBooks;
        private void OnOpenAuthors(object sender, RoutedEventArgs e) => MainTabs.SelectedItem = TabAuthors;
        private void OnOpenPublishers(object sender, RoutedEventArgs e) => MainTabs.SelectedItem = TabPublishers;

        private void OnSetPublisherHead(object sender, RoutedEventArgs e)
        {
            var win = new BookFair.WPF.Views.PublisherView.PublisherHeads(_publisherController, _authorController)
            {
                Owner = this
            };
            win.ShowDialog();

            LoadAll();
        }

        private void OnCompareVisitors(object sender, RoutedEventArgs e)
        {
            var win = new BookFair.WPF.Views.VisitorView.VisitorComparison(_bookController, _visitorController)
            {
                Owner = this
            };
            win.ShowDialog();
        }

        private void OnShowPublisherBooks(object sender, RoutedEventArgs e)
        {
            if (SelectedPublisher == null)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectPublisher);
                return;
            }
            var win = new BookFair.WPF.Views.PublisherView.PublisherBooks(
                _bookController,
                SelectedPublisher.Id,
                SelectedPublisher.Name ?? string.Empty)
            {
                Owner = this
            };
            win.ShowDialog();
        }

        private void OnShowPublisherAuthors(object sender, RoutedEventArgs e)
        {
            if (SelectedPublisher == null)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectPublisher);
                return;
            }

            var win = new BookFair.WPF.Views.PublisherView.PublisherAuthors(
                _bookController,
                _authorController,
                SelectedPublisher.Name ?? string.Empty)
            {
                Owner = this // centriranje na glavni prozor
            };

            win.ShowDialog();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.Msg_StateSaved, Properties.Resources.Msg_SavedTitle,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnCloseClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void OnNewClick(object sender, RoutedEventArgs e)
        {
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            if (tab == nameof(TabVisitors))
            {
                var win = new AddVisitor(_visitorController, _addressController);
                if (win.ShowDialog() == true) LoadAll();
            }
            else if (tab == nameof(TabBooks))
            {
                var win = new AddBook(_bookController, _authorController, _publisherController);
                if (win.ShowDialog() == true) LoadAll();
            }
            else if (tab == nameof(TabAuthors))
            {
                var win = new AddAuthor(_authorController, _addressController);
                if (win.ShowDialog() == true) LoadAll();
            }
            else if (tab == nameof(TabPublishers))
            {
                var win = new AddPublisher(_publisherController);
                if (win.ShowDialog() == true) LoadAll();
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            if (tab == nameof(TabVisitors))
            {
                if (SelectedVisitor == null) { MessageBox.Show(Properties.Resources.Msg_SelectVisitor); return; }
                var win = new UpdateVisitor(
                    _visitorController,
                    _addressController,
                    _bookController,
                    _authorController,
                    _buyingController,
                    SelectedVisitor);
                if (win.ShowDialog() == true) LoadAll();
            }
            else if (tab == nameof(TabBooks))
            {
                if (SelectedBook == null) { MessageBox.Show(Properties.Resources.Msg_SelectBook); return; }
                var win = new UpdateBook(_bookController, _authorController, _publisherController, SelectedBook);
                win.Owner = this; 
                if (win.ShowDialog() == true)
                    LoadAll();
            }
            else if (tab == nameof(TabAuthors))
            {
                if (SelectedAuthor == null) { MessageBox.Show(Properties.Resources.Msg_SelectAuthor); return; }
                var win = new UpdateAuthor(_authorController, _addressController, _bookController, _visitorController, SelectedAuthor) { Owner = this };
                if (win.ShowDialog() == true) LoadAll();
            }
            else if (tab == nameof(TabPublishers))
            {
                if (SelectedPublisher == null) { MessageBox.Show(Properties.Resources.Msg_SelectPublisher); return; }
                var win = new UpdatePublisher(_publisherController, SelectedPublisher) { Owner = this };
                if (win.ShowDialog() == true) LoadAll();
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            var tab = (MainTabs.SelectedItem as TabItem)?.Name;
            if (tab == nameof(TabVisitors))
            {
                if (SelectedVisitor == null) { MessageBox.Show(Properties.Resources.Msg_SelectVisitor); return; }
                if (MessageBox.Show(Properties.Resources.Msg_ConfirmDelete, Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var addrId = SelectedVisitor.Address?.Id;
                    _visitorController.DeleteVisitor(SelectedVisitor.Id);
                    if (addrId.HasValue) _addressController.DeleteAddress(addrId.Value);
                    LoadAll();
                }
            }
            else if (tab == nameof(TabBooks))
            {
                if (SelectedBook == null) { MessageBox.Show(Properties.Resources.Msg_SelectBook); return; }
                if (MessageBox.Show(Properties.Resources.Msg_ConfirmDelete, Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _bookController.DeleteBook(SelectedBook.Id);
                    LoadAll();
                }
            }
            else if (tab == nameof(TabAuthors))
            {
                if (SelectedAuthor == null) { MessageBox.Show(Properties.Resources.Msg_SelectAuthor); return; }
                if (MessageBox.Show(Properties.Resources.Msg_ConfirmDelete, Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _authorController.DeleteAuthor(SelectedAuthor.Id);
                    LoadAll();
                }
            }
            else if (tab == nameof(TabPublishers))
            {
                if (SelectedPublisher == null) { MessageBox.Show(Properties.Resources.Msg_SelectPublisher); return; }
                if (MessageBox.Show(Properties.Resources.Msg_ConfirmDelete, Properties.Resources.Msg_ConfirmDeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _publisherController.DeletePublisher(SelectedPublisher.Id);
                    LoadAll();
                }
            }
        }

        private IComparable? GetComparable(object obj, string property)
        {
            if (obj is Visitor v && (property == "MembershipCardNumber" || property == "membershipCardNumber"))
            {
                var text = v.MembershipCardNumber?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(text)) return Tuple.Create("", 0, 0);
                var parts = text.Split('|');
                if (parts.Length != 3) return Tuple.Create(text.ToLowerInvariant(), 0, 0);
                var prefix = parts[0].ToLowerInvariant();
                _ = int.TryParse(parts[1], out var num);
                _ = int.TryParse(parts[2], out var year);
                return Tuple.Create(prefix, num, year);
            }
            var prop = obj.GetType().GetProperty(property);
            var val = prop?.GetValue(obj);
            if (val is null) return string.Empty;
            if (val is IComparable cmp) return cmp;
            return val.ToString();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Book-Fair — WPF\nVersion: 1.0\n\nAuthors:\n- Luka Artukov | Birth Year: 2005. | Sremska Mitrovica\n- Stefan Vujic | Birth Year: 2004. | Sremska Mitrovica",
                "About Application",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                OnDeleteClick(sender, e);
                e.Handled = true;
            }
        }

        // (optional) language change remains, without status bar changes
        private void SetLanguage_En(object sender, RoutedEventArgs e) => SetLanguage("en");
        private void SetLanguage_Sr(object sender, RoutedEventArgs e) => SetLanguage("sr");

        public void SetLanguage(string cultureCode)
        {
            var culture = new System.Globalization.CultureInfo(cultureCode);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            Properties.Resources.Culture = culture;
            GenreLocalizer.RefreshResourceManager();
            new MainWindow().Show();
            Close();
        }
    }
}

