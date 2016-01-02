using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ClassicalMusicCSharp.Converters
{
    class BoolToSelectionMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool v = System.Convert.ToBoolean(value);
            return v ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
