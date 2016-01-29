using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ClassicalMusicCSharp.Converters
{
    public class StringVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string val = (string)value;
            return string.IsNullOrEmpty(val) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
