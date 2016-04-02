using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ClassicalMusicCSharp.Converters
{
    class ComposerImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string composer = value.ToString().ToLower();
            var uri = new Uri($"ms-appx:///Assets/composers/square/{composer.Replace(" ", "_")}_square.jpg");
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
