using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ClassicalMusicCSharp.Converters
{
    public class NotBoolToSelectionMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool v = System.Convert.ToBoolean(value);
            return v ? ListViewSelectionMode.Single : ListViewSelectionMode.Multiple;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
