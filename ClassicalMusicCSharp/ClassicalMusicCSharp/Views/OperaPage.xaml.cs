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

        private void addToPlaylist(object sender, RoutedEventArgs e)
        {
            Traccia track = (sender as FrameworkElement).DataContext as Traccia;
            VM.addToPlaylist(track);
        }

        private void openMenu(object sender, object e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void playlistSelected(object sender, RoutedEventArgs e)
        {
            List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
            VM.PlaylistSelected(sel);
        }

        private void playSelected(object sender, RoutedEventArgs e)
        {
            List<Traccia> sel = lista.SelectedItems.Cast<Traccia>().ToList();
            VM.PlaySelected(sel);
        }
    }
}
