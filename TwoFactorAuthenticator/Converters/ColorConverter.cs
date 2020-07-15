using System;
using System.Globalization;
using System.Windows.Data;

namespace TwoFactorAuthenticator.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var secondsLeft = (int)value;

            if (secondsLeft <= 3)
                return "Red";
            else if (secondsLeft <= 5)
                return "Orange";
            else if (secondsLeft <= 10)
                return "Yellow";
            else
                return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
