using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Template10.Services.NavigationService;
using ClassicalMusicCSharp.Views.ContentDialogs;
using ClassicalMusicCSharp.Classes.FileManager;

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlaylistViewerVM : Mvvm.ViewModelBase
    {
        private Playlist _playlist;
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            long idPlaylist = (long)parameter;
            Playlist = PlaylistManager.Instance.GetPlaylistById(idPlaylist);
            return Task.CompletedTask;
        }
        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }
        public Playlist Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                Set<Playlist>(ref _playlist, value);
            }
        }

        public void PlayTrack(object sender, ItemClickEventArgs e)
        {
            if (IsMultipleSelection)
                return;

            PlaylistTrack track = e.ClickedItem as PlaylistTrack;
            PlayerPageVM.PlayTrack(track);
        }
        public void PlayPlaylist(object sender, object e)
        {
            if (!Playlist.IsEmpty())
            {
                PlayerPageVM.CleanPlaylist();
                PlayerPageVM.ChangePlaylist(Playlist, true);
            }
        }
        public void SavePlaylist(object sender, object e)
        {
            Playlist.SaveJson();
        }
        public async void DeletePlaylist(object sender, object e)
        {
            Action deleteAction = () =>
            {
                PlaylistManager.Instance.DeletePlaylist(Playlist);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.Navigate(typeof(PlaylistPage), true);
                }
            };
            DeletePlaylistContentDialog dlg = new DeletePlaylistContentDialog(deleteAction);
            await dlg.ShowAsync();
        }
        private bool _multipleSel;
        public bool IsMultipleSelection
        {
            get
            {
                return _multipleSel;
            }
            set
            {
                Set<bool>(ref _multipleSel, value);
            }
        }
        public void DeleteTracks(List<PlaylistTrack> tracks)
        {
            foreach (var item in tracks)
            {
                Playlist.List.Remove(item);
            }
            Playlist.SaveJson();
        }
        public void ChangeSelectionMode(object s, object e)
        {
            IsMultipleSelection = !_multipleSel;
        }
        public async void Download(List<PlaylistTrack> tracks)
        {
            foreach (var item in tracks)
            {
                DownloadManager.Instance.DownloadTrack(new Models.DownloadItem()
                {
                    ComposerName = item.Album,
                    OperaName = item.Album,
                    TrackName = item.Track,
                    Url = item.Link
                });
            }
        }
        public void DownloadAll(object sender, object e)
        {
            Download(Playlist.List.ToList());
        }
    }
}
