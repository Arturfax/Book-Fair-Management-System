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
    public class AuthorService
    {
        private readonly IAuthorDAO _authorDAO;
       

        public AuthorService(IAuthorDAO authorDAO)
        {
            _authorDAO = authorDAO;
            
        }

        public Author AddAuthor(Author author)
        {
            return _authorDAO.AddAuthor(author);
        }
        public Author? UpdateAuthor(Author author)
        {
            return _authorDAO.UpdateAuthor(author);
        }
        public Author? RemoveAuthor(int id)
        {
            return _authorDAO.RemoveAuthor(id);
        }
        public Author? GetAuthorById(int id)
        {
            return _authorDAO.GetAuthorById(id);
        }
        public List<Author> GetAllAuthors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            return _authorDAO.GetAllAuthors(page, pageSize, sortCriteria, sortDirection);
        }

       

    }
}
