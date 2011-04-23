using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace PatchService.View
{
    public class NameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (path != null)
            {
                BitmapImage image = new BitmapImage(new Uri(path));
                return image;
            }
            else
                return null;
        }
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    // here you can return DrawingImage based on value that represents name field of your structure
        //    // just for example the piece of your code:
        //    if (value is string && !String.IsNullOrEmpty(value as string))
        //    {
        //        return SvgReader.Load(new MemoryStream(new System.Net.WebClient().DownloadData(value as string)));
        //    }
        //    else
        //    {
        //        // if value is null or not of string type
        //        return yourDefaultImage;
        //    }
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
