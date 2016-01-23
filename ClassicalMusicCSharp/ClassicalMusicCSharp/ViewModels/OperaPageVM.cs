using ClassicalMusicCSharp.OneClassical;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class OperaPageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            ValueSet parameters = parameter as ValueSet;
            _composer = parameters["Composer"] as Compositore;
            Opera = parameters["Opera"] as Opera;
            IsSingleMode = true;
            return Task.CompletedTask;
        }
        private Compositore _composer;
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
        public void onTrack(object sender, ItemClickEventArgs e)
        {
            if (!IsSingleMode)
                return;

            Traccia track = e.ClickedItem as Traccia;
            if (track == null)
                return;

            PlayerPageVM.PlayTrack(track, _composer.Nome, Opera.Nome);
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
            PlayerPageVM.AddTrack(track, _composer.Nome, Opera.Nome);
        }
        public void PlayAll(object sender, object e)
        {
            PlayerPageVM.AddTracks(Opera.Tracce, _composer.Nome, Opera.Nome, true);
        }

        public void PlaySelected(List<Traccia> sel)
        {
            PlayerPageVM.AddTracks(sel, _composer.Nome, Opera.Nome, true);
        }

        public void PlaylistAll(object s, object e)
        {
            PlayerPageVM.AddTracks(Opera.Tracce, _composer.Nome, Opera.Nome);
        }
        public void PlaylistSelected(List<Traccia> l)
        {
            PlayerPageVM.AddTracks(l, _composer.Nome, Opera.Nome);
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
