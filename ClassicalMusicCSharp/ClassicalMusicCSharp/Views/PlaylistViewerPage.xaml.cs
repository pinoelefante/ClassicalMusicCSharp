using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.ViewModels;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassicalMusicCSharp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistViewerPage : Page
    {
        public PlaylistViewerPage()
        {
            this.InitializeComponent();
        }
        public PlaylistViewerVM VM => this.DataContext as PlaylistViewerVM;

        private async void DeleteSelection(object sender, RoutedEventArgs e)
        {
            if (listPlaylist.SelectedItems.Count > 0)
            {
                Action delete = () =>
                {
                    List<PlaylistTrack> tracks = listPlaylist.SelectedItems.Cast<PlaylistTrack>().ToList();
                    VM.DeleteTracks(tracks);
                };
                DeletePlaylistTracksContentDialog dlg = new DeletePlaylistTracksContentDialog(delete);
                await dlg.ShowAsync();
            }
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            if (listPlaylist.SelectedItems.Count > 0)
            {
                List<PlaylistTrack> tracks = listPlaylist.SelectedItems.Cast<PlaylistTrack>().ToList();
                VM.Download(tracks);
            }
        }
    }
}
