using BookFair.Core.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookFair.WPF.DTO
{
    public class AuthorDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(nameof(Name)); } }
        }

        private string _surname = string.Empty;
        public string Surname
        {
            get => _surname;
            set { if (_surname != value) { _surname = value ?? string.Empty; OnPropertyChanged(nameof(Surname)); } }
        }

        private DateTime _dateOfBirth = DateTime.Today;
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set { if (_dateOfBirth != value) { _dateOfBirth = value; OnPropertyChanged(nameof(DateOfBirth)); } }
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set { if (_phone != value) { _phone = value ?? string.Empty; OnPropertyChanged(nameof(Phone)); } }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value ?? string.Empty; OnPropertyChanged(nameof(Email)); } }
        }

        private string _idCardNumber = string.Empty;
        public string IDCardNumber
        {
            get => _idCardNumber;
            set { if (_idCardNumber != value) { _idCardNumber = value ?? string.Empty; OnPropertyChanged(nameof(IDCardNumber)); } }
        }

        private int _yearsOfExperience;
        public int YearsOfExperience
        {
            get => _yearsOfExperience;
            set { if (_yearsOfExperience != value) { _yearsOfExperience = value; OnPropertyChanged(nameof(YearsOfExperience)); } }
        }

        private string _yearsOfExperienceText = "0";
        public string YearsOfExperienceText
        {
            get => _yearsOfExperienceText;
            set
            {
                if (_yearsOfExperienceText != value)
                {
                    _yearsOfExperienceText = value ?? string.Empty;
                    if (int.TryParse(value, out int years))
                    {
                        _yearsOfExperience = years;
                    }
                    OnPropertyChanged(nameof(YearsOfExperienceText));
                }
            }
        }

        public AddressDTO Address { get; set; } = new AddressDTO();

        public AuthorDTO() { }

        public AuthorDTO(Author author)
        {
            if (author == null) return;

            Id = author.Id;
            Name = author.Name ?? string.Empty;
            Surname = author.Surname ?? string.Empty;
            DateOfBirth = author.DateOfBirth;
            Phone = author.Phone ?? string.Empty;
            Email = author.Email ?? string.Empty;
            IDCardNumber = author.IDCardNumber ?? string.Empty;
            YearsOfExperience = author.YearsOfExperience;
            YearsOfExperienceText = author.YearsOfExperience.ToString();
            Address = new AddressDTO(author.Address ?? new Address());
        }

        public Author ToAuthor()
        {
            return new Author
            {
                Id = Id,
                Name = Name,
                Surname = Surname,
                DateOfBirth = DateOfBirth,
                Address = Address.ToAddress(),
                Phone = Phone,
                Email = Email,
                IDCardNumber = IDCardNumber,
                YearsOfExperience = YearsOfExperience
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static readonly Regex _nameRegex = new(@"^\p{Lu}\p{Ll}{2,}$");
        private static readonly Regex _mailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        public string Error => string.Empty;

        public string this[string columnName] =>
            columnName switch
            {
                nameof(Name) => string.IsNullOrWhiteSpace(Name) || !_nameRegex.IsMatch(Name)
                    ? "Name must start with uppercase and have at least 3 letters."
                    : string.Empty,
                nameof(Surname) => string.IsNullOrWhiteSpace(Surname) || !_nameRegex.IsMatch(Surname)
                    ? "Surname must start with uppercase and have at least 3 letters."
                    : string.Empty,
                nameof(Email) => string.IsNullOrWhiteSpace(Email) || !_mailRegex.IsMatch(Email)
                    ? "Invalid e-mail format."
                    : string.Empty,
                nameof(Phone) => string.IsNullOrWhiteSpace(Phone)
                    ? "Phone is required."
                    : string.Empty,
                nameof(DateOfBirth) => DateOfBirth == default
                    ? "Date of birth is required."
                    : string.Empty,
                nameof(IDCardNumber) => string.IsNullOrWhiteSpace(IDCardNumber)
                    ? "ID Card Number is required."
                    : string.Empty,
                nameof(YearsOfExperienceText) => !int.TryParse(YearsOfExperienceText, out int years) || years < 0
                    ? "Enter a valid number of years."
                    : string.Empty,
                _ => string.Empty
            };

        public string IsValid =>
            new[] { nameof(Name), nameof(Surname), nameof(Email), nameof(Phone), nameof(DateOfBirth), nameof(IDCardNumber), nameof(YearsOfExperienceText) }
                .Select(p => this[p])
                .FirstOrDefault(e => !string.IsNullOrEmpty(e)) ?? string.Empty;
    }
}
