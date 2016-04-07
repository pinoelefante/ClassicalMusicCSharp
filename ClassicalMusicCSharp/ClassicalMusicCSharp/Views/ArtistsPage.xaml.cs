using ClassicalMusicCSharp.ViewModels;
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
using System.ComponentModel;
using System.Diagnostics;
using ClassicalMusicCSharp.Classes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassicalMusicCSharp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistsPage : Page
    {
        public ArtistsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        
        public ArtistsPageVM VM => this.DataContext as ArtistsPageVM;

        private void Test(object sender, RoutedEventArgs e)
        {
            /*
            using (var vorbis = new NVorbis.VorbisReader(@"C:\PATH\TO\OGG\FILE.ogg"))
            {
                int channels = vorbis.Channels;
                int sampleRate = vorbis.SampleRate;
                var duration = vorbis.TotalTime;
                
                var buffer = new float[16384];
                int count;
                while ((count = vorbis.ReadSamples(buffer, 0, buffer.Length)) > 0)
                {
                    // Do stuff with the samples returned...
                    // Sample value range is -0.99999994f to 0.99999994f
                    // Samples are interleaved (chan0, chan1, chan0, chan1, etc.)
                }
            }
            */
        }
    }
}
