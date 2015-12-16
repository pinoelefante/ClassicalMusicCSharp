using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class OperePageVM : Mvvm.ViewModelBase
    {
        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter is Categoria)
            {
                Categoria cat = parameter as Categoria;
                ListaOpere = cat.Opere;
                Title = cat.Nome;
            }
            else if(parameter is Compositore)
            {
                Compositore comp = parameter as Compositore;
                ListaOpere = comp.Opere;
                Title = comp.Nome;
            }
        }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Set<string>(ref _title, value);
            }
        }
        private List<Opera> _opere;
        public List<Opera> ListaOpere
        {
            get
            {
                return _opere;
            }
            set
            {
                Set<List<Opera>>(ref _opere, value);
            }
        }
        public void goToOpera(object sender, object e)
        {
            if(sender is ListView)
            {
                ListView list = sender as ListView;
                Opera opera = list.SelectedItem as Opera;
                NavigationService.Navigate(typeof(OperaPage), opera);
            }
        }
    }
}
