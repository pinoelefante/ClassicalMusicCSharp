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
    public class OperePageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
            string composername = parameters["Composer"].ToString();
            Composer = OneClassicalHub.Instance.GetComposerByName(composername);

            Category = null;
            if (parameters.ContainsKey("Category"))
            {
                string catName = parameters["Category"].ToString();
                Category = Composer.Categorie.Where(x => x.Nome.Equals(catName)).First();
                ListaOpere = Category.Opere;
                Title = Category.Nome;
            }
            else
            {
                ListaOpere = Composer.Opere;
                Title = Composer.Nome;
            }
            return Task.CompletedTask;
        }
        private Compositore Composer;
        private Categoria Category;
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
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"Composer", Composer.Nome },
                {"Opera", opera.Nome }
            };
            if (Category != null)
                parameters.Add("Category", Category.Nome);
            NavigationService.Navigate(typeof(OperaPage), parameters);
        }
    }
}
