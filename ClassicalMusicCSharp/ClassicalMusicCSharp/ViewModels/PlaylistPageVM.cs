using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlaylistPageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
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
                ContentDialog dlg = new BuyPlaylistsContentDialog();
                await dlg.ShowAsync();
                if (PlaylistManager.Instance.CanAddNewPlaylist())
                {
                    ContentDialog dlg2 = new AddPlaylistContentDialog();
                    await dlg2.ShowAsync();
                    return true;
                }
            }
            else 
                return false;
            
            return true;
        }
        public void OpenPlaylist(object s, ItemClickEventArgs e)
        {
            Playlist playlist = e.ClickedItem as Playlist;
            Debug.WriteLine("Clicked " + playlist.Name);
        }
    }
}
