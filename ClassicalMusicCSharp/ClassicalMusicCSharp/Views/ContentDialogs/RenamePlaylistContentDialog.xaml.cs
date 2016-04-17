using ClassicalMusicCSharp.Classes.Playlist;
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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassicalMusicCSharp.Views.ContentDialogs
{
    public sealed partial class RenamePlaylistContentDialog : ContentDialog
    {
        public RenamePlaylistContentDialog()
        {
            this.InitializeComponent();
        }
        public Action OnFinish { get; set; }
        public bool FinishOk { get; set; } = false;
        public string NewPlaylistName { get; set; } = string.Empty;
        private void RenameClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            NewPlaylistName = textbox.Text.Trim();
            FinishOk = true;
            if (OnFinish != null)
                OnFinish.Invoke();
        }

        private void CancelClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var name = textbox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                IsPrimaryButtonEnabled = false;
                Error.Text = "Playlist's name can't be empty";
            }
            else if (PlaylistManager.Instance.IsPlaylistExists(name))
            {
                IsPrimaryButtonEnabled = false;
                Error.Text = "Playlist's name already exists";
            }
            else
            {
                IsPrimaryButtonEnabled = true;
                Error.Text = string.Empty;
            }
        }
    }
}
