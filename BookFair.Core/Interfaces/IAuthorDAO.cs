using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Interfaces
{
    public interface IAuthorDAO
    {
        Author AddAuthor(Author author);
        Author? UpdateAuthor(Author author);
        Author? RemoveAuthor(int id);
        Author? GetAuthorById(int id);
        List<Author> GetAllAuthors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);

    }
}
