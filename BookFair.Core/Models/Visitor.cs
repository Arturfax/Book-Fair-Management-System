using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BookFair.Core.Models.Enums;
using BookFair.Core.Utils;

namespace BookFair.Core.Models
{
    public class Visitor : ObservableModel, Serializable
    {
        private int _id;
        private string _name;
        private string _surname;
        private DateTime _dateOfBirth;
        private Address _address;
        private string _phone;
        private string _email;
        private string _membershipCardNumber;
        private int _currentMembershipYear;
        private MemberStatus _status;
        private double _averageRating;
        private List<int> _boughtBooks;
        private List<int> _wishlist;

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

        public string MembershipCardNumber
        {
            get => _membershipCardNumber;
            set => SetProperty(ref _membershipCardNumber, value);
        }

        public int CurrentMembershipYear
        {
            get => _currentMembershipYear;
            set => SetProperty(ref _currentMembershipYear, value);
        }

        public MemberStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public double AverageRating
        {
            get => _averageRating;
            set => SetProperty(ref _averageRating, value);
        }

        public List<int> BoughtBooks
        {
            get => _boughtBooks;
            set => SetProperty(ref _boughtBooks, value);
        }

        public List<int> Wishlist
        {
            get => _wishlist;
            set => SetProperty(ref _wishlist, value);
        }

            public Visitor()
            {
            }

            public Visitor(string name, string surname, DateTime dateOfBirth, Address address, string phone, string email, string membershipCardNumber, int currentMembershipYear, MemberStatus status, double averageRating, List<int> boughtBooks, List<int> wishlist)
            {
                Name = name;
                Surname = surname;
                DateOfBirth = dateOfBirth;
                Address = address;
                Phone = phone;
                Email = email;
                MembershipCardNumber = membershipCardNumber;
                CurrentMembershipYear = currentMembershipYear;
                Status = status;
                AverageRating = averageRating;
                BoughtBooks = boughtBooks;
                Wishlist = wishlist;
            }
            public override string ToString()
            {
                string BoughtBooksPrint = BoughtBooks.Count > 0 ? string.Join(", ", BoughtBooks) : "nema kupljenih knjiga";
                string WishlistPrint = Wishlist.Count > 0 ? string.Join(", ", Wishlist) : "nema knjiga na listi želja";

                return $"{Name} {Surname} | {Status} | Clanska karta: {MembershipCardNumber} | Godina: {CurrentMembershipYear} | Prosecna ocena: {AverageRating:F2} | Kupljene knjige: {BoughtBooksPrint} | Lista zelja: {WishlistPrint}";
            }

            public string[] ToCSV()
            {
                return new string[]
                {
                Id.ToString(),
                Name,
                Surname,
                DateOfBirth.ToString("yyyy-MM-dd"),
                Address.ToString(),
                Phone,
                Email,
                MembershipCardNumber,
                CurrentMembershipYear.ToString(),
                Status.ToString(),
                AverageRating.ToString("F2"),
                string.Join(";", BoughtBooks),
                string.Join(";", Wishlist)
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
                MembershipCardNumber = values[7];
                CurrentMembershipYear = int.Parse(values[8]);
                Status = (MemberStatus)Enum.Parse(typeof(MemberStatus), values[9]);
                AverageRating = double.Parse(values[10]);
                BoughtBooks = string.IsNullOrEmpty(values[11]) ? new List<int>() : values[11].Split(';').Select(int.Parse).ToList();
                Wishlist = string.IsNullOrEmpty(values[12]) ? new List<int>() : values[12].Split(';').Select(int.Parse).ToList();
            }

        }
    }
