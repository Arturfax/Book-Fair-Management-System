using BookFair.Core.Models.Enums;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace BookFair.WPF.Helpers
{
    public static class GenreLocalizer
    {
        private static ResourceManager? _resourceManager;

        private static ResourceManager ResourceManager
        {
            get
            {
                _resourceManager ??= new ResourceManager("BookFair.WPF.Properties.Resources", Assembly.GetExecutingAssembly());
                return _resourceManager;
            }
        }

        public static string GetLocalizedGenreName(string genreName)
        {
            var resourceKey = $"Genre_{genreName}";
            var cultureInfo = CultureInfo.CurrentUICulture;
            var localizedValue = ResourceManager.GetString(resourceKey, cultureInfo);
            return localizedValue ?? genreName;
        }

        public static List<GenreDisplayItem> GetLocalizedGenres()
        {
            var genres = new List<GenreDisplayItem>();

            foreach (var genre in Enum.GetValues(typeof(Genre)))
            {
                var genreName = genre.ToString();
                genres.Add(new GenreDisplayItem
                {
                    Value = genreName,
                    DisplayName = GetLocalizedGenreName(genreName)
                });
            }

            return genres;
        }

        public static void RefreshResourceManager()
        {
            _resourceManager = null;
        }
    }

    public class GenreDisplayItem
    {
        public string Value { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }
}
