using BookFair.Core.Controllers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.VisitorView
{
    public partial class VisitorAuthors : Window, INotifyPropertyChanged
    {
        private readonly AuthorController _authorController;
        private readonly BookController _bookController;
        private readonly VisitorController _visitorController;
        private readonly int _visitorId;

        public ObservableCollection<AuthorRow> AllAuthors { get; } = new();
        public ObservableCollection<AuthorRow> FilteredAuthors { get; } = new();

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

        public VisitorAuthors(AuthorController authorController,
                              BookController bookController,
                              VisitorController visitorController,
                              int visitorId,
                              string visitorName)
        {
            InitializeComponent();

            _authorController = authorController ?? throw new System.ArgumentNullException(nameof(authorController));
            _bookController = bookController ?? throw new System.ArgumentNullException(nameof(bookController));
            _visitorController = visitorController ?? throw new System.ArgumentNullException(nameof(visitorController));
            _visitorId = visitorId;

            Title = string.Format(Properties.Resources.VisitorAuthors_WindowTitleFormat, visitorName);
            DataContext = this;

            Loaded += (_, __) => LoadAuthors();
        }

        private void LoadAuthors()
        {
            AllAuthors.Clear();

            // find visitor
            var visitor = _visitorController.GetAllVisitors()?.FirstOrDefault(v => v.Id == _visitorId);
            if (visitor == null)
            {
                MessageBox.Show(Properties.Resources.Msg_VisitorNotFound, Properties.Resources.Msg_ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var wishlistIds = visitor.Wishlist ?? new System.Collections.Generic.List<int>();
            if (!wishlistIds.Any())
            {
                MessageBox.Show(Properties.Resources.Msg_VisitorNoWishlist, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // get all books in wishlist
            var allBooks = _bookController.GetAllBooks();
            var selectedBooks = allBooks?.Where(b => wishlistIds.Contains(b.Id)).ToList() ?? new();
            var authorIds = selectedBooks.SelectMany(b => b.AuthorIds ?? new()).ToHashSet();

            var allAuthors = _authorController.GetAllAuthors();
            if (allAuthors == null) return;

            foreach (var auth in allAuthors.Where(a => authorIds.Contains(a.Id)))
            {
                AllAuthors.Add(new AuthorRow
                {
                    Name = auth.Name,
                    Surname = auth.Surname,
                    Email = auth.Email
                });
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredAuthors.Clear();
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var a in AllAuthors) FilteredAuthors.Add(a);
                return;
            }

            var query = SearchQuery.Trim().ToLower();
            foreach (var a in AllAuthors)
            {
                if (AuthorMatches(a, query)) FilteredAuthors.Add(a);
            }
        }

        private bool AuthorMatches(AuthorRow a, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            var parts = query.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            if (parts.Count == 0) return true;
            string surname = (a.Surname ?? "").ToLower();
            string name = (a.Name ?? "").ToLower();
            if (parts.Count == 1)
            {
                return surname.Contains(parts[0]);
            }
            else
            {
                return surname.Contains(parts[0]) && name.Contains(parts[1]);
            }
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SearchQuery = SearchBox.Text;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchQuery = SearchBox.Text;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class AuthorRow
        {
            public string Name { get; set; } = "";
            public string Surname { get; set; } = "";
            public string Email { get; set; } = "";
        }
    }
}