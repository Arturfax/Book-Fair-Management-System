using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.Core.Controllers
{
    public class AddressController
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        public Address AddAddress(Address newAddress)
        { 
            return _addressService.AddAddress(newAddress);
        }

        public void AddAddress()
        {
            System.Console.WriteLine("\n--- Dodavanje adrese ---");

            System.Console.Write("Ulica: ");
            string street = System.Console.ReadLine() ?? "";

            System.Console.Write("Broj: ");
            string number = System.Console.ReadLine() ?? "";

            System.Console.Write("Grad: ");
            string city = System.Console.ReadLine() ?? "";

            System.Console.Write("Drzava: ");
            string country = System.Console.ReadLine() ?? "";

            var address = new Address
            {
                Street = street,
                Number = number,
                City = city,
                Country = country
            };

            _addressService.AddAddress(address);
            System.Console.WriteLine($"\nAdresa uspesno dodata! ID: {address.Id}");
        }

        public void ViewAllAddresses()
        {
            System.Console.WriteLine("\n--- Sve adrese ---");
            var addresses = _addressService.GetAllAddresses();

            if (addresses.Count == 0)
            {
                System.Console.WriteLine("Nema adresa u sistemu.");
                return;
            }

            foreach (var address in addresses)
            {
                System.Console.WriteLine(address);
            }
        }

        public void UpdateAddress()
        {
            System.Console.WriteLine("\n--- Izmena adrese ---");
            System.Console.Write("Unesite ID adrese: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var address = _addressService.GetAddressById(id);
            if (address == null)
            {
                System.Console.WriteLine("Adresa sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Trenutni podaci: {address}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Ulica [{address.Street}]: ");
            string street = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(street)) address.Street = street;

            System.Console.Write($"Broj [{address.Number}]: ");
            string number = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(number)) address.Number = number;

            System.Console.Write($"Grad [{address.City}]: ");
            string city = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(city)) address.City = city;

            System.Console.Write($"Drzava [{address.Country}]: ");
            string country = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(country)) address.Country = country;

            _addressService.UpdateAddress(address);
            System.Console.WriteLine("\nAdresa uspesno izmenjena!");
        }

        public Address UpdateAddress(Address updatedAddress)
        {
            _addressService.UpdateAddress(updatedAddress);
            return updatedAddress;
        }
        public void DeleteAddress(int id)
        {
            var address = _addressService.GetAddressById(id);
            if (address == null)
            {
                System.Console.WriteLine("Adresa sa tim ID-jem ne postoji.");
                return;
            }
            _addressService.RemoveAddress(id);
        }

        public void DeleteAddress()
        {
            System.Console.WriteLine("\n--- Brisanje adrese ---");
            System.Console.Write("Unesite ID adrese: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var address = _addressService.GetAddressById(id);
            if (address == null)
            {
                System.Console.WriteLine("Adresa sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete: {address.Street} {address.Number}, {address.City}? (da/ne)");
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _addressService.RemoveAddress(id);
                System.Console.WriteLine("Adresa uspesno obrisana!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }
    }
}
