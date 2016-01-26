using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.OneClassical;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassicalMusicCSharp.Views.ContentDialogs
{
    public sealed partial class PlaylistChoiserContentDialog : ContentDialog
    {
        public ObservableCollection<Playlist> Playlists { get; } = PlaylistManager.Instance.Playlists;
        private Action<Playlist> ActionOnChosen;
        public PlaylistChoiserContentDialog(Action<Playlist> function)
        {
            this.InitializeComponent();
            ActionOnChosen = function;
        }
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
        private void PlaylistChosen(object sender, ItemClickEventArgs e)
        {
            Playlist playlist = e.ClickedItem as Playlist;
            if (ActionOnChosen != null)
                ActionOnChosen.Invoke(playlist);
            Hide();
        }

        private void AddToNowPlaying(object sender, object e)
        {
            Playlist playlist = PlaylistManager.Instance.GetPlayingNowPlaylist();
            if (ActionOnChosen != null)
                ActionOnChosen.Invoke(playlist);
            Hide();
        }
    }
}
