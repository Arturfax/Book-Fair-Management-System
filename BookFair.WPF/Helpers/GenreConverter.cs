using System;
using System.Globalization;
using System.Windows.Data;

namespace BookFair.WPF.Helpers
{
    public class GenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string genreName)
                return GenreLocalizer.GetLocalizedGenreName(genreName);
            return value ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
