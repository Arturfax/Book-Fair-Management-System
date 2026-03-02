using BookFair.Core.Models;
using BookFair.Core.Services;

namespace BookFair.Core.Controllers
{
    public class PublisherController
    {
        private readonly PublisherService _publisherService;

        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        public Publisher AddPublisher(Publisher publisher)
        {
            return _publisherService.AddPublisher(publisher);
        }

        public void AddPublisher()
        {
            System.Console.WriteLine("\n--- Dodavanje izdavaca ---");

            System.Console.Write("Sifra: ");
            string code = System.Console.ReadLine() ?? "";

            System.Console.Write("Naziv: ");
            string name = System.Console.ReadLine() ?? "";

            System.Console.Write("ID rukovodioca: ");
            int headId;
            while (!int.TryParse(System.Console.ReadLine(), out headId))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var publisher = new Publisher
            {
                Code = code,
                Name = name,
                HeadOfPublisherId = headId,
                AuthorIds = new List<int> { },
                BookIds = new List<int> { }
            };

            var addedPublisher = _publisherService.AddPublisher(publisher);
            System.Console.WriteLine($"\nIzdavac uspesno dodat! ID: {addedPublisher.Id}");
        }

        public List<Publisher> ViewAllPublishers()
        {
            System.Console.WriteLine("\n--- Svi izdavaci ---");
            var publishers = _publisherService.GetAllPublishers();

            if (publishers.Count == 0)
            {
                System.Console.WriteLine("Nema izdavaca u sistemu.");
            }

            return publishers;
        }

        public List<Publisher> GetAllPublishers()
        {
            return _publisherService.GetAllPublishers();
        }

        public void UpdatePublisher()
        {
            System.Console.WriteLine("\n--- Izmena izdavaca ---");
            System.Console.Write("Unesite ID izdavaca: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var publisher = _publisherService.GetPublisherById(id);
            if (publisher == null)
            {
                System.Console.WriteLine("Izdavac sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Trenutni podaci: {publisher}");
            System.Console.WriteLine("\nOstavite prazno da zadrzite trenutnu vrednost.");

            System.Console.Write($"Sifra [{publisher.Code}]: ");
            string code = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(code)) publisher.Code = code;

            System.Console.Write($"Naziv [{publisher.Name}]: ");
            string name = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) publisher.Name = name;

            System.Console.Write($"ID rukovodioca [{publisher.HeadOfPublisherId}]: ");
            string headIdInput = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(headIdInput) && int.TryParse(headIdInput, out int headId))
            {
                publisher.HeadOfPublisherId = headId;
            }

            _publisherService.UpdatePublisher(publisher);
            System.Console.WriteLine("\nIzdavac uspesno izmenjen!");
        }

        public void UpdatePublisher(Publisher updatedPublisher)
        {
            _publisherService.UpdatePublisher(updatedPublisher);
        }

        public void DeletePublisher(int id)
        {
            _publisherService.RemovePublisher(id);
        }

        public void DeletePublisher()
        {
            System.Console.WriteLine("\n--- Brisanje izdavaca ---");
            System.Console.Write("Unesite ID izdavaca: ");
            int id;
            while (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.Write("Nevalidan ID. Pokusajte ponovo: ");
            }

            var publisher = _publisherService.GetPublisherById(id);
            if (publisher == null)
            {
                System.Console.WriteLine("Izdavac sa tim ID-jem ne postoji.");
                return;
            }

            System.Console.WriteLine($"Da li ste sigurni da zelite da obrisete: {publisher.Name}? (da/ne)");
            string confirmation = System.Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "da")
            {
                _publisherService.RemovePublisher(id);
                System.Console.WriteLine("Izdavac uspesno obrisan!");
            }
            else
            {
                System.Console.WriteLine("Brisanje otkazano.");
            }
        }
    }
}
