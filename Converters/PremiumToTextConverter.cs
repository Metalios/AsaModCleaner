using System.Globalization;
using System.Windows.Data;

namespace AsaModCleaner.Converters
{
    public class PremiumToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isPremium)
            {
                return isPremium ? "Premium Mod" : "Free Mod";
            }

            // Handle null or invalid input
            return "Free Mod";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
