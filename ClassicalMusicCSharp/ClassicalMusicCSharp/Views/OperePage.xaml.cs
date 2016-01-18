using ClassicalMusicCSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.Views
{
    public sealed partial class OperePage : Page
    {
        public OperePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        public OperePageVM VM => this.DataContext as OperePageVM;
    }
}
