using BookFair.Core.Models.Enums;
using BookFair.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Models
{
    public class Book : ObservableModel, Serializable
    {
        private int _id;
        private string _isbn;
        private string _name;
        private string _genre;
        private int _yearOfRelease;
        private decimal _price;
        private int _numberOfPages;
        private List<int> _authorIds;
        private string _publisher;
        private List<int> _buyerIds;
        private List<int> _wishlistVisitorIds;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string ISBN
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public int YearOfRelease
        {
            get => _yearOfRelease;
            set => SetProperty(ref _yearOfRelease, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public int NumberOfPages
        {
            get => _numberOfPages;
            set => SetProperty(ref _numberOfPages, value);
        }

        public List<int> AuthorIds
        {
            get => _authorIds;
            set => SetProperty(ref _authorIds, value);
        }

        public string Publisher
        {
            get => _publisher;
            set => SetProperty(ref _publisher, value);
        }

        public List<int> BuyerIds
        {
            get => _buyerIds;
            set => SetProperty(ref _buyerIds, value);
        }

        public List<int> WishlistVisitorIds
        {
            get => _wishlistVisitorIds;
            set => SetProperty(ref _wishlistVisitorIds, value);
        }

        public Book()
        {
        }

        public Book(string isbn, string name, string genre, int yearOfRelease, decimal price, int numberOfPages, List<int> authorIds, string publisher, List<int> buyerIds, List<int> wishlistVisitorIds)
        {
            ISBN = isbn;
            Name = name;
            Genre = genre;
            YearOfRelease = yearOfRelease;
            Price = price;
            NumberOfPages = numberOfPages;
            AuthorIds = authorIds;
            Publisher = publisher;
            BuyerIds = buyerIds;
            WishlistVisitorIds = wishlistVisitorIds;
        }

        public override string ToString()
        {
            string AuthorsPrint = AuthorIds.Count > 0 ? string.Join(", ", AuthorIds) : "nema autora";
            string BuyersPrint = BuyerIds.Count > 0 ? string.Join(", ", BuyerIds) : "nema kupaca knjige";
            string VisitorsWishlistPrint = WishlistVisitorIds.Count > 0 ? string.Join(", ", WishlistVisitorIds) : "nema posetilaca sa ovom knjigom na listi želja";

            return $"{ISBN}| {Name} | {Genre} | Datum Izdavanja: {YearOfRelease} | Cena: {Price} | Broj Stranica: {NumberOfPages} | Autori knjige: {AuthorsPrint} | Izdavac: {Publisher}| Kupci: {BuyersPrint}| Posetioci sa knjigom na listi zelja: {VisitorsWishlistPrint}";
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ISBN,
                Name,
                Genre,
                YearOfRelease.ToString(),
                Price.ToString(),
                NumberOfPages.ToString(),
                string.Join(";", AuthorIds),
                Publisher,
                string.Join(";", BuyerIds),
                string.Join(";", WishlistVisitorIds)
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ISBN = values[1];
            Name = values[2];
            Genre = values[3];
            YearOfRelease = int.Parse(values[4]);
            Price = decimal.Parse(values[5]);
            NumberOfPages = int.Parse(values[6]);
            AuthorIds = string.IsNullOrEmpty(values[7]) ? new List<int>() : values[7].Split(';').Select(int.Parse).ToList();
            Publisher = values[8];
            BuyerIds = string.IsNullOrEmpty(values[9]) ? new List<int>() : values[9].Split(';').Select(int.Parse).ToList();
            WishlistVisitorIds = string.IsNullOrEmpty(values[10]) ? new List<int>() : values[10].Split(';').Select(int.Parse).ToList();
        }


    }
}
