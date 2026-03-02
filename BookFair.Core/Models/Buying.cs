using BookFair.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Models
{
    public class Buying : ObservableModel, Serializable
    {
        private int _id;
        private int _visitorId;
        private int _bookId;
        private DateTime _buyingDate;
        private int _rating;
        private string _comment;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int VisitorId
        {
            get => _visitorId;
            set => SetProperty(ref _visitorId, value);
        }

        public int BookId
        {
            get => _bookId;
            set => SetProperty(ref _bookId, value);
        }

        public DateTime BuyingDate
        {
            get => _buyingDate;
            set => SetProperty(ref _buyingDate, value);
        }

        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public Buying() { }

        public Buying(int visitorId, int bookId, DateTime buyingDate, int rating, string comment)
        {
            VisitorId = visitorId;
            BookId = bookId;
            BuyingDate = buyingDate;
            Rating = rating;
            Comment = comment;
        }

        public override string ToString()
        {
            return $"Posetilac sa id {VisitorId} kupio/la knjigu sa id {BookId} dana {BuyingDate:dd.MM.yyyy} | Ocena: {Rating}/5 | Komentar: {Comment}";
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                VisitorId.ToString(),
                BookId.ToString(),
                BuyingDate.ToString("yyyy-MM-dd"),
                Rating.ToString(),
                Comment
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            VisitorId = int.Parse(values[1]);
            BookId = int.Parse(values[2]);
            BuyingDate = DateTime.Parse(values[3]);
            Rating = int.Parse(values[4]);
            Comment = values[5];
        }
    }
}

