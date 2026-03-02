using BookFair.Core.Models;
using BookFair.Core.Models.Enums;
using BookFair.Core.Services;

namespace BookFair.Core.Controllers
{
    public class VisitorController
    {
        private readonly VisitorService _visitorService;

        public VisitorController(VisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        public void AddVisitor(Visitor newVisitor)
        {
            _visitorService.AddVisitor(newVisitor);
        }

        public void AddVisitor()
        {
            System.Console.WriteLine("\n--- Dodavanje posetioca ---");

            System.Console.Write("Ime: ");
            string name = System.Console.ReadLine() ?? "";

            System.Console.Write("Prezime: ");
            string surname = System.Console.ReadLine() ?? "";

            System.Console.Write("Datum rodjenja (YYYY-MM-DD): ");
            DateTime dateOfBirth;
            while (!DateTime.TryParse(System.Console.ReadLine(), out dateOfBirth))
            {
                System.Console.Write("Nevalidan datum. Pokusajte ponovo (YYYY-MM-DD): ");
            }

            System.Console.Write("Adresa(Street,Number,City,Country): ");
            string address = System.Console.ReadLine() ?? "";

            System.Console.Write("Telefon: ");
            string phone = System.Console.ReadLine() ?? "";

            System.Console.Write("Email: ");
            string email = System.Console.ReadLine() ?? "";

            System.Console.Write("Broj clanske karte: ");
            string membershipCard = System.Console.ReadLine() ?? "";

            System.Console.Write("Godina clanstva: ");
            int membershipYear;
            while (!int.TryParse(System.Console.ReadLine(), out membershipYear))
            {
                System.Console.Write("Nevalidan broj. Pokusajte ponovo: ");
            }

            var visitor = new Visitor
            {
                Name = name,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                Address = Address.Parse(address),
                Phone = phone,
                Email = email,
                MembershipCardNumber = membershipCard,
                CurrentMembershipYear = membershipYear,
                Status = MemberStatus.R,
                AverageRating = 0,
                BoughtBooks = new List<int> { },
                Wishlist = new List<int> { }
            };

            var addedVisitor = _visitorService.AddVisitor(visitor);
            System.Console.WriteLine($"\nPosetilac uspesno dodat! ID: {addedVisitor.Id}");
        }

        public List<Visitor> ViewAllVisitors()
        {
            System.Console.WriteLine("\n--- Svi posetioci ---");
            var visitors = _visitorService.GetAllVisitors();

            if (visitors.Count == 0)
            {
                System.Console.WriteLine("Nema posetioca u sistemu.");
            }

            return visitors;
        }

        public List<Visitor> GetAllVisitors()
        {
            return _visitorService.GetAllVisitors();
        }

        public void UpdateVisitor()
        {
            System.Console.WriteLine("\n--- Izmena posetioca ---");
            System.Console.Write("Unesite ID posetioca: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var visitor = _visitorService.GetVisitorById(id);
            if (visitor == null)
            {
                System.Console.WriteLine("Posetilac sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Trenutni podaci: {visitor}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Ime [{visitor.Name}]: ");
            string name = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) visitor.Name = name;

            System.Console.Write($"Prezime [{visitor.Surname}]: ");
            string surname = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(surname)) visitor.Surname = surname;

            System.Console.Write($"Adresa [{visitor.Address}]: ");
            string address = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address)) visitor.Address = Address.Parse(address);

            System.Console.Write($"Telefon [{visitor.Phone}]: ");
            string phone = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone)) visitor.Phone = phone;

            System.Console.Write($"Email [{visitor.Email}]: ");
            string email = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) visitor.Email = email;

            _visitorService.UpdateVisitor(visitor);
            System.Console.WriteLine("\nPosetilac uspesno izmenjen!");
        }

        public void UpdateVisitor(Visitor updatedVisitor)
        {
            _visitorService.UpdateVisitor(updatedVisitor);
        }

        public void DeleteVisitor(int id)
        {
            var visitor = _visitorService.GetVisitorById(id);
            if (visitor == null)
            {
                System.Console.WriteLine("Posetilac sa tim ID-jem ne postoji.");
                return;
            }
            _visitorService.RemoveVisitor(id);
        }

        public void DeleteVisitor()
        {
            System.Console.WriteLine("\n--- Brisanje posetioca ---");
            System.Console.Write("Unesite ID posetioca: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var visitor = _visitorService.GetVisitorById(id);
            if (visitor == null)
            {
                System.Console.WriteLine("Posetilac sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete: {visitor.Name} {visitor.Surname}? (da/ne)");
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _visitorService.RemoveVisitor(id);
                System.Console.WriteLine("Posetilac uspesno obrisan!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }

        public void AddBookToWishlist()
        {
            System.Console.WriteLine("\n--- Dodavanje knjige na listu zelja ---");
            System.Console.Write("Unesite ID posetioca: ");
            int visitorId;
            while (!int.TryParse(System.Console.ReadLine(), out visitorId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            System.Console.Write("Unesite ID knjige: ");
            int bookId;
            while (!int.TryParse(System.Console.ReadLine(), out bookId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            try
            {
                _visitorService.AddBookToWishlist(visitorId, bookId);
                System.Console.WriteLine("Knjiga uspesno dodata na listu zelja!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Greska: {ex.Message}");
            }
        }
    }
}
