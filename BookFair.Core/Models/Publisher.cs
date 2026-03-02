using BookFair.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookFair.Core.Models
{
    public class Publisher : ObservableModel, Serializable
    {
        private int _id;
        private string _code;
        private string _name;
        private int _headOfPublisherId;
        private string _headAuthorName;
        private List<int> _authorIds;
        private List<int> _bookIds;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int HeadOfPublisherId
        {
            get => _headOfPublisherId;
            set => SetProperty(ref _headOfPublisherId, value);
        }

        public string HeadAuthorName
        {
            get => _headAuthorName;
            set => SetProperty(ref _headAuthorName, value);
        }

        public List<int> AuthorIds
        {
            get => _authorIds;
            set => SetProperty(ref _authorIds, value);
        }

        public List<int> BookIds
        {
            get => _bookIds;
            set => SetProperty(ref _bookIds, value);
        }

        public Publisher() { }

        public Publisher(string code, string name, int headOfPublisher, List<int> authors, List<int> books)
        {
            Code = code;
            Name = name;
            HeadOfPublisherId = headOfPublisher;
            AuthorIds = authors;
            BookIds = books;
        }

        public override string ToString()
        {
            string authorsPrint = AuthorIds.Count > 0 ? string.Join(", ", AuthorIds) : "nema autora";
            string booksPrint = BookIds.Count > 0 ? string.Join(", ", BookIds) : "nema knjiga";

            return $"{Id} | {Name} ({Code}) | Sef: {HeadOfPublisherId} | Autori: {authorsPrint} | Knjige: {booksPrint}";
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                Code,
                Name,
                HeadOfPublisherId.ToString(),
                string.Join(";", AuthorIds),
                string.Join(";", BookIds)
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Code = values[1];
            Name = values[2];
            HeadOfPublisherId = int.Parse(values[3]);
            AuthorIds = string.IsNullOrEmpty(values[4]) ? new List<int>() : values[4].Split(';').Select(int.Parse).ToList();
            BookIds = string.IsNullOrEmpty(values[5]) ? new List<int>() : values[5].Split(';').Select(int.Parse).ToList();
        }
    }
}
