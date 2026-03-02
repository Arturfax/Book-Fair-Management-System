using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.Core.Controllers
{
    public class BookController
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        public void AddBook()
        {
            System.Console.WriteLine("\n--- Dodavanje knjige ---");

            System.Console.Write("ISBN: ");
            string isbn = System.Console.ReadLine() ?? "";

            System.Console.Write("Naziv: ");
            string name = System.Console.ReadLine() ?? "";

            System.Console.Write("Zanr: ");
            string genre = System.Console.ReadLine() ?? "";

            System.Console.Write("Godina izdanja: ");
            int year;
            while (!int.TryParse(System.Console.ReadLine(), out year))
            {
                System.Console.Write("Nevalidan broj. Pokusajte ponovo: ");
            }

            System.Console.Write("Cena: ");
            decimal price;
            while (!decimal.TryParse(System.Console.ReadLine(), out price))
            {
                System.Console.Write("Nevalidan broj. Pokusajte ponovo: ");
            }

            System.Console.Write("Broj strana: ");
            int pages;
            while (!int.TryParse(System.Console.ReadLine(), out pages))
            {
                System.Console.Write("Nevalidan broj. Pokusajte ponovo: ");
            }

            System.Console.Write("Izdavac: ");
            string publisher = System.Console.ReadLine() ?? "";

            var book = new Book
            {
                ISBN = isbn,
                Name = name,
                Genre = genre,
                YearOfRelease = year,
                Price = price,
                NumberOfPages = pages,
                Publisher = publisher,
                AuthorIds = new List<int> { },
                BuyerIds = new List<int> { },
                WishlistVisitorIds = new List<int> { }
            };

            var addedBook = _bookService.AddBook(book);
            System.Console.WriteLine($"\nKnjiga uspesno dodata! ID: {addedBook.Id}");
        }

        public Book AddBook(Book newBook)
        {
            var addedBook = _bookService.AddBook(newBook);
            return addedBook;
        }

        public List<Book> ViewAllBooks()
        {
            System.Console.WriteLine("\n--- Sve knjige ---");
            var books = _bookService.GetAllBooks();

            if (books.Count == 0)
            {
                System.Console.WriteLine("Nema knjiga u sistemu.");
            }

            return books;
        }

        public void UpdateBook()
        {
            System.Console.WriteLine("\n--- Izmena knjige ---");
            System.Console.Write("Unesite ID knjige: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                System.Console.WriteLine("Knjiga sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Trenutni podaci: {book}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Naziv [{book.Name}]: ");
            string name = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) book.Name = name;

            System.Console.Write($"Zanr [{book.Genre}]: ");
            string genre = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(genre)) book.Genre = genre;

            System.Console.Write($"Cena [{book.Price}]: ");
            string priceInput = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
            {
                book.Price = price;
            }

            System.Console.Write($"Izdavac [{book.Publisher}]: ");
            string publisher = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(publisher)) book.Publisher = publisher;

            _bookService.UpdateBook(book);
            System.Console.WriteLine("\nKnjiga uspesno izmenjena!");
        }

        public void UpdateBook(Book updatedBook)
        {
            _bookService.UpdateBook(updatedBook);
        }

        public void DeleteBook()
        {
            System.Console.WriteLine("\n--- Brisanje knjige ---");
            System.Console.Write("Unesite ID knjige: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                System.Console.WriteLine("Knjiga sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete: {book.Name}? (da/ne)");
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _bookService.RemoveBook(id);
                System.Console.WriteLine("Knjiga uspesno obrisana!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }

        public void DeleteBook(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return;
            }
            _bookService.RemoveBook(id);
        }

        public Book? GetBookById(int id)
        {
            return _bookService.GetBookById(id);
        }

        public List<Book> GetAllBooks()
        {
            return _bookService.GetAllBooks();
        }

    }
}
