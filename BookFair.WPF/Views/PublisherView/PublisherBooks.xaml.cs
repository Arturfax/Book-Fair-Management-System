using BookFair.Core.Controllers;
using BookFair.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.PublisherView
{
    public partial class PublisherBooks : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly int _publisherId;
        private readonly string _publisherName;

        public ObservableCollection<Book> AllBooks { get; } = new();
        public ObservableCollection<Book> FilteredBooks { get; } = new();

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value ?? string.Empty;
                    ApplyFilter();
                }
            }
        }

        public PublisherBooks(BookController bookController, int publisherId, string publisherName)
        {
            InitializeComponent();
            _bookController = bookController ?? throw new System.ArgumentNullException(nameof(bookController));
            _publisherId = publisherId;
            _publisherName = publisherName ?? string.Empty;

            Title = string.Format(Properties.Resources.PublisherBooks_WindowTitle, _publisherName);
            DataContext = this;
            Loaded += (_, __) => LoadBooks();
        }

        private void LoadBooks()
        {
            AllBooks.Clear();
            var books = _bookController.GetAllBooks() ?? new System.Collections.Generic.List<Book>();
            // filter by publisher name, ignoring case
            var matching = books.Where(b => (b.Publisher ?? "").Equals(_publisherName, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (!matching.Any())
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.PublisherBooks_NoBooksMessage, _publisherName),
                    Properties.Resources.PublisherBooks_WindowDefaultTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            foreach (var b in matching)
                AllBooks.Add(b);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredBooks.Clear();
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var b in AllBooks)
                    FilteredBooks.Add(b);
                return;
            }

            var q = SearchQuery.Trim().ToLowerInvariant();
            foreach (var b in AllBooks)
            {
                if ((b.ISBN ?? "").ToLowerInvariant().Contains(q) ||
                    (b.Name ?? "").ToLowerInvariant().Contains(q) ||
                    b.Price.ToString().ToLowerInvariant().Contains(q) ||
                    b.YearOfRelease.ToString().ToLowerInvariant().Contains(q) ||
                    (b.Genre ?? "").ToLowerInvariant().Contains(q))
                {
                    FilteredBooks.Add(b);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SearchQuery = SearchBox.Text;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}