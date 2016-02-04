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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassicalMusicCSharp.Views.UserControls
{
    public sealed partial class MediaPlayerUC : UserControl
    {
        public MediaPlayerUC()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnload;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Unloaded");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Loaded");
        }

        public PlaylistTrack CurrentTrack { get; set; }
        public bool IsBuffering { get; set; }
        public bool IsPlaying { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
        public long CurrentPosition { get; set; }
        public long CurrentLength { get; set; }

        private void Play(object sender, RoutedEventArgs e)
        {

        }

        private void Pause(object sender, RoutedEventArgs e)
        {

        }

        private void Stop(object sender, RoutedEventArgs e)
        {

        }

        private void Prev(object sender, RoutedEventArgs e)
        {

        }

        private void Next(object sender, RoutedEventArgs e)
        {

        }
    }
}
