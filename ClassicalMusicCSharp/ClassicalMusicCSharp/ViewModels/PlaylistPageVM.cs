using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.Views;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlaylistPageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            IsLoading = true;
            if (parameter != null)
            {
                bool cleanStack = (bool)parameter;
                NavigationService.ClearHistory();
                NavigationService.ClearCache();
            }
            Playlists = PlaylistManager.Instance.Playlists;
            IsLoading = false;
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        public ObservableCollection<Playlist> Playlists { get; private set; } 
        public async void AddPlaylist(object s, object e)
        {
            await CreateNewPlaylist();
        }
        public static async Task<bool> CreateNewPlaylist()
        {
            if (PlaylistManager.Instance.CanAddNewPlaylist())
            {
                AddPlaylistContentDialog dlg = new AddPlaylistContentDialog();
                await dlg.ShowAsync();
                if (dlg.Cancelled)
                    return false;
                return true;
            }
            else if (PlaylistManager.Instance.CanPurchase())
            {
                Action<bool> OnSucced = async (bool res) =>
                {
                    if (res)
                    {
                        await CreateNewPlaylist();
                    }
                };
                BuyPlaylistsContentDialog dlg = new BuyPlaylistsContentDialog();
                dlg.OnBuySucceded = OnSucced;
                Shell.SetBusy(true, "Wait please");
                await dlg.ShowAsync();
                Shell.SetBusy(false);
            }
            else 
                return false;
            
            return true;
        }
        public void OpenPlaylist(object s, ItemClickEventArgs e)
        {
            if (IsMultipleSelection)
                return;
            Playlist playlist = e.ClickedItem as Playlist;
            NavigationService.Navigate(typeof(PlaylistViewerPage), playlist.Id);
        }
        public void DeletePlaylists(List<Playlist> list)
        {
            foreach (var item in list)
            {
                PlaylistManager.Instance.DeletePlaylist(item);
            }
        }
        private bool _multiple;
        public bool IsMultipleSelection
        {
            get
            {
                return _multiple;
            }
            set
            {
                Set<bool>(ref _multiple, value);
            }
        }
        public void ChangeSelectionMode(object sender, RoutedEventArgs e)
        {
            IsMultipleSelection = !_multiple;
        }
    }
}
