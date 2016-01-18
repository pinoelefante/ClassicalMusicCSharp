using ClassicalMusicCSharp.Classes.Radio;
using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class RadioVM : Mvvm.ViewModelBase
    {
        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!RadioManager.IsLoaded)
                await RadioManager.LoadRadio();
            RadioList = RadioManager.Radios.radio;
        }
        private List<RadioWrapper.Radio> _list;
        public List<RadioWrapper.Radio> RadioList
        {
            get
            {
                return _list;
            }
            set
            {
                Set<List<RadioWrapper.Radio>>(ref _list, value);
            }
        }
        public void RadioSelected(object sender, ItemClickEventArgs e)
        {
            RadioWrapper.Radio radio = e.ClickedItem as RadioWrapper.Radio;
            Debug.WriteLine("Selezionata " + radio?.title);
            BackgroundMediaPlayer.SendMessageToBackground(new Windows.Foundation.Collections.ValueSet()
            {
                {"Command", "PlayRadio"},
                {"Title", radio.title },
                {"Link", radio.link }
            });
        }
    }
}
