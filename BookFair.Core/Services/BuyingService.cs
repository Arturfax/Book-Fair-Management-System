using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookFair.Core.Interfaces;

namespace BookFair.Core.Services
{
    public class BuyingService
    {
        private readonly IBuyingDAO _buyingDAO;
        private readonly IVisitorDAO _visitorDAO;
        public BuyingService(IBuyingDAO buyingDAO, IVisitorDAO visitorDAO)
        {
            _buyingDAO = buyingDAO;
            _visitorDAO = visitorDAO;
        }
        public Buying AddBuying(Buying buying)
        {
            return _buyingDAO.AddBuying(buying);
        }
        public Buying? UpdateBuying(Buying buying)
        {
            return _buyingDAO.UpdateBuying(buying);
        }
        public Buying? RemoveBuying(int id)
        {
            return _buyingDAO.RemoveBuying(id);
        }
        public Buying? GetBuyingById(int id)
        {
            return _buyingDAO.GetBuyingById(id);
        }
        public List<Buying> GetAllBuying(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            return _buyingDAO.GetAllBuyings(page, pageSize, sortCriteria, sortDirection);

        }

        public List<Buying> GetBuyingByBookAndVisitor(int visitorId, int bookId)
        {
            return _buyingDAO.GetBuyingByBookAndVisitor(visitorId, bookId);

        }

        public List<Buying> GetBuyingsByVisitor(int visitorId)
        {
            return _buyingDAO.GetBuyingsByVisitor(visitorId);
        }

    }
}
