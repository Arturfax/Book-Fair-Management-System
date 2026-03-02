using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Interfaces
{
    public interface IBookDAO
    {
        Book AddBook(Book book);
        Book? UpdateBook(Book book);
        Book? RemoveBook(int id);
        Book? GetBookById(int id);
        List<Book> GetAllBooks(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);
    }
}
