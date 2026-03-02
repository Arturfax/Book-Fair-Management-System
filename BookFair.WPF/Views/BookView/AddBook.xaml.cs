using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.Core.Models.Enums;
using BookFair.WPF.DTO;
using BookFair.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.BookView
{
    public partial class AddBook : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly AuthorController _authorController;
        private readonly PublisherController _publisherController;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BookDTO Book { get; set; }
        public ObservableCollection<AuthorDisplayItem> AuthorDropdownList { get; } = new();
        public ObservableCollection<GenreDisplayItem> GenreDropdownList { get; } = new();

        public AddBook(BookController bookController, AuthorController authorController, PublisherController publisherController)
        {
            InitializeComponent();

            _bookController = bookController ?? throw new ArgumentNullException(nameof(bookController));
            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _publisherController = publisherController ?? throw new ArgumentNullException(nameof(publisherController));

            Book = new BookDTO();

            DataContext = this;

            Loaded += (_, __) =>
            {
                LoadAuthors();
                LoadGenres();
                UpdateConfirm();
            };
        }

        private void LoadAuthors()
        {
            AuthorDropdownList.Clear();
            var authors = _authorController.GetAllAuthors();
            if (authors != null)
            {
                foreach (var author in authors)
                {
                    AuthorDropdownList.Add(new AuthorDisplayItem
                    {
                        Id = author.Id,
                        DisplayName = $"{author.Name} {author.Surname}"
                    });
                }
            }
            AuthorComboBox.ItemsSource = AuthorDropdownList;
        }

        private void LoadGenres()
        {
            GenreDropdownList.Clear();
            var genres = GenreLocalizer.GetLocalizedGenres();
            foreach (var genre in genres)
            {
                GenreDropdownList.Add(genre);
            }
        }

        private void OnAuthorSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AuthorComboBox.SelectedItem is AuthorDisplayItem author)
            {
                Book.AuthorIds = new List<int> { author.Id };
                Book.Authors = author.DisplayName;

                var publishers = _publisherController.GetAllPublishers();
                var matched = publishers?.FirstOrDefault(p => p.AuthorIds != null && p.AuthorIds.Contains(author.Id));
                Book.Publisher = matched?.Name ?? string.Empty;
            }
            UpdateConfirm();
        }

        private void OnGenreSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is GenreDisplayItem genreItem)
            {
                Book.Genre = genreItem.Value;
            }
            UpdateConfirm();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => UpdateConfirm();

        private void UpdateConfirm()
        {
            if (Confirm == null) return;

            bool bookOk = string.IsNullOrEmpty(Book?.IsValid);

            Confirm.IsEnabled = bookOk;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Book.IsValid))
            {
                MessageBox.Show(Book.IsValid, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected author from ComboBox
            var authorIds = new List<int>();
            if (AuthorComboBox.SelectedItem is AuthorDisplayItem author)
            {
                authorIds.Add(author.Id);
            }

            if (authorIds.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectAuthorHeads, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book.AuthorIds = authorIds;
            var book = Book.ToBook();

            _bookController.AddBook(book);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected virtual void OnPropertyChanged(string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class AuthorDisplayItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
        }
    }
}
