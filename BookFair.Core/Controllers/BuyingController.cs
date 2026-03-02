using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.Core.Controllers
{
    public class BuyingController
    {
        private readonly BuyingService _buyingService;

        public BuyingController(BuyingService buyingService)
        {
            _buyingService = buyingService;
        }

        public void AddBuying()
        {
            System.Console.WriteLine("\n--- Dodavanje kupovine ---");

            System.Console.Write("ID posetioca: ");
            int visitorId;
            while (!int.TryParse(System.Console.ReadLine(), out visitorId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            System.Console.Write("ID knjige: ");
            int bookId;
            while (!int.TryParse(System.Console.ReadLine(), out bookId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            System.Console.Write("Datum kupovine (YYYY-MM-DD): ");
            DateTime buyingDate;
            while (!DateTime.TryParse(System.Console.ReadLine(), out buyingDate))
            {
                System.Console.Write("Nevalidan datum. Pokusajte ponovo (YYYY-MM-DD): ");
            }

            System.Console.Write("Ocena (1-5): ");
            int rating;
            while (!int.TryParse(System.Console.ReadLine(), out rating) || rating < 1 || rating > 5)
            {
                System.Console.Write("Nevalidna ocena. Unesite broj od 1 do 5: ");
            }

            System.Console.Write("Komentar: ");
            string comment = System.Console.ReadLine() ?? "";

            var buying = new Buying
            {
                VisitorId = visitorId,
                BookId = bookId,
                BuyingDate = buyingDate,
                Rating = rating,
                Comment = comment
            };

            var addedBuying = _buyingService.AddBuying(buying);
            System.Console.WriteLine($"\nKupovina uspesno dodata! ID: {addedBuying.Id}");
        }

       
        public Buying AddBuying(Buying buying)
        {
            return _buyingService.AddBuying(buying);
        }

        public void ViewAllBuyings()
        {
            System.Console.WriteLine("\n--- Sve kupovine ---");
            var buyings = _buyingService.GetAllBuying();

            if (buyings.Count == 0)
            {
                System.Console.WriteLine("Nema kupovina u sistemu.");
                return;
            }

            foreach (var buying in buyings)
            {
                System.Console.WriteLine(buying);
            }
        }

        public void UpdateBuying()
        {
            System.Console.WriteLine("\n--- Izmena kupovine ---");
            System.Console.Write("Unesite ID kupovine: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var buying = _buyingService.GetBuyingById(id);
            if (buying == null)
            {
                System.Console.WriteLine("Kupovina sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Trenutni podaci: {buying}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Ocena (1-5) [{buying.Rating}]: ");
            string ratingInput = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ratingInput) && int.TryParse(ratingInput, out int rating) && rating >= 1 && rating <= 5)
            {
                buying.Rating = rating;
            }

            System.Console.Write($"Komentar [{buying.Comment}]: ");
            string comment = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(comment)) buying.Comment = comment;

            _buyingService.UpdateBuying(buying);
            System.Console.WriteLine("\nKupovina uspesno izmenjena!");
        }

        public void DeleteBuying()
        {
            System.Console.WriteLine("\n--- Brisanje kupovine ---");
            System.Console.Write("Unesite ID kupovine: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var buying = _buyingService.GetBuyingById(id);
            if (buying == null)
            {
                System.Console.WriteLine("Kupovina sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete ovu kupovinu? (da/ne)");
            System.Console.WriteLine(buying);
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _buyingService.RemoveBuying(id);
                System.Console.WriteLine("Kupovina uspesno obrisana!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }

        public void ViewBuyingsByVisitorAndBook()
        {
            System.Console.WriteLine("\n--- Pretraga kupovina po posetiocu i knjizi ---");

            System.Console.Write("ID posetioca: ");
            int visitorId;
            while (!int.TryParse(System.Console.ReadLine(), out visitorId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            System.Console.Write("ID knjige: ");
            int bookId;
            while (!int.TryParse(System.Console.ReadLine(), out bookId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var buyings = _buyingService.GetBuyingByBookAndVisitor(visitorId, bookId);

            if (buyings.Count == 0)
            {
                System.Console.WriteLine("Nije pronadjena nijedna kupovina za zadatu kombinaciju posetioca i knjige.");
                return;
            }

            System.Console.WriteLine($"\nPronadjeno {buyings.Count} kupovina:");
            foreach (var buying in buyings)
            {
                System.Console.WriteLine(buying);
            }


        }

        public List<Buying> GetBuyingsByVisitor(int visitorId)
        {
            return _buyingService.GetBuyingsByVisitor(visitorId);
        }

        public Buying? RemoveBuyingById(int id)
        {
            return _buyingService.RemoveBuying(id);
        }
    }
}
