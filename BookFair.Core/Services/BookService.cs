using BookFair.Core.DAO;
using BookFair.Core.Interfaces;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Services
{
    public class BookService
    {
        private readonly IBookDAO _bookDAO;

        public BookService(IBookDAO bookDAO)
        {
            _bookDAO = bookDAO;
        }

        public Book AddBook(Book book)
        {
            return _bookDAO.AddBook(book);
        }

        public Book? UpdateBook(Book book)
        {
            return _bookDAO.UpdateBook(book);
        }

        public Book? RemoveBook(int id)
        {
            return _bookDAO.RemoveBook(id);
        }

        public Book? GetBookById(int id)
        {
            return _bookDAO.GetBookById(id);
        }

        public List<Book> GetAllBooks(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            return _bookDAO.GetAllBooks(page, pageSize, sortCriteria, sortDirection);
        }
    }
}
