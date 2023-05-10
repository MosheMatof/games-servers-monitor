using System.Globalization;
using System.Windows.Data;

namespace WPFClient.Converters;

    /// <summary>
    /// A converter that takes a boolean value and returns the inverse boolean value.
    /// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        else
        {
            throw new ArgumentException("The value must be a boolean.", nameof(value));
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        else
        {
            throw new ArgumentException("The value must be a boolean.", nameof(value));
        }
    }
}
