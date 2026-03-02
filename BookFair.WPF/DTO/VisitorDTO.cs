using BookFair.Core.Models;
using BookFair.Core.Models.Enums;
using BookFair.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookFair.WPF.DTO
{
    public class VisitorDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        private DateTime _dateOfBirth = DateTime.Today;
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set { if (_dateOfBirth != value) { _dateOfBirth = value; OnPropertyChanged(nameof(DateOfBirth)); } }
        }

        public string MembershipCardNumber { get; set; } = string.Empty;

        private int _currentMembershipYearBinding;
        public int CurrentMembershipYearBinding
        {
            get => _currentMembershipYearBinding;
            set { if (_currentMembershipYearBinding != value) { _currentMembershipYearBinding = value; OnPropertyChanged(nameof(CurrentMembershipYearBinding)); } }
        }

        public int StatusBinding { get; set; }

        public AddressDTO Address { get; set; } = new AddressDTO();


        public VisitorDTO() { }

        public VisitorDTO(Visitor visitor)
        {
            if (visitor == null) return;

            Id = visitor.Id;
            Name = visitor.Name ?? string.Empty;
            Surname = visitor.Surname ?? string.Empty;
            Email = visitor.Email ?? string.Empty;
            Phone = visitor.Phone ?? string.Empty;
            DateOfBirth = visitor.DateOfBirth;
            MembershipCardNumber = visitor.MembershipCardNumber?.ToString() ?? string.Empty;
            CurrentMembershipYearBinding = Math.Max(0, visitor.CurrentMembershipYear - 1);
            StatusBinding = visitor.Status == Core.Models.Enums.MemberStatus.R ? 0 : 1;
            Address = new AddressDTO(visitor.Address ?? new Address());
        }

        public Visitor ToVisitor()
        {
            Visitor visitor = new Visitor
            {
                Id = Id,
                Name = Name,
                Surname = Surname,
                DateOfBirth = DateOfBirth,
                Address = Address.ToAddress(),
                Phone = Phone,
                Email = Email,
                MembershipCardNumber = MembershipCardNumber,
                CurrentMembershipYear = CurrentMembershipYearBinding + 1,
                Status = StatusBinding == 0 ? MemberStatus.R : MemberStatus.V,
                AverageRating = 0,
                BoughtBooks = new List<int>(),
                Wishlist = new List<int>()
            };
            return visitor;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static readonly Regex _nameRegex = new(@"^\p{Lu}\p{Ll}{2,}$");
        private static readonly Regex _mailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        private static readonly Regex _indexRegex = new(@"^[A-Za-z]{1,3}[-/]\d{1,3}[-/]\d{4}$");

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
                nameof(MembershipCardNumber) => string.IsNullOrWhiteSpace(MembershipCardNumber) || !_indexRegex.IsMatch(MembershipCardNumber)
                    ? "Index format: SK-123-2021 or SK-123-2021"
                    : string.Empty,
                _ => string.Empty
            };

        public string IsValid =>
            new[] { nameof(Name), nameof(Surname), nameof(Email), nameof(Phone), nameof(DateOfBirth), nameof(MembershipCardNumber) }
                .Select(p => this[p])
                .FirstOrDefault(e => !string.IsNullOrEmpty(e)) ?? string.Empty;
    }
}
