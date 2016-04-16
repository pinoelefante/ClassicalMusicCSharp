using ClassicalMusicCSharp.Classes.FileManager;
using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using ClassicalMusicCSharp.Views.ContentDialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
            _composer = parameters["Composer"].ToString();
            string catName = string.Empty;
            string opName = parameters["Opera"].ToString();
            
            composer = OneClassicalHub.Instance.GetComposerByName(_composer);

            if (parameters.ContainsKey("Category"))
            {
                catName = parameters["Category"].ToString();
                Opera = composer.Categorie.Where(x => x.Nome.Equals(catName)).First().Opere.Where(x => x.Nome.Equals(opName)).First();
            }
            else
                Opera = composer.Opere.Where(x => x.Nome.Equals(opName)).First();
            
            IsSingleMode = true;
            return Task.CompletedTask;
        }
        private Compositore composer;
        private string _composer;
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

            PlayerPageVM.PlayTrack(track, _composer, Opera.Nome);
            Shell.Instance.ShowMessagePopup($"Playing {track.Titolo}");
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
        public void PlayAll(object sender, object e)
        {
            PlayerPageVM.AddTracks(Opera.Tracce, _composer, Opera.Nome, true);
            Shell.Instance.ShowMessagePopup($"Playing {Opera.Nome}");
        }

        public void PlaySelected(List<Traccia> sel)
        {
            if (sel.Count > 0)
            {
                PlayerPageVM.AddTracks(sel, _composer, Opera.Nome, true);
                Shell.Instance.ShowMessagePopup($"Playing selected tracks from {Opera.Nome}");
            }
        }

        public void PlaylistAll(Playlist list)
        {
            if (list == PlaylistManager.Instance.GetPlayingNowPlaylist())
            {
                PlayerPageVM.AddTracks(Opera.Tracce, _composer, Opera.Nome);
                Shell.Instance.ShowMessagePopup($"Added to Now Playing");
            }
            else
            {
                List<PlaylistTrack> listAdd = new List<PlaylistTrack>(Opera.Tracce.Count);
                foreach (var t in Opera.Tracce)
                {
                    PlaylistTrack track = new PlaylistTrack()
                    {
                        Album = Opera.Nome,
                        Composer = _composer,
                        Track = t.Titolo,
                        Link = t.Link
                    };
                    listAdd.Add(track);
                }
                PlaylistManager.Instance.AddTrackToPlaylist(listAdd, list, true);
                Shell.Instance.ShowMessagePopup($"Added to {list.Name}");
            }
        }
        public void PlaylistSelected(List<Traccia> tracks, Playlist playlist)
        {
            if (tracks.Count == 0)
                return;

            if (playlist == PlaylistManager.Instance.GetPlayingNowPlaylist())
            {
                PlayerPageVM.AddTracks(tracks, _composer, Opera.Nome);
                Shell.Instance.ShowMessagePopup($"Added to Now Playing");
            }
            else
            {
                foreach (var t in tracks)
                {
                    PlaylistTrack track = new PlaylistTrack()
                    {
                        Album = Opera.Nome,
                        Composer = _composer,
                        Track = t.Titolo,
                        Link = t.Link
                    };
                    PlaylistManager.Instance.AddTrackToPlaylist(track, playlist, false);
                }
                Shell.Instance.ShowMessagePopup($"Added to {playlist.Name}");
                playlist.SaveJson();
            }
        }
        public void SelezioneMultipla(object s, object e)
        {
            IsSingleMode = false;
        }
        public void SelezioneSingola(object s, object e)
        {
            IsSingleMode = true;
        }
        public ObservableCollection<Playlist> Playlists { get; } = PlaylistManager.Instance.Playlists;
        public async void Download(List<Traccia> tracks)
        {
            foreach (var item in tracks)
            {
                DownloadManager.Instance.DownloadTrack(new Models.DownloadItem()
                {
                    ComposerName = composer.Nome,
                    OperaName = _opera.Nome,
                    TrackName = item.Titolo,
                    Url = item.Link
                });
            }
        }
        public void DownloadAll(object sender, object e)
        {
            Download(Opera.Tracce);
        }
    }
}
