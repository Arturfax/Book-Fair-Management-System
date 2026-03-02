using BookFair.Core.Controllers;
using BookFair.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BookFair.WPF.Views.PublisherView
{
    public partial class PublisherHeads : Window
    {
        private readonly PublisherController _publisherController;
        private readonly AuthorController _authorController;

        public ObservableCollection<Publisher> Publishers { get; } = new();
        public ObservableCollection<AuthorOption> Authors { get; } = new();

        private Publisher? _selectedPublisher;
        private AuthorOption? _selectedAuthor;

        public PublisherHeads(PublisherController publisherController, AuthorController authorController)
        {
            InitializeComponent();

            _publisherController = publisherController ?? throw new System.ArgumentNullException(nameof(publisherController));
            _authorController = authorController ?? throw new System.ArgumentNullException(nameof(authorController));

            PublishersListBox.ItemsSource = Publishers;
            AuthorsListBox.ItemsSource = Authors;

            Loaded += (_, __) => LoadPublishers();
        }

        private void LoadPublishers()
        {
            Publishers.Clear();
            var publishers = _publisherController.GetAllPublishers();
            if (publishers != null)
            {
                foreach (var pub in publishers)
                {
                    Publishers.Add(pub);
                }
            }
        }

        private void PublishersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedPublisher = PublishersListBox.SelectedItem as Publisher;
            LoadAuthorsForPublisher();
        }

        private void LoadAuthorsForPublisher()
        {
            Authors.Clear();

            if (_selectedPublisher == null) return;

            // Get all authors who have published with this publisher
            var allAuthors = _authorController.GetAllAuthors();
            if (allAuthors == null) return;

            var candidateAuthors = allAuthors
                .Where(a => a.YearsOfExperience >= 5)
                .OrderBy(a => a.Name)
                .ToList();

            foreach (var author in candidateAuthors)
            {
                Authors.Add(new AuthorOption
                {
                    Id = author.Id,
                    DisplayName = $"{author.Name} {author.Surname} ({author.YearsOfExperience} yrs)",
                    YearsOfExperience = author.YearsOfExperience
                });
            }
        }

        private void AuthorsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedAuthor = AuthorsListBox.SelectedItem as AuthorOption;
        }

        private void SetAsHead_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPublisher == null)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectPublisherHeads, Properties.Resources.Msg_ValidationTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedAuthor == null)
            {
                MessageBox.Show(Properties.Resources.Msg_SelectAuthorHeads, Properties.Resources.Msg_ValidationTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verify author meets requirements
            if (_selectedAuthor.YearsOfExperience < 5)
            {
                MessageBox.Show(string.Format(Properties.Resources.Msg_AuthorExperienceError, _selectedAuthor.YearsOfExperience), Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Set as head and save
            _selectedPublisher.HeadOfPublisherId = _selectedAuthor.Id;
            _publisherController.UpdatePublisher(_selectedPublisher);

            MessageBox.Show(string.Format(Properties.Resources.Msg_PublisherHeadSetSuccess, _selectedAuthor.DisplayName, _selectedPublisher.Name), Properties.Resources.Msg_SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh display
            LoadPublishers();
            Authors.Clear();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class AuthorOption
        {
            public int Id { get; set; }
            public string DisplayName { get; set; } = "";
            public int YearsOfExperience { get; set; }
        }
    }
}
