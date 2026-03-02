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
    public class PublisherService
    {
        private readonly IPublisherDAO _publisherDAO;

        public PublisherService(IPublisherDAO publisherDAO)
        {
            _publisherDAO = publisherDAO;
        }

        public Publisher AddPublisher(Publisher publisher)
        {
            return _publisherDAO.AddPublisher(publisher);
        }
        public Publisher? UpdatePublisher(Publisher publisher)
        {
            return _publisherDAO.UpdatePublisher(publisher);
        }
        public Publisher? RemovePublisher(int id)
        {
            return _publisherDAO.RemovePublisher(id);
        }
        public Publisher? GetPublisherById(int id)
        {
            return _publisherDAO.GetPublisherById(id);
        }
        public List<Publisher> GetAllPublishers(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            return _publisherDAO.GetAllPublishers(page, pageSize, sortCriteria, sortDirection);
        }


    }
}

