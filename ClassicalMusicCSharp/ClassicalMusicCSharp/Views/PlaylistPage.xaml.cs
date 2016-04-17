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
    public sealed partial class PlaylistPage : Page
    {
        public PlaylistPage()
        {
            this.InitializeComponent();
        }
        public PlaylistPageVM VM => this.DataContext as PlaylistPageVM;

        private async void DeleteSelection(object sender, RoutedEventArgs e)
        {
            if (listPlaylist.SelectedItems.Count > 0)
            {
                Action deleteAction = () =>
                {
                    List<Playlist> list = listPlaylist.SelectedItems.Cast<Playlist>().ToList();
                    VM.DeletePlaylists(list);
                };
                DeletePlaylistContentDialog dlg = new DeletePlaylistContentDialog(deleteAction);
                await dlg.ShowAsync();
            }
        }

        private async void RenamePlaylist(object sender, RoutedEventArgs e)
        {
            Playlist p = (sender as FrameworkElement).DataContext as Playlist;
            RenamePlaylistContentDialog dlg = new RenamePlaylistContentDialog();
            Action OnFinish = async () =>
            {
                if (dlg.FinishOk)
                {
                    var newName = dlg.NewPlaylistName;
                    var res = await VM.RenamePlaylist(p, newName);
                    if(res)
                        Shell.Instance.ShowMessagePopup("Playlist renamed");
                    else
                        Shell.Instance.ShowMessagePopup("An error occurred while renaming the playlist");
                }
            };
            dlg.OnFinish = OnFinish;
            await dlg.ShowAsync();
        }

        private async void DeletePlaylist(object sender, RoutedEventArgs e)
        {
            Playlist p = (sender as FrameworkElement).DataContext as Playlist;
            Action deleteAction = () =>
            {
                VM.DeletePlaylist(p);
            };
            DeletePlaylistContentDialog dlg = new DeletePlaylistContentDialog(deleteAction);
            await dlg.ShowAsync();
        }
    }
}
