using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Frontend.Converters
{
    // "Pizza King" -> Images/RestaurantLogos/pizzaking_logo.png
    public class FranchiseLogoConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string name)
                return null;

            var fileName = $"{name.Replace(" ", "").Replace("'", "").ToLowerInvariant()}_logo.png";

            try
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Images/RestaurantLogos/{fileName}"));
            }
            catch (IOException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}