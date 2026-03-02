using BookFair.Core.Controllers;
using BookFair.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.VisitorView
{
    public partial class VisitorComparison : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly VisitorController _visitorController;

        public ObservableCollection<Book> AllBooks { get; } = new();

        public ObservableCollection<VisitorRow> AllBoth { get; } = new();
        public ObservableCollection<VisitorRow> FilteredBoth { get; } = new();

        public ObservableCollection<VisitorRow> AllBoughtOnly { get; } = new();
        public ObservableCollection<VisitorRow> FilteredBoughtOnly { get; } = new();

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

        public VisitorComparison(BookController bookController, VisitorController visitorController)
        {
            InitializeComponent();
            _bookController = bookController ?? throw new System.ArgumentNullException(nameof(bookController));
            _visitorController = visitorController ?? throw new System.ArgumentNullException(nameof(visitorController));

            DataContext = this;
            Loaded += (_, __) => LoadBooks();
        }

        private void LoadBooks()
        {
            AllBooks.Clear();
            var books = _bookController.GetAllBooks();
            if (books != null)
            {
                foreach (var b in books)
                {
                    AllBooks.Add(b);
                }
            }
            Book1Combo.ItemsSource = AllBooks;
            Book2Combo.ItemsSource = AllBooks;
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            if (Book1Combo.SelectedItem is not Book b1 || Book2Combo.SelectedItem is not Book b2)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectBothBooks, Properties.Resources.Msg_ValidationTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (b1.Id == b2.Id)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectDifferentBooks, Properties.Resources.Msg_ValidationTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AllBoth.Clear();
            AllBoughtOnly.Clear();

            var visitors = _visitorController.GetAllVisitors() ?? new System.Collections.Generic.List<Visitor>();
            foreach (var v in visitors)
            {
                string card = v.MembershipCardNumber;
                string name = v.Name;
                string surname = v.Surname;
                string status = "Active";

                bool wish1 = v.Wishlist != null && v.Wishlist.Contains(b1.Id);
                bool wish2 = v.Wishlist != null && v.Wishlist.Contains(b2.Id);
                bool bought1 = v.BoughtBooks != null && v.BoughtBooks.Contains(b1.Id);
                bool bought2 = v.BoughtBooks != null && v.BoughtBooks.Contains(b2.Id);

                if (wish1 && wish2)
                {
                    AllBoth.Add(new VisitorRow { CardNumber = card, Name = name, Surname = surname, Status = status });
                }
                if (bought1 && !bought2)
                {
                    AllBoughtOnly.Add(new VisitorRow { CardNumber = card, Name = name, Surname = surname, Status = status });
                }
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredBoth.Clear();
            FilteredBoughtOnly.Clear();
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                foreach (var v in AllBoth) FilteredBoth.Add(v);
                foreach (var v in AllBoughtOnly) FilteredBoughtOnly.Add(v);
                return;
            }
            var q = SearchQuery.Trim().ToLower();
            var parts = q.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            foreach (var v in AllBoth)
                if (VisitorMatches(v, parts)) FilteredBoth.Add(v);
            foreach (var v in AllBoughtOnly)
                if (VisitorMatches(v, parts)) FilteredBoughtOnly.Add(v);
        }

        private bool VisitorMatches(VisitorRow v, System.Collections.Generic.List<string> parts)
        {
            if (parts.Count == 0) return true;
            string surname = (v.Surname ?? "").ToLower();
            string name = (v.Name ?? "").ToLower();
            string card = (v.CardNumber ?? "").ToLower();
            if (parts.Count == 1)
            {
                return surname.Contains(parts[0]);
            }
            else if (parts.Count == 2)
            {
                return surname.Contains(parts[0]) && name.Contains(parts[1]);
            }
            else
            {
                return card.Contains(parts[0]) && name.Contains(parts[1]) && surname.Contains(parts[2]);
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

        public class VisitorRow
        {
            public string CardNumber { get; set; } = "";
            public string Name { get; set; } = "";
            public string Surname { get; set; } = "";
            public string Status { get; set; } = "";
        }
    }
}