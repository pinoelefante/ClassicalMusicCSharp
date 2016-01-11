using ClassicalMusicCSharp.OneClassical;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            IsSingleMode = true;
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
            if (!IsSingleMode)
                return;

            if (sender is ListView)
            {
                ListView list = sender as ListView;
                Traccia track = list.SelectedItem as Traccia;
                //int index = Opera.Tracce.IndexOf(track);
                Debug.WriteLine("Adding " + track.Titolo);
                PlayerPageVM.PlayTrack(track);
            }
        }
        private bool _isSingleMode;
        public bool IsSingleMode
        {
            get
            {
                return _isSingleMode;
            }
            set
            {
                Set<bool>(ref _isSingleMode, value);
            }
        }
        public void addToPlaylist(Traccia track)
        {
            PlayerPageVM.AddTrack(track);
        }
        public void PlayAll(object sender, object e)
        {
            PlayerPageVM.AddTracks(Opera.Tracce, true);
        }

        public void PlaySelected(List<Traccia> sel)
        {
            PlayerPageVM.AddTracks(sel, true);
        }

        public void PlaylistAll(object s, object e)
        {
            PlayerPageVM.AddTracks(Opera.Tracce);
        }
        public void PlaylistSelected(List<Traccia> l)
        {
            PlayerPageVM.AddTracks(l);
        }
        public void SelezioneMultipla(object s, object e)
        {
            IsSingleMode = false;
        }
        public void SelezioneSingola(object s, object e)
        {
            IsSingleMode = true;
        }
    }
}
