using ClassicalMusicCSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ClassicalMusicCSharp.Views
{
    public sealed partial class OperePage : Page
    {
        public OperePage()
        {
            this.InitializeComponent();
        }
        public OperePageVM VM => this.DataContext as OperePageVM;
    }
}
