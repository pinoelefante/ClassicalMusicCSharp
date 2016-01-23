using ClassicalMusicCSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class PlayerPage : Page
    {
        public PlayerPage()
        {
            this.InitializeComponent();
        }
        public PlayerPageVM VM => this.DataContext as PlayerPageVM;

        private void PlaylistItemClicked(object sender, object e)
        {
            ListView list = sender as ListView;
            int index = list.SelectedIndex;
            Debug.WriteLine("ItemClicked: " + index);
            VM.PlayAt(index);
        }
        private async void PlaylistItemMenu(object sender, object e)
        {
            PlaylistTrack track = (sender as FrameworkElement).DataContext as PlaylistTrack;
            int index = _xplaylist.Items.IndexOf(track);
            Debug.WriteLine("RightClick index = " + index);
            MessageDialog dialog = new MessageDialog("Are you sure to remove the track?","Confirm...");
            UICommand yes = new UICommand("Yes") { Id = 0, Invoked = (x) => { VM.RemoveTrackAt(index); } };
            UICommand no = new UICommand("No") { Id = 1 };
            dialog.Commands.Add(yes);
            dialog.Commands.Add(no);
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            await dialog.ShowAsync();
        }

        private void PlaylistItemMenuHolding(object sender, HoldingRoutedEventArgs e)
        {
            Debug.WriteLine("sender = " + sender.GetType());
            Debug.WriteLine("Holding source = " + e.OriginalSource.GetType());
            e.Handled = true;
        }
    }
}
