using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Interfaces
{
    public interface IBuyingDAO
    {
        Buying AddBuying(Buying buying);
        Buying? UpdateBuying(Buying buying);
        Buying? RemoveBuying(int id);
        Buying? GetBuyingById(int id);
        List<Buying> GetAllBuyings(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);
        List<Buying> GetBuyingByBookAndVisitor(int visitorId, int bookId);
        List<Buying> GetBuyingsByVisitor(int visitorId);

    }
}
