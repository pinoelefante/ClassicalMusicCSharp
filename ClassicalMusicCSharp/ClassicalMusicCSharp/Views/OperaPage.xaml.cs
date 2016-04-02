using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class OperaPage : Page
    {
        public OperaPage()
        {
            this.InitializeComponent();
        }
        public OperaPageVM VM => this.DataContext as OperaPageVM;
        
        private void OpenPlaylistMenu(object sender, RoutedEventArgs e)
        {
            SemanticZoomControl.ToggleActiveView();
        }
        private void playSelected(object sender, RoutedEventArgs e)
        {
            List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
            VM.PlaySelected(sel);
        }

        private void PlaylistSelected(object sender, ItemClickEventArgs e)
        {
            Playlist playlist = e.ClickedItem as Playlist;
            Debug.WriteLine("Selezionata la playlist: " + playlist.Name);
            if (VM.IsSingleMode) //playlist all
            {
                VM.PlaylistAll(playlist);
            }
            else
            {
                List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
                VM.PlaylistSelected(sel, playlist);
            }
            SemanticZoomControl.ToggleActiveView(); //chiude zoomout
        }

        private async void CreateNewPlaylist(object sender, RoutedEventArgs e)
        {
            if(await PlaylistPageVM.CreateNewPlaylist())
            {

            }
            else
            {
                //TODO Show error
            }
        }

        private void AddToNowPlaying(object sender, RoutedEventArgs e)
        {
            Playlist playlist = PlaylistManager.Instance.GetPlayingNowPlaylist();
            if (VM.IsSingleMode) //playlist all
            {
                VM.PlaylistAll(playlist);
            }
            else
            {
                List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
                VM.PlaylistSelected(sel, playlist);
            }
            SemanticZoomControl.ToggleActiveView(); //chiude zoomout
        }
        private void ListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Source:"+e.OriginalSource.GetType()+"\nSender: "+sender.GetType());
            Playlist playlist = (sender as FrameworkElement).DataContext as Playlist;
            Debug.WriteLine("Selected "+playlist.Name);
            if (VM.IsSingleMode) //playlist all
            {
                VM.PlaylistAll(playlist);
            }
            else
            {
                List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
                VM.PlaylistSelected(sel, playlist);
            }
            e.Handled = true;
        }

        private void ClosePlaylists(object sender, RoutedEventArgs e)
        {
            SemanticZoomControl.IsZoomedInViewActive = true;
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            VM.Download(lista.SelectedItems.Cast<Traccia>().ToList());
        }
    }
}
