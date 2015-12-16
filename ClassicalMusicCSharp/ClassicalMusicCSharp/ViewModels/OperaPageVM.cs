using ClassicalMusicCSharp.OneClassical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class OperaPageVM : Mvvm.ViewModelBase
    {
        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Opera = parameter as Opera;
        }
        private Opera _opera;
        public Opera Opera
        {
            get
            {
                return _opera;
            }
            set
            {
                Set<Opera>(ref _opera, value);
            }
        }
        public void onTrack(object sender, object e)
        {
            if(sender is ListView)
            {
                ListView list = sender as ListView;
                Traccia track = list.SelectedItem as Traccia;
                new MessageDialog(track.Titolo).ShowAsync();
            }
        }
    }
}
