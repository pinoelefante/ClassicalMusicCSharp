using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlaylistPageVM : Mvvm.ViewModelBase
    {
        public PlaylistPageVM() { }
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
        private void s(object s, object e) { }
    }
}
