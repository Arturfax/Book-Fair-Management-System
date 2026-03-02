using BookFair.Core.Controllers;
using BookFair.Core.DAO;
using BookFair.Core.Services;

namespace BookFair.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize DAOs
            var visitorDAO = new VisitorDAO();
            var bookDAO = new BookDAO();
            var publisherDAO = new PublisherDAO();
            var authorDAO = new AuthorDAO();
            var addressDAO = new AddressDAO();
            var buyingDAO = new BuyingDAO();

            // Initialize Services
            var bookService = new BookService(bookDAO);
            var visitorService = new VisitorService(visitorDAO, bookDAO);
            var publisherService = new PublisherService(publisherDAO);
            var authorService = new AuthorService(authorDAO);
            var addressService = new AddressService(addressDAO);
            var buyingService = new BuyingService(buyingDAO, visitorDAO);

            // Initialize Controllers
            var visitorController = new VisitorController(visitorService);
            var bookController = new BookController(bookService);
            var publisherController = new PublisherController(publisherService);
            var authorController = new AuthorController(authorService);
            var addressController = new AddressController(addressService);
            var buyingController = new BuyingController(buyingService);
            var menuController = new MenuController(visitorController, bookController, publisherController, authorController, addressController, buyingController);

            // Start the interactive menu
            menuController.ShowMainMenu();
        }
    }
}
