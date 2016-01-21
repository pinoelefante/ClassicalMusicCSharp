using ClassicalMusicCSharp.Classes.Grouping;
using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class ArtistsPageVM : Mvvm.ViewModelBase
    {
        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Debug.WriteLine("OneClassical loaded = "+ OneClassicalHub.Instance.Loaded);
            if (Compositori == null || Compositori.Count == 0)
            {
                List<Compositore> list = await OneClassicalHub.Instance.readJson();
                Debug.WriteLine("OneClassical loaded = " + OneClassicalHub.Instance.Loaded);
                Compositori = MyGrouping<Compositore>.AlphaKeyGroup(list, (x) => { return x.Nome; });
            }
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
            if (comp.HasCategorie)
            {
                NavigationService.Navigate(typeof(CategoriePage), comp);
            }
            else
            {
                NavigationService.Navigate(typeof(OperePage), comp);
            }
        }
    }
}
