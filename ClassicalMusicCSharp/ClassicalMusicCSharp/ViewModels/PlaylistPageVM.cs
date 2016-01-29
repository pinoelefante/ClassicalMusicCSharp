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
            if(parameter != null)
            {
                bool cleanStack = (bool)parameter;
                NavigationService.ClearHistory();
                NavigationService.ClearCache();
            }
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        public ObservableCollection<Playlist> Playlists { get; } = PlaylistManager.Instance.Playlists;
        public async void AddPlaylist(object s, object e)
        {
            await CreateNewPlaylist();
        }
        public static async Task<bool> CreateNewPlaylist()
        {
            if (PlaylistManager.Instance.CanAddNewPlaylist())
            {
                ContentDialog dlg = new AddPlaylistContentDialog();
                await dlg.ShowAsync();
                return true;
            }
            else if (PlaylistManager.Instance.CanPurchase())
            {
                Action<bool> OnSucced = async (bool res) =>
                {
                    if (res)
                    { 
                        ContentDialog dlg2 = new AddPlaylistContentDialog();
                        await dlg2.ShowAsync();
                    }
                };
                BuyPlaylistsContentDialog dlg = new BuyPlaylistsContentDialog();
                dlg.OnBuySucceded = OnSucced;
                await dlg.ShowAsync();
                
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
