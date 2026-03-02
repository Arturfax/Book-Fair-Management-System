using BookFair.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BookFair.WPF.DTO
{
    public class PublisherDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _code = string.Empty;
        public string Code
        {
            get => _code;
            set { if (_code != value) { _code = value ?? string.Empty; OnPropertyChanged(nameof(Code)); } }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(nameof(Name)); } }
        }

        public int Id { get; set; }

        public PublisherDTO() { }

        public PublisherDTO(Publisher publisher)
        {
            if (publisher == null) return;
            Id = publisher.Id;
            Code = publisher.Code ?? string.Empty;
            Name = publisher.Name ?? string.Empty;
        }

        public Publisher ToPublisher(Publisher? existing = null)
        {
            return new Publisher
            {
                Id = Id,
                Code = Code,
                Name = Name,
                HeadOfPublisherId = existing?.HeadOfPublisherId ?? 0,
                AuthorIds = existing?.AuthorIds ?? new List<int>(),
                BookIds = existing?.BookIds ?? new List<int>()
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string Error => string.Empty;

        public string this[string columnName] =>
            columnName switch
            {
                nameof(Code) => string.IsNullOrWhiteSpace(Code) ? "Code is required." : string.Empty,
                nameof(Name) => string.IsNullOrWhiteSpace(Name) ? "Name is required." : string.Empty,
                _ => string.Empty
            };

        public string IsValid =>
            new[] { nameof(Code), nameof(Name) }
                .Select(p => this[p])
                .FirstOrDefault(e => !string.IsNullOrEmpty(e)) ?? string.Empty;
    }
}
