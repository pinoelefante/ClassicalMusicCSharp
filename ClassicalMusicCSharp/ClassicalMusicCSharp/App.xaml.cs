using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using ClassicalMusicCSharp.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.ViewModels;
using Template10.Common;
using Newtonsoft.Json;
using System.Diagnostics;
using Template10.Services.SerializationService;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.ApplicationModel;
using ClassicalMusicCSharp.Classes;

namespace ClassicalMusicCSharp
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        ISettingsService _settings;

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(Microsoft.ApplicationInsights.WindowsCollectors.Metadata | Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // setup hamburger shell
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
#if DEBUG
            //FOR IAP DEVELOPMENT
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file = await folder.GetFileAsync("IAPTest.xml");
            await CurrentAppSimulator.ReloadSimulatorAsync(file);
#endif
            await Task.Yield();
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // perform long-running load
            await Task.Delay(0);

            // navigate to first page
            NavigationService.Navigate(typeof(Views.ArtistsPage));
            PlayerPageVM.Init();
        }
    }
}

