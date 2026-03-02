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
    public class AddressService
    {
        private readonly IAddressDAO _addressDAO;
        

        public AddressService(IAddressDAO addressDAO)
        {
            _addressDAO = addressDAO;
            
        }

        public Address AddAddress(Address address)
        {
            return _addressDAO.AddAddress(address);
        }
        public Address? UpdateAddress(Address address)
        {
            return _addressDAO.UpdateAddress(address);
        }
        public Address? RemoveAddress(int id)
        {
            return _addressDAO.RemoveAddress(id);
        }
        public Address? GetAddressById(int id)
        {
            return _addressDAO.GetAddressById(id);
        }
        public List<Address> GetAllAddresses(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            return _addressDAO.GetAllAddresses(page, pageSize, sortCriteria, sortDirection);
        }

        

    }
}
