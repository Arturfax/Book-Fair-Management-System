using BookFair.Core.Models.Enums;
using BookFair.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.Core.Models
{
    public class Author : ObservableModel, Serializable
    {
        private int _id;
        private string _name;
        private string _surname;
        private DateTime _dateOfBirth;
        private Address _address;
        private string _phone;
        private string _email;
        private string _idCardNumber;
        private int _yearsOfExperience;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value);
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        public Address Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string IDCardNumber
        {
            get => _idCardNumber;
            set => SetProperty(ref _idCardNumber, value);
        }

        public int YearsOfExperience
        {
            get => _yearsOfExperience;
            set => SetProperty(ref _yearsOfExperience, value);
        }

        public Author()
        {
        }

        public Author(string name, string surname, DateTime dateOfBirth, Address address, string phone, string email, string iDCardNumber, int yearsOfExperience)
        {
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            Address = address;
            Phone = phone;
            Email = email;
            IDCardNumber = iDCardNumber;
            YearsOfExperience = yearsOfExperience;

        }
        public override string ToString()
        {
            
            return $"{Id}| {Name} {Surname} | {DateOfBirth:yyyy-MM-dd} | {Address} | {Phone} | {Email} | {IDCardNumber} | {YearsOfExperience}";
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                Name,
                Surname,
                DateOfBirth.ToString("yyyy-MM-dd"),
                Address?.ToAddressString() ?? string.Empty,
                Phone,
                Email,
                IDCardNumber,
                YearsOfExperience.ToString(),

            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Name = values[1];
            Surname = values[2];
            DateOfBirth = DateTime.Parse(values[3]);
            Address = Address.Parse(values[4]);
            Phone = values[5];
            Email = values[6];
            IDCardNumber = values[7];
            YearsOfExperience = int.Parse(values[8]);

        }

    }
}
