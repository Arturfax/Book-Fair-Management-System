using BookFair.Core.Models;
using BookFair.Core.Services;
using System;
using System.Linq;

namespace BookFair.Core.Controllers
{
    public class AuthorController
    {
        private readonly AuthorService _authorService;

        public AuthorController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        public void AddAuthor()
        {
            System.Console.WriteLine("\n--- Dodavanje autora ---");

            System.Console.Write("Ime: ");
            string name = System.Console.ReadLine() ?? "";

            System.Console.Write("Prezime: ");
            string surname = System.Console.ReadLine() ?? "";

            System.Console.Write("Datum rodjenja (YYYY-MM-DD): ");
            DateTime dateOfBirth;
            while (!DateTime.TryParse(System.Console.ReadLine(), out dateOfBirth))
            {
                System.Console.Write("Nevalidan datum. Pokusajte ponovo (YYYY-MM-DD): ");
            }

            System.Console.Write("Adresa(Street,Number,City,Country): ");
            string address = System.Console.ReadLine() ?? "";

            System.Console.Write("Telefon: ");
            string phone = System.Console.ReadLine() ?? "";

            System.Console.Write("Email: ");
            string email = System.Console.ReadLine() ?? "";

            System.Console.Write("Broj licne karte: ");
            string idCard = System.Console.ReadLine() ?? "";

            System.Console.Write("Godine iskustva: ");
            int experience;
            while (!int.TryParse(System.Console.ReadLine(), out experience))
            {
                System.Console.Write("Nevalidan broj. Pokusajte ponovo: ");
            }

            var author = new Author
            {
                Name = name,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                Address = Address.Parse(address),
                Phone = phone,
                Email = email,
                IDCardNumber = idCard,
                YearsOfExperience = experience
            };

            var addedAuthor = _authorService.AddAuthor(author);
            System.Console.WriteLine($"\nAutor uspesno dodat! ID: {addedAuthor.Id}");
        }

        public Author AddAuthor(Author newAuthor)
        {
            return _authorService.AddAuthor(newAuthor);
        }

        public List<Author> ViewAllAuthors()
        {
            System.Console.WriteLine("\n--- Svi autori ---");
            var authors = _authorService.GetAllAuthors();

            if (authors.Count == 0)
            {
                System.Console.WriteLine("Nema autora u sistemu.");
            }

            return authors;
        }

        public Author? GetAuthorById(int id)
        {
            return _authorService.GetAuthorById(id);
        }

        public List<Author> GetAllAuthors()
        {
            return _authorService.GetAllAuthors();
        }

        public Author? FindByNameAndSurname(string name, string surname)
        {
            var authors = _authorService.GetAllAuthors();
            return authors.FirstOrDefault(a =>
                a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                a.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase));
        }

        public Author UpdateAuthor()
        {
            System.Console.WriteLine("\n--- Izmena autora ---");
            System.Console.Write("Unesite ID autora: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                System.Console.WriteLine("Autor sa tim ID-jem ne postoji.");
            }

            System.Console.WriteLine($"Trenutni podaci: {author}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Ime [{author.Name}]: ");
            string name = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) author.Name = name;

            System.Console.Write($"Prezime [{author.Surname}]: ");
            string surname = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(surname)) author.Surname = surname;

            System.Console.Write($"Adresa [{author.Address}]: ");
            string address = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address)) author.Address = Address.Parse(address);

            System.Console.Write($"Telefon [{author.Phone}]: ");
            string phone = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone)) author.Phone = phone;

            System.Console.Write($"Email [{author.Email}]: ");
            string email = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) author.Email = email;

            System.Console.Write($"Godine iskustva [{author.YearsOfExperience}]: ");
            string experienceInput = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(experienceInput) && int.TryParse(experienceInput, out int experience))
            {
                author.YearsOfExperience = experience;
            }

            return _authorService.UpdateAuthor(author);
            System.Console.WriteLine("\nAutor uspesno izmenjen!");
        }

        public void UpdateAuthor(Author updatedAuthor)
        {
            _authorService.UpdateAuthor(updatedAuthor);
        }

        public void DeleteAuthor(int id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                System.Console.WriteLine("Autor sa tim ID-jem ne postoji.");
                return;
            }
            _authorService.RemoveAuthor(id);
        }


        public void DeleteAuthor()
        {
            System.Console.WriteLine("\n--- Brisanje autora ---");
            System.Console.Write("Unesite ID autora: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                System.Console.WriteLine("Autor sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete: {author.Name} {author.Surname}? (da/ne)");
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _authorService.RemoveAuthor(id);
                System.Console.WriteLine("Autor uspesno obrisan!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }
    }
}
