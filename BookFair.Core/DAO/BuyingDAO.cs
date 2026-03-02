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
    public class BuyingDAO : IBuyingDAO
    {
        private readonly List<Buying> _buyings;
        private readonly Storage<Buying> _storage;

        public BuyingDAO()
        {
            _storage = new Storage<Buying>("buyings.csv");
            _buyings = _storage.Load();
        }

        private int GenerateId()
        {
            if (_buyings.Count == 0) return 1;
            return _buyings.Max(b => b.Id) + 1;
        }

        public Buying AddBuying(Buying buying)
        {
            buying.Id = GenerateId();
            _buyings.Add(buying);
            _storage.Save(_buyings);
            Console.WriteLine("Kupovina uspesno dodata.");
            return buying;
        }

        public Buying? UpdateBuying(Buying buying)
        {
            Buying? oldbuy = GetBuyingById(buying.Id);
            if (oldbuy == null)
                return null;

            oldbuy.BuyingDate = buying.BuyingDate;
            oldbuy.VisitorId = buying.VisitorId;
            oldbuy.BookId = buying.BookId;
            oldbuy.BuyingDate = buying.BuyingDate;
            oldbuy.Rating = buying.Rating;
            oldbuy.Comment = buying.Comment;

            _storage.Save(_buyings);
            Console.WriteLine("Kupovina uspesno izmenjena.");
            return oldbuy;
        }

        public Buying? RemoveBuying(int id)
        {
            Buying? buying = GetBuyingById(id);
            if (buying == null)
                return null;
            _buyings.Remove(buying);
            _storage.Save(_buyings);
            Console.WriteLine("Kupovina uspesno uklonjena.");
            return buying;
        }

        public Buying? GetBuyingById(int id)
        {
            Buying? buying = _buyings.Find(k => k.Id == id);
            if (buying == null)
                Console.WriteLine("Kupovina sa ID {0} nije pronadjena.", id);
            return buying;
        }

        public List<Buying> GetAllBuyings(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            IEnumerable<Buying> buyings = sortCriteria switch
            {
                "DatumKupovine" => _buyings.OrderBy(x => x.BuyingDate),
                "Posetilac" => _buyings.OrderBy(x => x.VisitorId),
                "Knjiga" => _buyings.OrderBy(x => x.BookId),
                "Ocena" => _buyings.OrderBy(x => x.Rating),
                _ => _buyings.OrderBy(x => x.Id)
            };

            // promeni redosled ukoliko ima potrebe za tim
            if (sortDirection == SortDirection.Descending)
                buyings = buyings.Reverse();

            // paginacija
            buyings = buyings.Skip((page - 1) * pageSize).Take(pageSize);

            return buyings.ToList();
        }

        public List<Buying> GetBuyingByBookAndVisitor(int visitorId, int bookId)
        {
            List<Buying> buyings = _buyings.Where(b => b.VisitorId == visitorId && b.BookId == bookId).ToList();
            if (buyings.Count == 0)
                Console.WriteLine("Kupovina za posetioca sa ID {0} i knjigu sa ID {1} nije pronadjena.", visitorId, bookId);
            return buyings;
        }
        public List<Buying> GetBuyingsByVisitor(int visitorId)
        {
            return _buyings.Where(b => b.VisitorId == visitorId).ToList();
        }
    }

}

