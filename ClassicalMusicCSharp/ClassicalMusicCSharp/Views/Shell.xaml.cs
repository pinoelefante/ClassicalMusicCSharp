using ClassicalMusicCSharp.Classes;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using Template10.Controls;
using Template10.Services.NavigationService;
using Template10.Common;
using Windows.UI.Core;

namespace ClassicalMusicCSharp.Views
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    public sealed partial class Shell : Page, INotifyPropertyChanged
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu { get { return Instance.MyHamburgerMenu; } }

        public Shell(INavigationService navigationService)
        {
            Instance = this;
            InitializeComponent();
            MyHamburgerMenu.NavigationService = navigationService;
        }

        public bool IsBusy { get; set; } = false;
        public string BusyText { get; set; } = "Please wait...";
        public event PropertyChangedEventHandler PropertyChanged;

        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                if (busy)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                else
                    BootStrapper.Current.UpdateShellBackButton();

                Instance.IsBusy = busy;
                Instance.BusyText = text;

                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsBusy)));
                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(BusyText)));
            });
        }
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            AdsRemoved = IAPManager.Instance.IsProductActive(IAPCodes.REMOVE_ADS);
            if (!AdsRemoved)
            {
                FrameworkElement adsCont = this.FindName("AdsContainer") as FrameworkElement;
                if (adsCont != null)
                    adsCont.Visibility = Visibility.Visible;
                IsAdsEnabled = true;
            }
            else
            {
                if (AdsContainer != null)
                {
                    AdsContainer.Visibility = Visibility.Collapsed;
                }
            }
        }
        private bool _adsRemoved = false;
        public bool AdsRemoved
        {
            get
            {
                return _adsRemoved;
            }
            set
            {
                _adsRemoved = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdsRemoved)));
            }
        }
        private void CloseAds()
        {
            if (AdsContainer != null)
            {
                AdsContainer.Children.Clear();
                AdsContainer.Visibility = Visibility.Collapsed;
                AdsContainer = null;
            }
        }
        public async void RemoveAds(object sender, object ev)
        {
            BuyRemoveAdsContentDialog dlg = new BuyRemoveAdsContentDialog();
            dlg.AtFinish = () =>
            {
                Debug.WriteLine("Running AtFinish");
                AdsRemoved = IAPManager.Instance.IsProductActive(IAPCodes.REMOVE_ADS);
                if (AdsRemoved)
                    CloseAds();
                Debug.WriteLine("AdsRemoved value = " + AdsRemoved);
            };
            await dlg.ShowAsync();
        }
        public bool IsAdsEnabled { get; set; } = false;
        private void HamburgerOpen(object sender, EventArgs e)
        {
            if (IsAdsEnabled && Window.Current.Bounds.Width < 1200)
                AdsContainer.Opacity = 0;
        }

        private void HamburgerClosed(object sender, EventArgs e)
        {
            if (IsAdsEnabled)
                AdsContainer.Opacity = 1;
        }
    }
}

