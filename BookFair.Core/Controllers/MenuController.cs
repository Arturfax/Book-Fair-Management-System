using BookFair.Core.Models;

namespace BookFair.Core.Controllers
{
    public class MenuController
    {
        private readonly VisitorController _visitorController;
        private readonly BookController _bookController;
        private readonly PublisherController _publisherController;
        private readonly AuthorController _authorController;
        private readonly AddressController _addressController;
        private readonly BuyingController _buyingController;

        public MenuController(VisitorController visitorController, BookController bookController, PublisherController publisherController, AuthorController authorController, AddressController addressController, BuyingController buyingController)
        {
            _visitorController = visitorController;
            _bookController = bookController;
            _publisherController = publisherController;
            _authorController = authorController;
            _addressController = addressController;
            _buyingController = buyingController;
        }

        public void ShowMainMenu()
        {
            bool exit = false;

            while (!exit)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("        SAJAM KNJIGA - GLAVNI MENI      ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Upravljanje posetiocima");
                System.Console.WriteLine("2. Upravljanje knjigama");
                System.Console.WriteLine("3. Upravljanje izdavacima");
                System.Console.WriteLine("4. Upravljanje autorima");
                System.Console.WriteLine("5. Upravljanje adresama");
                System.Console.WriteLine("6. Upravljanje kupovinama");
                System.Console.WriteLine("0. Izlaz");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        ShowVisitorMenu();
                        break;
                    case "2":
                        ShowBookMenu();
                        break;
                    case "3":
                        ShowPublisherMenu();
                        break;
                    case "4":
                        ShowAuthorMenu();
                        break;
                    case "5":
                        ShowAddressMenu();
                        break;
                    case "6":
                        ShowBuyingMenu();
                        break;
                    case "0":
                        exit = true;
                        System.Console.WriteLine("\nHvala sto ste koristili aplikaciju!");
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowVisitorMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("         UPRAVLJANJE POSETIOCIMA        ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj posetioca");
                System.Console.WriteLine("2. Prikazi sve posetioce");
                System.Console.WriteLine("3. Izmeni posetioca");
                System.Console.WriteLine("4. Obrisi posetioca");
                System.Console.WriteLine("5. Dodaj knjige na listu zelja");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _visitorController.AddVisitor();
                        PauseScreen();
                        break;
                    case "2":
                        _visitorController.ViewAllVisitors();
                        PauseScreen();
                        break;
                    case "3":
                        _visitorController.UpdateVisitor();
                        PauseScreen();
                        break;
                    case "4":
                        _visitorController.DeleteVisitor();
                        PauseScreen();
                        break;
                    case "5":
                        _visitorController.AddBookToWishlist();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowBookMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("          UPRAVLJANJE KNJIGAMA          ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj knjigu");
                System.Console.WriteLine("2. Prikazi sve knjige");
                System.Console.WriteLine("3. Izmeni knjigu");
                System.Console.WriteLine("4. Obrisi knjigu");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _bookController.AddBook();
                        PauseScreen();
                        break;
                    case "2":
                        _bookController.ViewAllBooks();
                        PauseScreen();
                        break;
                    case "3":
                        _bookController.UpdateBook();
                        PauseScreen();
                        break;
                    case "4":
                        _bookController.DeleteBook();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowPublisherMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("         UPRAVLJANJE IZDAVACIMA         ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj izdavaca");
                System.Console.WriteLine("2. Prikazi sve izdavace");
                System.Console.WriteLine("3. Izmeni izdavaca");
                System.Console.WriteLine("4. Obrisi izdavaca");
                System.Console.WriteLine("5. Prikazi knjige izdavaca");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _publisherController.AddPublisher();
                        PauseScreen();
                        break;
                    case "2":
                        _publisherController.ViewAllPublishers();
                        PauseScreen();
                        break;
                    case "3":
                        _publisherController.UpdatePublisher();
                        PauseScreen();
                        break;
                    case "4":
                        _publisherController.DeletePublisher();
                        PauseScreen();
                        break;
                    case "5":
                        ShowBooksByPublisher();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowBooksByPublisher()
        {
            var allPubs = _publisherController.GetAllPublishers();
            if (allPubs == null || allPubs.Count == 0)
            {
                System.Console.WriteLine("Nema izdavaca u sistemu.");
                return;
            }

            System.Console.WriteLine("\n--- Izdavaci ---");
            foreach (var p in allPubs)
            {
                System.Console.WriteLine($"{p.Id}. {p.Name} ({p.Code})");
            }
            System.Console.Write("\nUnesite ID izdavaca: ");
            if (!int.TryParse(System.Console.ReadLine(), out int id))
            {
                System.Console.WriteLine("Nevalidan ID.");
                return;
            }

            var publisher = allPubs.FirstOrDefault(p => p.Id == id);
            if (publisher == null)
            {
                System.Console.WriteLine("Izdavac nije pronadjen.");
                return;
            }

            var allBooks = _bookController.GetAllBooks();
            if (allBooks == null) allBooks = new System.Collections.Generic.List<Book>();
            var books = allBooks.Where(b => (b.Publisher ?? "").Equals(publisher.Name, System.StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (books.Count == 0)
            {
                System.Console.WriteLine("Nema knjiga za odabranog izdavaca.");
            }
            else
            {
                System.Console.WriteLine($"\nKnjige izdavaca '{publisher.Name}':");
                foreach (var b in books)
                {
                    System.Console.WriteLine(b);
                }
            }
        }

        private void ShowAuthorMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("          UPRAVLJANJE AUTORIMA          ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj autora");
                System.Console.WriteLine("2. Prikazi sve autore");
                System.Console.WriteLine("3. Izmeni autora");
                System.Console.WriteLine("4. Obrisi autora");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _authorController.AddAuthor();
                        PauseScreen();
                        break;
                    case "2":
                        _authorController.ViewAllAuthors();
                        PauseScreen();
                        break;
                    case "3":
                        _authorController.UpdateAuthor();
                        PauseScreen();
                        break;
                    case "4":
                        _authorController.DeleteAuthor();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowAddressMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("          UPRAVLJANJE ADRESAMA          ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj adresu");
                System.Console.WriteLine("2. Prikazi sve adrese");
                System.Console.WriteLine("3. Izmeni adresu");
                System.Console.WriteLine("4. Obrisi adresu");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _addressController.AddAddress();
                        PauseScreen();
                        break;
                    case "2":
                        _addressController.ViewAllAddresses();
                        PauseScreen();
                        break;
                    case "3":
                        _addressController.UpdateAddress();
                        PauseScreen();
                        break;
                    case "4":
                        _addressController.DeleteAddress();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowBuyingMenu()
        {
            bool back = false;

            while (!back)
            {
                System.Console.Clear();
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("         UPRAVLJANJE KUPOVINAMA         ");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.WriteLine("1. Dodaj kupovinu");
                System.Console.WriteLine("2. Prikazi sve kupovine");
                System.Console.WriteLine("3. Izmeni kupovinu");
                System.Console.WriteLine("4. Obrisi kupovinu");
                System.Console.WriteLine("5. Pretrazi po posetiocu i knjizi");
                System.Console.WriteLine("0. Nazad");
                System.Console.WriteLine("═══════════════════════════════════════");
                System.Console.Write("\nIzaberite opciju: ");

                string choice = System.Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        _buyingController.AddBuying();
                        PauseScreen();
                        break;
                    case "2":
                        _buyingController.ViewAllBuyings();
                        PauseScreen();
                        break;
                    case "3":
                        _buyingController.UpdateBuying();
                        PauseScreen();
                        break;
                    case "4":
                        _buyingController.DeleteBuying();
                        PauseScreen();
                        break;
                    case "5":
                        _buyingController.ViewBuyingsByVisitorAndBook();
                        PauseScreen();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        System.Console.WriteLine("\nNevalidna opcija. Pritisnite bilo koji taster...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }

        private void PauseScreen()
        {
            System.Console.WriteLine("\nPritisnite bilo koji taster za nastavak...");
            System.Console.ReadKey();
        }
    }
}
