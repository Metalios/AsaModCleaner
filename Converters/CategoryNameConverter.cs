using System.Globalization;
using System.Windows.Data;
using AsaModCleaner.Models;

namespace AsaModCleaner.Converters
{
    public class CategoryNameConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not List<ModCategory> categories || categories.Count == 0)
                return "Unknown";

            return categories.FirstOrDefault()?.Name ?? "Unknown";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}