using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Interfaces
{
    public interface IPublisherDAO
    {
        Publisher AddPublisher(Publisher publisher);
        Publisher? UpdatePublisher(Publisher publisher);
        Publisher? RemovePublisher(int id);
        Publisher? GetPublisherById(int id);
        List<Publisher> GetAllPublishers(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);
    }
}
