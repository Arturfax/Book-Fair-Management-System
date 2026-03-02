using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
using BookFair.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BookFair.WPF.Views.BookView
{
    public partial class UpdateBook : Window, INotifyPropertyChanged
    {
        private readonly BookController _bookController;
        private readonly AuthorController _authorController;
        private readonly PublisherController _publisherController;

        public BookDTO Book { get; set; }
        public ObservableCollection<AuthorDisplayItem> AuthorDropdownList { get; } = new();
        public ObservableCollection<GenreDisplayItem> GenreDropdownList { get; } = new();

        public UpdateBook(BookController bookController, AuthorController authorController, PublisherController publisherController, Book model)
        {
            InitializeComponent();

            _bookController = bookController ?? throw new ArgumentNullException(nameof(bookController));
            _authorController = authorController ?? throw new ArgumentNullException(nameof(authorController));
            _publisherController = publisherController ?? throw new ArgumentNullException(nameof(publisherController));

            Book = new BookDTO(model);
            DataContext = this;

            Loaded += (_, __) =>
            {
                LoadAuthors();
                SyncAuthorButtons();
                LoadGenres();
                RecalcSave();
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

            // BookDTO kod tebe inicijalno popunjava Authors kao "1,2,3" (ID-evi).
            // Za 8.24 u formi treba prikaz "Ime Prezime", zato mapiramo prvi AuthorId.
            if (Book.AuthorIds != null && Book.AuthorIds.Count > 0)
            {
                var firstAuthorId = Book.AuthorIds[0];
                var found = AuthorDropdownList.FirstOrDefault(a => a.Id == firstAuthorId);
                Book.Authors = found != null ? found.DisplayName : string.Empty;
            }
            else
            {
                Book.Authors = string.Empty;
            }
        }

        // 8.24: + aktivno samo ako knjiga nema autora, - aktivno samo ako ima autora
        private void SyncAuthorButtons()
        {
            bool hasAuthor = Book.AuthorIds != null && Book.AuthorIds.Count > 0;
            BtnAddAuthor.IsEnabled = !hasAuthor;
            BtnRemoveAuthor.IsEnabled = hasAuthor;
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            // dodatna za�tita (iako je dugme disabled kad ima autora)
            if (Book.AuthorIds != null && Book.AuthorIds.Count > 0)
                return;

            var dlg = new ChooseAuthorDialog(
                AuthorDropdownList.Select(a => new ChooseAuthorDialog.AuthorListItem
                {
                    Id = a.Id,
                    DisplayName = a.DisplayName
                }).ToList()
            )
            { Owner = this }; // centriranje "Izaberi autora" u odnosu na "Izmeni knjigu"

            bool? ok = dlg.ShowDialog();
            if (ok != true) return;

            Book.AuthorIds = new List<int> { dlg.SelectedAuthorId };
            Book.Authors = dlg.SelectedAuthorDisplayName;

            var publishers = _publisherController.GetAllPublishers();
            var matched = publishers?.FirstOrDefault(p => p.AuthorIds != null && p.AuthorIds.Contains(dlg.SelectedAuthorId));
            Book.Publisher = matched?.Name ?? string.Empty;

            SyncAuthorButtons();
            RecalcSave();
        }
        private void RemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            // ako nema autora, nema �ta da uklanjamo
            if (Book.AuthorIds == null || Book.AuthorIds.Count == 0)
                return;

            // 8.25: potvrda (modalno) + centriranje u odnosu na UpdateBook
            var dlg = new RemoveAuthorConfirmDialog
            {
                Owner = this
            };

            if (dlg.ShowDialog() != true)
                return; // korisnik odustao

            // ukloni autora
            Book.AuthorIds = new List<int>();
            Book.Authors = string.Empty;

            SyncAuthorButtons();
            RecalcSave();
        }
        private void LoadGenres()
        {
            GenreDropdownList.Clear();
            var genres = GenreLocalizer.GetLocalizedGenres();
            foreach (var genre in genres)
            {
                GenreDropdownList.Add(genre);
            }
            GenreComboBox.ItemsSource = GenreDropdownList;

            // Set selected genre from Book.Genre
            if (!string.IsNullOrEmpty(Book?.Genre))
            {
                GenreComboBox.SelectedItem = GenreDropdownList.FirstOrDefault(g => g.Value == Book.Genre);
            }
        }

        private void OnAuthorSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // ako nema autora, nema �ta da uklanjamo
            if (Book.AuthorIds == null || Book.AuthorIds.Count == 0)
                return;

            // 8.25: modalna potvrda, centrirana u odnosu na UpdateBook
            var dlg = new RemoveAuthorConfirmDialog
            {
                Owner = this
            };

            if (dlg.ShowDialog() != true)
                return; // korisnik odustao

            // ukloni autora
            Book.AuthorIds = new List<int>();
            Book.Authors = string.Empty;

            SyncAuthorButtons();
            RecalcSave();
        }

        private void OnGenreSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is GenreDisplayItem genreItem)
            {
                Book.Genre = genreItem.Value;
            }
            RecalcSave();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => RecalcSave();

        private void RecalcSave()
        {
            if (BtnSave == null) return;
            var bookOk = string.IsNullOrEmpty(Book?.IsValid);
            BtnSave.IsEnabled = bookOk;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var bErr = Book?.IsValid ?? "";
            if (!string.IsNullOrEmpty(bErr))
            {
                MessageBox.Show(bErr, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // zadr�avamo isto pona�anje koje si imao: autor je obavezan
            if (Book.AuthorIds == null || Book.AuthorIds.Count == 0)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectAuthorHeads, Properties.Resources.Msg_ValidationErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            

            // Get selected genre from ComboBox
            if (GenreComboBox.SelectedItem is GenreDisplayItem genreItem)
            {
                Book.Genre = genreItem.Value;
            }

            var updated = Book.ToBook();
            _bookController.UpdateBook(updated);

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class AuthorDisplayItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
        }
    }
}
