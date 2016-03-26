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
        /*

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine("PropertyChanged: " + e.PropertyName);
            switch (e.PropertyName)
            {
                case "AdsRemoved":
                    if (VM.AdsRemoved == false) //ADS Visibile
                    {
                        FrameworkElement adsCont = this.FindName("AdsContainer") as FrameworkElement;
                        if (adsCont != null)
                            adsCont.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (AdsContainer != null)
                        {
                            AdsContainer.Children.Clear();
                            AdsContainer.Visibility = Visibility.Collapsed;
                            AdsContainer = null;
                        }
                    }
                    break;
            }
        }
        */
        public ArtistsPageVM VM => this.DataContext as ArtistsPageVM;
    }
}
