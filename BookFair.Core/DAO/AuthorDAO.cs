using BookFair.Core.Models;
using BookFair.Core.Interfaces;
using BookFair.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.DAO
{
    public class AuthorDAO : IAuthorDAO
    {
        private readonly List<Author> _authors;
        private readonly Storage<Author> _storage;

        public AuthorDAO()
        {
            _storage = new Storage<Author>("authors.csv");
            _authors = _storage.Load();

        }

        private int GenerateId()
        {
            if (_authors.Count == 0) return 1;
            return _authors.Max(a => a.Id) + 1;
        }

        public Author AddAuthor(Author author)
        {
            author.Id = GenerateId();
            _authors.Add(author);
            _storage.Save(_authors);
            Console.WriteLine("Autor uspesno dodat.");
            return author;
        }


        public Author? UpdateAuthor(Author author)
        {
            Author? oldAuthor = GetAuthorById(author.Id);
            if (oldAuthor == null)
                return null;

            oldAuthor.Name = author.Name;
            oldAuthor.Surname = author.Surname;
            oldAuthor.DateOfBirth = author.DateOfBirth;
            oldAuthor.Address = author.Address;
            oldAuthor.Phone = author.Phone;
            oldAuthor.Email = author.Email;
            oldAuthor.IDCardNumber = author.IDCardNumber;
            oldAuthor.YearsOfExperience = author.YearsOfExperience;


            _storage.Save(_authors);
            Console.WriteLine("Autor uspesno izmenjen.");
            return oldAuthor;
        }

        public Author? RemoveAuthor(int id)
        {
            Author? author = GetAuthorById(id);
            if (author == null)
                return null;
            _authors.Remove(author);
            _storage.Save(_authors);
            Console.WriteLine("Autor uspesno uklonjen.");
            return author;
        }


        public Author? GetAuthorById(int id)
        {
            Author? author = _authors.Find(a => a.Id == id);
            if (author == null)
                Console.WriteLine("Autor sa ID {0} nije pronadjen.", id);
            return author;
        }

        public List<Author> GetAllAuthors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            IEnumerable<Author> authors = sortCriteria switch
            {
                "Ime" => _authors.OrderBy(x => x.Name),
                "Prezime" => _authors.OrderBy(x => x.Surname),
                "DatumRodjenja" => _authors.OrderBy(x => x.DateOfBirth),
                "Adresa" => _authors.OrderBy(x => x.Address),
                "Telefon" => _authors.OrderBy(x => x.Phone),
                "Email" => _authors.OrderBy(x => x.Email),
                "BrojLicneKarte" => _authors.OrderBy(x => x.IDCardNumber),
                "GodineIskustva" => _authors.OrderBy(x => x.YearsOfExperience),
                _ => _authors.OrderBy(x => x.Id)
            };


            // promeni redosled ukoliko ima potrebe za tim
            if (sortDirection == SortDirection.Descending)
                authors = authors.Reverse();

            authors = authors.Skip((page - 1) * pageSize).Take(pageSize);

            return authors.ToList();


        }
    }
}
