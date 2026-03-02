using BookFair.Core.Interfaces;
using BookFair.Core.Models;
using BookFair.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BookFair.Core.DAO.VisitorDAO;

namespace BookFair.Core.DAO
{
    public class VisitorDAO : IVisitorDAO
    {
        private readonly List<Visitor> _visitors;
        private readonly Storage<Visitor> _storage;

        public VisitorDAO()
        {
            _storage = new Storage<Visitor>("visitors.csv");
            _visitors = _storage.Load();
        }

        private int GenerateId()
        {
            if (_visitors.Count == 0) return 1;
            return _visitors.Max(v => v.Id) + 1;
        }

        public Visitor AddVisitor(Visitor visitor)
        {
            visitor.Id = GenerateId();
            _visitors.Add(visitor);
            _storage.Save(_visitors);
            Console.WriteLine("Posetilac uspesno dodat.");
            return visitor;
        }

        public Visitor? UpdateVisitor(Visitor visitor)
        {
            Visitor? oldVisitor = GetVisitorById(visitor.Id);
            if (oldVisitor == null)
                return null;

            oldVisitor.Name = visitor.Name;
            oldVisitor.Surname = visitor.Surname;
            oldVisitor.DateOfBirth = visitor.DateOfBirth;
            oldVisitor.Address = visitor.Address;
            oldVisitor.Phone = visitor.Phone;
            oldVisitor.Email = visitor.Email;
            oldVisitor.MembershipCardNumber = visitor.MembershipCardNumber;
            oldVisitor.CurrentMembershipYear = visitor.CurrentMembershipYear;
            oldVisitor.Status = visitor.Status;
            oldVisitor.AverageRating = visitor.AverageRating;
            oldVisitor.BoughtBooks = visitor.BoughtBooks;
            oldVisitor.Wishlist = visitor.Wishlist;

            _storage.Save(_visitors);
            Console.WriteLine("Posetilac uspesno izmenjen.");
            return oldVisitor;
        }

        public Visitor? RemoveVisitor(int id)
        {
            Visitor? visitor = GetVisitorById(id);
            if (visitor == null)
                return null;
            _visitors.Remove(visitor);
            _storage.Save(_visitors);
            Console.WriteLine("Posetilac uspesno uklonjen.");
            return visitor;
        }

        public Visitor? GetVisitorById(int id)
        {
            Visitor? visitor = _visitors.Find(p => p.Id == id);
            if (visitor == null)
                Console.WriteLine("Posetilac sa ID {0} nije pronadjen.", id);
            return visitor;
        }

        public List<Visitor> GetAllVisitors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            IEnumerable<Visitor> visitors = sortCriteria switch
            {
                "Ime" => _visitors.OrderBy(x => x.Name),
                "Prezime" => _visitors.OrderBy(x => x.Surname),
                "DatumRodjenja" => _visitors.OrderBy(x => x.DateOfBirth),
                "Adresa" => _visitors.OrderBy(x => x.Address),
                "Telefon" => _visitors.OrderBy(x => x.Phone),
                "Email" => _visitors.OrderBy(x => x.Email),
                "BrojClanskeKarte" => _visitors.OrderBy(x => x.MembershipCardNumber),
                "TrenutnaGodinaClanstva" => _visitors.OrderBy(x => x.CurrentMembershipYear),
                "Status" => _visitors.OrderBy(x => x.Status),
                "ProsjecnaOcenaRecenzija" => _visitors.OrderBy(x => x.AverageRating),
                _ => _visitors.OrderBy(x => x.Id)
            };

            // promeni redosled ukoliko ima potrebe za tim
            if (sortDirection == SortDirection.Descending)
                visitors = visitors.Reverse();

            // paginacija
            visitors = visitors.Skip((page - 1) * pageSize).Take(pageSize);

            return visitors.ToList();
        }
    }
}
