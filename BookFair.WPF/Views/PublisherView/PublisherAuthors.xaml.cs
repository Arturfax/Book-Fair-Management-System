using BookFair.Core.Controllers;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Windows;

namespace BookFair.WPF.Views.PublisherView
{
    public partial class PublisherAuthors : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly AuthorController _authorController;
        private readonly string _publisherName;

        public ObservableCollection<Author> AllAuthors { get; } = new();
        public ObservableCollection<Author> FilteredAuthors { get; } = new();

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

        public PublisherAuthors(BookController bookController, AuthorController authorController, string publisherName)
        {
            InitializeComponent();

            _bookController = bookController ?? throw new ArgumentNullException(nameof(bookController));
            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _publisherName = publisherName ?? string.Empty;

            Title = string.Format(Properties.Resources.PublisherAuthors_WindowTitleFormat, _publisherName);
            DataContext = this;

            Loaded += (_, __) => LoadAuthors();
        }

        private void LoadAuthors()
        {
            AllAuthors.Clear();

            var books = _bookController.GetAllBooks() ?? new List<Book>();

            // Knjige izabranog izdavača (kod vas je Book.Publisher string)
            var publisherBooks = books
                .Where(b => (b.Publisher ?? "")
                    .Equals(_publisherName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            // Iz svih knjiga tog izdavača uzmi AuthorIds (unikatno)
            var authorIds = publisherBooks
                .Where(b => b.AuthorIds != null)
                .SelectMany(b => b.AuthorIds!)
                .Distinct()
                .ToList();

            var authors = _authorController.GetAllAuthors() ?? new List<Author>();

            var matchingAuthors = authors
                .Where(a => authorIds.Contains(a.Id))
                .ToList();

            foreach (var a in matchingAuthors)
                AllAuthors.Add(a);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredAuthors.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var a in AllAuthors)
                    FilteredAuthors.Add(a);
                return;
            }

            var q = SearchQuery.Trim().ToLowerInvariant();

            foreach (var a in AllAuthors)
            {
                var addressText = a.Address?.ToString() ?? "";
                var dobText = a.DateOfBirth.ToString("d");

                if ((a.Name ?? "").ToLowerInvariant().Contains(q) ||
                    (a.Surname ?? "").ToLowerInvariant().Contains(q) ||
                    (a.Email ?? "").ToLowerInvariant().Contains(q) ||
                    (a.Phone ?? "").ToLowerInvariant().Contains(q) ||
                    addressText.ToLowerInvariant().Contains(q) ||
                    dobText.ToLowerInvariant().Contains(q))
                {
                    FilteredAuthors.Add(a);
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
