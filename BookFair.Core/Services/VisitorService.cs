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
    public class VisitorService
    {
        private readonly IVisitorDAO _visitorDAO;
        private readonly IBookDAO _bookDAO;

        public VisitorService(IVisitorDAO visitorDAO, IBookDAO bookDAO)
        {
            _visitorDAO = visitorDAO;
            _bookDAO = bookDAO;
        }

        public Visitor AddVisitor(Visitor visitor) { 
            return _visitorDAO.AddVisitor(visitor);
        }
        public Visitor? UpdateVisitor(Visitor visitor) { 
            return _visitorDAO.UpdateVisitor(visitor);
        }
        public Visitor? RemoveVisitor(int id) { 
            return _visitorDAO.RemoveVisitor(id);
        }
        public Visitor? GetVisitorById(int id) { 
            return _visitorDAO.GetVisitorById(id);
        }
        public List<Visitor> GetAllVisitors(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending) { 
            return _visitorDAO.GetAllVisitors(page, pageSize, sortCriteria, sortDirection);
        }

        public void AddBookToWishlist(int visitorId, int bookId)
        {
           Visitor visitor = _visitorDAO.GetVisitorById(visitorId);
           Book book = _bookDAO.GetBookById(bookId);
            if (visitor != null){
                if (!visitor.Wishlist.Contains(bookId))
                {
                        visitor.Wishlist.Add(bookId);
                        _visitorDAO.UpdateVisitor(visitor);

                }
            }

            if (book != null)
            {
                if (!book.WishlistVisitorIds.Contains(visitorId))
                {
                    book.WishlistVisitorIds.Add(visitorId);
                    _bookDAO.UpdateBook(book);

                }
            }
        }

    }
}
