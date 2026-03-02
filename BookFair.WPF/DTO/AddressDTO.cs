using BookFair.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFair.WPF.DTO
{
    public class AddressDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _street = string.Empty;
        public string Street
        {
            get => _street;
            set { if (value != _street) { _street = value ?? string.Empty; OnPropertyChanged(nameof(Street)); } }
        }

        private string _number = string.Empty;
        public string Number
        {
            get => _number;
            set { if (value != _number) { _number = value ?? string.Empty; OnPropertyChanged(nameof(Number)); } }
        }

        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set { if (value != _city) { _city = value ?? string.Empty; OnPropertyChanged(nameof(City)); } }
        }

        private string _country = string.Empty;
        public string Country
        {
            get => _country;
            set { if (value != _country) { _country = value ?? string.Empty; OnPropertyChanged(nameof(Country)); } }
        }

        public AddressDTO() { }

        public AddressDTO(Address a)
        {
            if (a == null) return;
            Id = a.Id;
            Street = a.Street ?? string.Empty;
            Number = a.Number ?? string.Empty;
            City = a.City ?? string.Empty;
            Country = a.Country ?? string.Empty;
        }

        public Address ToAddress() => new Address
        {
            Id = Id,
            Street = Street ?? string.Empty,
            Number = Number ?? string.Empty,
            City = City ?? string.Empty,
            Country = Country ?? string.Empty
        };

        // ---- Validation ----
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Street):
                        return string.IsNullOrWhiteSpace(Street) ? "Ulica je obavezna." : string.Empty;

                    case nameof(Number):
                        if (string.IsNullOrWhiteSpace(Number)) return "Broj je obavezan.";
                        // ako želiš striktno broj: otkomentariši sledeću liniju
                        // if (!int.TryParse(Number, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)) return "Broj mora biti numerički.";
                        return string.Empty;

                    case nameof(City):
                        return string.IsNullOrWhiteSpace(City) ? "Grad je obavezan." : string.Empty;

                    case nameof(Country):
                        return string.IsNullOrWhiteSpace(Country) ? "Država je obavezna." : string.Empty;

                    default:
                        return string.Empty;
                }
            }
        }

        public string IsValid
        {
            get
            {
                foreach (var p in new[] { nameof(Street), nameof(Number), nameof(City), nameof(Country) })
                {
                    var err = this[p];
                    if (!string.IsNullOrEmpty(err)) return err;
                }
                return string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
