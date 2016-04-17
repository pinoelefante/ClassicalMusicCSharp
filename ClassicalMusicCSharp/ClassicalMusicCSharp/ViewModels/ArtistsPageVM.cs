using ClassicalMusicCSharp.Classes;
using ClassicalMusicCSharp.Classes.Grouping;
using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using ClassicalMusicCSharp.Views.ContentDialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using Template10.Common;
using Template10.Services.NavigationService;

namespace ClassicalMusicCSharp.ViewModels
{
    public class ArtistsPageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            OneClassicalHub.Instance.PropertyChanged += OnPropertyChanged;
            NavigationService.ClearCache();
            CheckLoaded();
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            OneClassicalHub.Instance.PropertyChanged -= OnPropertyChanged;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            
            Compositori.Clear();
            Compositori = null;
           
            return base.OnNavigatingFromAsync(args);
        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckLoaded();
        }
        private void CheckLoaded()
        {
            if (Compositori!=null && Compositori.Count > 0)
                return;
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                IsLoaded = OneClassicalHub.Instance.Loaded;
                Debug.WriteLine("IsLoaded = " + IsLoaded);
                if (IsLoaded && (Compositori == null || Compositori.Count == 0))
                {
                    Compositori = MyGrouping<Compositore>.AlphaKeyGroup(OneClassicalHub.Instance.ListaCompositori,
                                                                        (x) => { return x.Nome; });
                }
            });
        }
        private Dictionary<String, List<Compositore>> _l;
        public Dictionary<String, List<Compositore>> Compositori
        {
            get
            {
                return _l;
            }
            private set
            {
                Set<Dictionary<String, List<Compositore>>>(ref _l, value);
            }
        }
        public void goToArtist(object sender, ItemClickEventArgs e)
        {
            Compositore comp = e.ClickedItem as Compositore;
            Dictionary<string,object> parameters = new Dictionary<string, object>()
            {
                {"Composer", comp.Nome }
            };
            if (comp.HasCategorie)
            {
                NavigationService.Navigate(typeof(CategoriePage), parameters);
            }
            else
            {
                NavigationService.Navigate(typeof(OperePage), parameters);
            }
        }
        private bool _loaded;
        public bool IsLoaded
        {
            get
            {
                return _loaded;
            }
            set
            {
                Set<bool>(ref _loaded, value);
            }
        }
    }
}
