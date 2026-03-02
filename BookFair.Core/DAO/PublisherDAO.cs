using BookFair.Core.Models;
using BookFair.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BookFair.Core.Interfaces;

namespace BookFair.Core.DAO
{
    public class PublisherDAO : IPublisherDAO
    {

        private readonly List<Publisher> _publishers;
        private readonly Storage<Publisher> _storage;

        public PublisherDAO()
        {
            _storage = new Storage<Publisher>("publishers.csv");
            _publishers = _storage.Load();
        }

        private int GenerateId()
        {
            if (_publishers.Count == 0) return 1;
            return _publishers.Max(p => p.Id) + 1;
        }

        public Publisher AddPublisher(Publisher publisher)
        {
            publisher.Id = GenerateId();
            _publishers.Add(publisher);
            _storage.Save(_publishers);
            Console.WriteLine("Izdavac uspesno dodat.");
            return publisher;
        }

        public Publisher? UpdatePublisher(Publisher publisher)
        {
            Publisher? oldPublisher = GetPublisherById(publisher.Id);
            if (oldPublisher == null)
                return null;

            oldPublisher.Code = publisher.Code;
            oldPublisher.Name = publisher.Name;
            oldPublisher.HeadOfPublisherId = publisher.HeadOfPublisherId;
            oldPublisher.AuthorIds = publisher.AuthorIds;
            oldPublisher.BookIds = publisher.BookIds;

            _storage.Save(_publishers);
            Console.WriteLine("Izdavac uspesno izmenjen.");
            return oldPublisher;
        }

        public Publisher? RemovePublisher(int id)
        {
            Publisher? publisher = GetPublisherById(id);
            if (publisher == null)
                return null;
            _publishers.Remove(publisher);
            _storage.Save(_publishers);
            Console.WriteLine("Izdavac uspesno uklonjen.");
            return publisher;
        }

        public Publisher? GetPublisherById(int id)
        {
            Publisher? publisher = _publishers.Find(a => a.Id == id);
            if (publisher == null)
                Console.WriteLine("Izdavac sa ID {0} nije pronadjen.", id);
            return publisher;
        }

        public List<Publisher> GetAllPublishers(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            IEnumerable<Publisher> publishers = sortCriteria switch
            {
                "Sifra" => _publishers.OrderBy(x => x.Code),
                "Naziv" => _publishers.OrderBy(x => x.Name),
                "SefIzdavaca" => _publishers.OrderBy(x => x.HeadOfPublisherId),
                _ => _publishers.OrderBy(x => x.Id)
            };

            // promeni redosled ukoliko ima potrebe za tim
            if (sortDirection == SortDirection.Descending)
                publishers = publishers.Reverse();

            // paginacija
            publishers = publishers.Skip((page - 1) * pageSize).Take(pageSize);

            return publishers.ToList();
        }
    }
}
