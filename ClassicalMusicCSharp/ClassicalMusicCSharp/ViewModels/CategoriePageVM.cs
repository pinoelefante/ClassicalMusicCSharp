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
    public class CategoriePageVM : Mvvm.ViewModelBase
    {
        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Compositore = parameter as Compositore;
        }
        private Compositore _comp;
        public Compositore Compositore
        {
            get
            {
                return _comp;
            }
            set
            {
                Set<Compositore>(ref _comp, value);
            }
        }
        public void goToCategoria(object sender, ItemClickEventArgs e)
        {
            Categoria cat = e.ClickedItem as Categoria;
            NavigationService.Navigate(typeof(OperePage), cat);
        }
    }
}
