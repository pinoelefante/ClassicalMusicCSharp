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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassicalMusicCSharp.Views.UserControls
{
    public sealed partial class TrackUC : UserControl
    {
        public TrackUC()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            string composer = ComposerName.Text.ToLower();
            Image.ImageSource = new BitmapImage(new Uri($"ms-appx:///Assets/composers/square/{composer.Replace(" ", "_")}_square.jpg"));
        }
    }
}
