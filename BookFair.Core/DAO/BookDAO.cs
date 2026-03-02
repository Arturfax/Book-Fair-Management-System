using BookFair.Core.Interfaces;
using BookFair.Core.Models;
using BookFair.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BookFair.Core.Utils;
namespace BookFair.Core.DAO;

public class BookDAO : IBookDAO
{
    private readonly List<Book> _books;
    private readonly Storage<Book> _storage;

    public BookDAO()
    {
        _storage = new Storage<Book>("books.csv");
        _books = _storage.Load();
    }

    private int GenerateId()
    {
        if (_books.Count == 0) return 1;
        return _books.Max(bo => bo.Id) + 1;
    }

    public Book AddBook(Book book)
    {
        if (book.AuthorIds == null || book.AuthorIds.Count == 0)
        {
            Console.WriteLine("Greska: Knjiga mora imati bar jednog autora!");
            return null;
        }
        book.Id = GenerateId();
        _books.Add(book);
        _storage.Save(_books);
        Console.WriteLine("Knjiga uspesno dodata.");
        return book;
    }

    public Book? UpdateBook(Book book)
    {
        Book? oldBook = GetBookById(book.Id);
        if (oldBook == null)
            return null;

        oldBook.ISBN = book.ISBN;
        oldBook.Name = book.Name;
        oldBook.Genre = book.Genre;
        oldBook.YearOfRelease = book.YearOfRelease;
        oldBook.Price = book.Price;
        oldBook.NumberOfPages = book.NumberOfPages;
        oldBook.AuthorIds = book.AuthorIds;
        oldBook.Publisher = book.Publisher;
        oldBook.BuyerIds = book.BuyerIds;
        oldBook.WishlistVisitorIds = book.WishlistVisitorIds;

        _storage.Save(_books);
        Console.WriteLine("Knjiga uspesno izmenjena.");
        return oldBook;
    }

    public Book? RemoveBook(int id)
    {
        Book? book = GetBookById(id);
        if (book == null)
            return null;
        _books.Remove(book);
        _storage.Save(_books);
        Console.WriteLine("Knjiga uspesno uklonjena.");
        return book;
    }

    public Book? GetBookById(int id)
    {
        Book? book = _books.Find(k => k.Id == id);
        if (book == null)
            Console.WriteLine("Knjiga sa ID {0} nije pronadjena.", id);
        return book;
    }

    public List<Book> GetAllBooks(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
    {
        IEnumerable<Book> knjige = sortCriteria switch
        {
            "ISBN" => _books.OrderBy(x => x.ISBN),
            "Naziv" => _books.OrderBy(x => x.Name),
            "Zanr" => _books.OrderBy(x => x.Genre),
            "GodinaIzdanja" => _books.OrderBy(x => x.YearOfRelease),
            "Cena" => _books.OrderBy(x => x.Price),
            "BrojStrana" => _books.OrderBy(x => x.NumberOfPages),
            "Autori" => _books.OrderBy(x => x.AuthorIds),
            "Izdavac" => _books.OrderBy(x => x.Publisher),
            "Kupci" => _books.OrderBy(x => x.BuyerIds),
            "ListaZelja" => _books.OrderBy(x => x.WishlistVisitorIds),
            _ => _books.OrderBy(x => x.Id)
        };

        // promeni redosled ukoliko ima potrebe za tim
        if (sortDirection == SortDirection.Descending)
            knjige = knjige.Reverse();

        // paginacija
        knjige = knjige.Skip((page - 1) * pageSize).Take(pageSize);

        return knjige.ToList();
    }
}
