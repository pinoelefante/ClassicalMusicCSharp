using ClassicalMusicCSharp.Classes;
using ClassicalMusicCSharp.Classes.Playlist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassicalMusicCSharp.Views.ContentDialogs
{
    public sealed partial class BuyPlaylistsContentDialog : ContentDialog
    {
        public BuyPlaylistsContentDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool result = await PlaylistManager.Instance.PayNewPlaylist();
            this.Hide();
            if (result)
                await new MessageDialog("You have just unlocked unlimited playlists", "Congratulation!").ShowAsync();
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
