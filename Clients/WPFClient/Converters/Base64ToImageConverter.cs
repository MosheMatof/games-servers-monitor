using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WPFClient.Converters;

public class Base64ToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string imageData = value as string;
        if (string.IsNullOrEmpty(imageData)) return null;

        byte[] imageBytes = System.Convert.FromBase64String(imageData);
        using MemoryStream stream = new MemoryStream(imageBytes);
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

