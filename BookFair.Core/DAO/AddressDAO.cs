using BookFair.Core.Interfaces;
using BookFair.Core.Models;
using BookFair.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookFair.Core.Utils;
using static BookFair.Core.DAO.AddressDAO;

namespace BookFair.Core.DAO
{
    public class AddressDAO : IAddressDAO
    {
        private readonly List<Address> _addresses;
        private readonly Storage<Address> _storage;

        public AddressDAO()
        {
            _storage = new Storage<Address>("addresses.csv");
            _addresses = _storage.Load();
        }

        private int GenerateId()
        {
            if (_addresses.Count == 0) return 1;
            return _addresses.Max(a => a.Id) + 1;
        }

        public Address AddAddress(Address address)
        {
            address.Id = GenerateId();
            _addresses.Add(address);
            _storage.Save(_addresses);
            Console.WriteLine("Adresa uspesno dodata.");
            return address;
        }

        public Address? UpdateAddress(Address address)
        {
            Address? oldAddress = GetAddressById(address.Id);
            if (oldAddress == null)
                return null;

            oldAddress.Street = address.Street;
            oldAddress.Number = address.Number;
            oldAddress.City = address.City;
            oldAddress.Country = address.Country;
            

            _storage.Save(_addresses);
            Console.WriteLine("Adresa uspesno izmenjena.");
            return oldAddress;
        }

        public Address? RemoveAddress(int id)
        {
            Address? address = GetAddressById(id);
            if (address == null)
                return null;
            _addresses.Remove(address);
            _storage.Save(_addresses);
            Console.WriteLine("Adresa uspesno uklonjena.");
            return address;
        }

        public Address? GetAddressById(int id)
        {
            Address? address = _addresses.Find(a => a.Id == id);
            if (address == null)
                Console.WriteLine("Adresa sa ID {0} nije pronadjena.", id);
            return address;
        }

        public List<Address> GetAllAddresses(int page = 1, int pageSize = 50, string sortCriteria = "Id", SortDirection sortDirection = SortDirection.Ascending)
        {
            IEnumerable<Address> addresses = sortCriteria switch
            {
                "Ulica" => _addresses.OrderBy(x => x.Street),
                "Broj" => _addresses.OrderBy(x => x.Number),
                "Grad" => _addresses.OrderBy(x => x.City),
                "Drzava" => _addresses.OrderBy(x => x.Country),
                _ => _addresses.OrderBy(x => x.Id)
            };

            // promeni redosled ukoliko ima potrebe za tim
            if (sortDirection == SortDirection.Descending)
                addresses = addresses.Reverse();

            // paginacija
            addresses = addresses.Skip((page - 1) * pageSize).Take(pageSize);

            return addresses.ToList();
        }
    }
}
