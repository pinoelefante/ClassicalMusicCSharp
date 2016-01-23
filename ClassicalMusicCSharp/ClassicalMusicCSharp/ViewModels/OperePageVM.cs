using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class OperePageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            ValueSet parameters = parameter as ValueSet;
            Composer = parameters["Composer"] as Compositore;
            if (parameters.ContainsKey("Category"))
            {
                Categoria cat = parameters["Category"] as Categoria;
                ListaOpere = (cat).Opere;
                Title = cat.Nome;
            }
            else
            {
                ListaOpere = Composer.Opere;
                Title = Composer.Nome;
            }
            return Task.CompletedTask;
        }
        private Compositore Composer;
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
        public void goToOpera(object sender, ItemClickEventArgs e)
        {
            Opera opera = (Opera)e.ClickedItem;
            ValueSet parameters = new ValueSet()
            {
                {"Composer", Composer },
                {"Opera", opera }
            };
            NavigationService.Navigate(typeof(OperaPage), parameters);
        }
    }
}
