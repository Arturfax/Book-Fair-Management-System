using BookFair.Core.Controllers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.AuthorView
{
    public partial class AuthorVisitors : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly VisitorController _visitorController;
        private readonly int _authorId;

        public ObservableCollection<VisitorRow> AllVisitors { get; } = new();
        public ObservableCollection<VisitorRow> FilteredVisitors { get; } = new();

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

        public AuthorVisitors(BookController bookController, VisitorController visitorController, int authorId, string authorName)
        {
            InitializeComponent();

            _bookController = bookController ?? throw new System.ArgumentNullException(nameof(bookController));
            _visitorController = visitorController ?? throw new System.ArgumentNullException(nameof(visitorController));
            _authorId = authorId;

            Title = string.Format(Properties.Resources.AuthorVisitors_WindowTitleFormat, authorName);
            DataContext = this;

            Loaded += (_, __) => LoadVisitors();
        }

        private void LoadVisitors()
        {
            AllVisitors.Clear();

            // Get all books by this author
            var allBooks = _bookController.GetAllBooks();
            var authorBooks = allBooks?.Where(b => b.AuthorIds != null && b.AuthorIds.Contains(_authorId)).ToList() ?? new();

            if (!authorBooks.Any())
            {
                MessageBox.Show(Properties.Resources.Msg_NoBooksAuthor, Properties.Resources.Msg_InfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var bookIds = authorBooks.Select(b => b.Id).ToHashSet();

            // Get all visitors
            var allVisitors = _visitorController.GetAllVisitors();
            if (allVisitors == null) return;

            // Find visitors who have at least one book by this author in their wishlist
            var visitorsWithAuthorBooks = new HashSet<int>();
            foreach (var visitor in allVisitors)
            {
                if (visitor.Wishlist != null && visitor.Wishlist.Any(bookId => bookIds.Contains(bookId)))
                {
                    visitorsWithAuthorBooks.Add(visitor.Id);
                }
            }

            // Populate display list (each visitor appears only once)
            foreach (var visitor in allVisitors.Where(v => visitorsWithAuthorBooks.Contains(v.Id)))
            {
                string address = "";
                if (visitor.Address != null)
                {
                    address = $"{visitor.Address.Street} {visitor.Address.Number}, {visitor.Address.City}";
                }

                AllVisitors.Add(new VisitorRow
                {
                    VisitorId = visitor.Id,
                    CardNumber = visitor.MembershipCardNumber,
                    Name = visitor.Name,
                    Surname = visitor.Surname,
                    Status = "Active",
                    Address = address
                });
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredVisitors.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var visitor in AllVisitors)
                    FilteredVisitors.Add(visitor);
                return;
            }

            var query = SearchQuery.Trim().ToLower();
            var parts = query.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => p.Trim())
                             .ToList();

            foreach (var visitor in AllVisitors)
            {
                if (VisitorMatches(visitor, parts))
                    FilteredVisitors.Add(visitor);
            }
        }

        private bool VisitorMatches(VisitorRow visitor, System.Collections.Generic.List<string> parts)
        {
            // Apply same matching rules as main visitor search (spec 8.19)
            if (parts.Count == 0) return true;

            var surname = visitor.Surname?.ToLower() ?? "";
            var name = visitor.Name?.ToLower() ?? "";
            var card = visitor.CardNumber?.ToLower() ?? "";

            if (parts.Count == 1)
            {
                // One word: match surname
                return surname.Contains(parts[0]);
            }
            else if (parts.Count == 2)
            {
                // Two words: first in surname, second in name
                return surname.Contains(parts[0]) && name.Contains(parts[1]);
            }
            else if (parts.Count >= 3)
            {
                // Three words: first in card, second in name, third in surname
                return card.Contains(parts[0]) && name.Contains(parts[1]) && surname.Contains(parts[2]);
            }

            return false;
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

        public class VisitorRow
        {
            public int VisitorId { get; set; }
            public string CardNumber { get; set; } = "";
            public string Name { get; set; } = "";
            public string Surname { get; set; } = "";
            public string Status { get; set; } = "";
            public string Address { get; set; } = "";
        }
    }
}
