using ClassicalMusicCSharp.OneClassical;
using ClassicalMusicCSharp.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class CategoriePageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
            string compname = parameters["Composer"].ToString();
            Compositore = OneClassicalHub.Instance.GetComposerByName(compname);
            return Task.CompletedTask;
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
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"Composer", Compositore.Nome },
                {"Category", cat.Nome }
            };
            NavigationService.Navigate(typeof(OperePage), parameters);
        }
    }
}
