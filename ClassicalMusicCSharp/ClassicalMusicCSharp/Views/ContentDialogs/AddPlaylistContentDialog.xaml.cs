using ClassicalMusicCSharp.Classes.Playlist;
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

namespace ClassicalMusicCSharp.Views.ContentDialogs
{
    public sealed partial class AddPlaylistContentDialog : ContentDialog
    {
        public AddPlaylistContentDialog()
        {
            this.InitializeComponent();
        }
        private async void AddPlaylist(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string name = textbox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Shell.Instance.ShowMessagePopup("Playlist's name can't be empty", true);
                Cancelled = true;
                return;
            }
            bool insert = await PlaylistManager.Instance.AddNewPlaylist(name);
            Debug.WriteLine("Insert = " + insert);
        }

        private void Cancel(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Cancelled = true;
            this.Hide();
            args.GetDeferral().Complete();
        }
        public bool Cancelled { get; set; } = false;

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string name = textbox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Error.Text = "Playlist's name can't be empty";
                IsPrimaryButtonEnabled = false;
            }
            else if (PlaylistManager.Instance.IsPlaylistExists(name))
            {
                Error.Text = "Playlist's name already exists";
                IsPrimaryButtonEnabled = false;
            }
            else
            {
                Error.Text = string.Empty;
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
