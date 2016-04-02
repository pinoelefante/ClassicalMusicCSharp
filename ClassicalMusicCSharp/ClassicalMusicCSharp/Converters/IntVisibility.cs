using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ClassicalMusicCSharp.Converters
{
    class IntVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int v = System.Convert.ToInt32(value);
            if (v != 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
