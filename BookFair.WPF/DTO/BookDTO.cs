using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookFair.WPF.DTO
{
    public class BookDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _isbn = string.Empty;
        public string ISBN
        {
            get => _isbn;
            set { if (_isbn != value) { _isbn = value ?? string.Empty; OnPropertyChanged(nameof(ISBN)); } }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(nameof(Name)); } }
        }

        private string _genre = string.Empty;
        public string Genre
        {
            get => _genre;
            set { if (_genre != value) { _genre = value ?? string.Empty; OnPropertyChanged(nameof(Genre)); } }
        }

        private int _yearOfRelease = DateTime.Now.Year;
        public int YearOfRelease
        {
            get => _yearOfRelease;
            set { if (_yearOfRelease != value) { _yearOfRelease = value; OnPropertyChanged(nameof(YearOfRelease)); } }
        }

        private string _yearOfReleaseText = DateTime.Now.Year.ToString();
        public string YearOfReleaseText
        {
            get => _yearOfReleaseText;
            set
            {
                if (_yearOfReleaseText != value)
                {
                    _yearOfReleaseText = value ?? string.Empty;
                    if (int.TryParse(value, out int year))
                    {
                        _yearOfRelease = year;
                    }
                    OnPropertyChanged(nameof(YearOfReleaseText));
                }
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { if (_price != value) { _price = value; OnPropertyChanged(nameof(Price)); } }
        }

        private string _priceText = "0";
        public string PriceText
        {
            get => _priceText;
            set
            {
                if (_priceText != value)
                {
                    _priceText = value ?? string.Empty;
                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    {
                        _price = price;
                    }
                    OnPropertyChanged(nameof(PriceText));
                }
            }
        }

        private int _numberOfPages;
        public int NumberOfPages
        {
            get => _numberOfPages;
            set { if (_numberOfPages != value) { _numberOfPages = value; OnPropertyChanged(nameof(NumberOfPages)); } }
        }

        private string _numberOfPagesText = "0";
        public string NumberOfPagesText
        {
            get => _numberOfPagesText;
            set
            {
                if (_numberOfPagesText != value)
                {
                    _numberOfPagesText = value ?? string.Empty;
                    if (int.TryParse(value, out int pages))
                    {
                        _numberOfPages = pages;
                    }
                    OnPropertyChanged(nameof(NumberOfPagesText));
                }
            }
        }

        private string _publisher = string.Empty;
        public string Publisher
        {
            get => _publisher;
            set { if (_publisher != value) { _publisher = value ?? string.Empty; OnPropertyChanged(nameof(Publisher)); } }
        }

        private string _authors = string.Empty;
        public string Authors
        {
            get => _authors;
            set
            {
                if (_authors != value)
                {
                    _authors = value ?? string.Empty;
                    OnPropertyChanged(nameof(Authors));
                }
            }
        }

        public List<int> AuthorIds { get; set; } = new List<int>();
        public List<int> BuyerIds { get; set; } = new List<int>();
        public List<int> WishlistVisitorIds { get; set; } = new List<int>();

        public BookDTO() { }

        public BookDTO(Book book)
        {
            if (book == null) return;

            Id = book.Id;
            ISBN = book.ISBN ?? string.Empty;
            Name = book.Name ?? string.Empty;
            Genre = book.Genre ?? string.Empty;
            YearOfRelease = book.YearOfRelease;
            YearOfReleaseText = book.YearOfRelease.ToString();
            Price = book.Price;
            PriceText = book.Price.ToString(CultureInfo.InvariantCulture);
            NumberOfPages = book.NumberOfPages;
            NumberOfPagesText = book.NumberOfPages.ToString();
            Publisher = book.Publisher ?? string.Empty;
            AuthorIds = book.AuthorIds ?? new List<int>();
            Authors = AuthorIds.Count > 0 ? string.Join(", ", AuthorIds) : string.Empty;
            BuyerIds = book.BuyerIds ?? new List<int>();
            WishlistVisitorIds = book.WishlistVisitorIds ?? new List<int>();
        }

        public Book ToBook()
        {
            return new Book
            {
                Id = Id,
                ISBN = ISBN,
                Name = Name,
                Genre = Genre,
                YearOfRelease = YearOfRelease,
                Price = Price,
                NumberOfPages = NumberOfPages,
                Publisher = Publisher,
                AuthorIds = AuthorIds ?? new List<int>(),
                BuyerIds = BuyerIds ?? new List<int>(),
                WishlistVisitorIds = WishlistVisitorIds ?? new List<int>()
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static readonly Regex _isbnRegex = new(@"^(?:\d{10}|\d{13}|[\d-]{10,17})$");

        public string Error => string.Empty;

        public string this[string columnName] =>
            columnName switch
            {
                nameof(ISBN) => string.IsNullOrWhiteSpace(ISBN)
                    ? "ISBN is required."
                    : string.Empty,
                nameof(Name) => string.IsNullOrWhiteSpace(Name)
                    ? "Name is required."
                    : string.Empty,
                nameof(Genre) => string.IsNullOrWhiteSpace(Genre)
                    ? "Genre is required."
                    : string.Empty,
                nameof(YearOfReleaseText) => !int.TryParse(YearOfReleaseText, out int year) || year < 1000 || year > DateTime.Now.Year + 1
                    ? "Enter a valid year."
                    : string.Empty,
                nameof(PriceText) => !decimal.TryParse(PriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0
                    ? "Enter a valid price."
                    : string.Empty,
                nameof(NumberOfPagesText) => !int.TryParse(NumberOfPagesText, out int pages) || pages <= 0
                    ? "Enter a valid number of pages."
                    : string.Empty,
                nameof(Authors) => string.IsNullOrWhiteSpace(Authors)
                    ? "At least one author is required (Name Surname)."
                    : string.Empty,
                _ => string.Empty
            };

        public string IsValid =>
            new[] { nameof(ISBN), nameof(Name), nameof(Genre), nameof(YearOfReleaseText), nameof(PriceText), nameof(NumberOfPagesText), nameof(Authors) }
                .Select(p => this[p])
                .FirstOrDefault(e => !string.IsNullOrEmpty(e)) ?? string.Empty;
    }
}
