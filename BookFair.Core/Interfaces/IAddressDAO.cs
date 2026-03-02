using BookFair.Core.DAO;
using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Interfaces
{
    public interface IAddressDAO
    {
        Address AddAddress(Address address);
        Address? UpdateAddress(Address address);
        Address? RemoveAddress(int id);
        Address? GetAddressById(int id);
        List<Address> GetAllAddresses(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending);

    }
}
