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
using Windows.UI.Xaml.Media;
using Windows.UI;

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
        public void CheckAdsRemoved(object sender = null, RoutedEventArgs e = null)
        {
            AdsRemoved = IAPManager.Instance.IsProductActive(IAPCodes.REMOVE_ADS) || IAPManager.Instance.IsProductActive(IAPCodes.ADS_PLUS_PLAYLISTS);
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
            IsAdsEnabled = false;
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
            SetBusy(true, "Wait please");
            await dlg.ShowAsync();
            SetBusy(false);
        }
        public bool IsAdsEnabled { get; set; } = false;
        private void HamburgerOpen(object sender, EventArgs e)
        {
            if (IsAdsEnabled && Window.Current.Bounds.Width < 1200)
                AdsContainer.Visibility = Visibility.Collapsed;
        }

        private void HamburgerClosed(object sender, EventArgs e)
        {
            if (IsAdsEnabled)
                AdsContainer.Visibility = Visibility.Visible;
        }
        public bool ShowMessage { get; set; } = false;
        public string MessageText { get; set; } = string.Empty;
        public void ShowMessagePopup(string message, bool error = false)
        {
            MessageText = message;
            if (!error)
                MessageContainer.Background = new SolidColorBrush(Colors.LimeGreen);
            else
                MessageContainer.Background = new SolidColorBrush(Colors.Red);
            ShowMessage = true;

            Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(MessageText)));
            Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(ShowMessage)));

            DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1500) };
            timer.Tick += (s, e) =>
            {
                CloseMessagePopup();
                timer.Stop();
            };
            timer.Start();
        }

        public void CloseMessagePopup(object s = null, object e = null)
        {
            MessageText = string.Empty;
            ShowMessage = false;
            Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(MessageText)));
            Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(ShowMessage)));
        }
    }
}

