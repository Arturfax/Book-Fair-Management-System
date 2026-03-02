using BookFair.Core.Models.Enums;
using BookFair.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Models
{
    public class Address : ObservableModel, Serializable
    {
        private int _id;
        private string _street;
        private string _number;
        private string _city;
        private string _country;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        public string Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }
        
        
        public Address()
        {
        }

        public Address(string street, string number, string city, string country)
        {
            Street = street;
            Number = number;
            City = city;
            Country = country;
        
        }
        public override string ToString()
        { 

            return $"{Street} ,{Number} , {City} , {Country}";
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Street,
                Number,
                City,
                Country
            };
        }

        public void FromCSV(string[] values)
        {
            Street = values[0];
            Number = values[1];
            City = values[2];
            Country = values[3];
        }

        public static Address Parse(string addressString)
        {
            if (string.IsNullOrEmpty(addressString))
            {
                return new Address();
            }

            var parts = addressString.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException("Invalid address format. Expected format: Street,Number,City,Country");
            }

            return new Address
            {
                Street = parts[0],
                Number = parts[1],
                City = parts[2],
                Country = parts[3]
            };
        }

        public string ToAddressString()
        {
            return $"{Street},{Number},{City},{Country}";
        }

    }
}
