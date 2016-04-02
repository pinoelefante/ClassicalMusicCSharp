using ClassicalMusicCSharp.Classes.FileManager;
using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class DownloadPageVM : Mvvm.ViewModelBase
    {
        public DownloadPageVM()
        {
            DownloadList = new ObservableCollection<DownloadItem>();
        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Init();
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            return base.OnNavigatedFromAsync(state, suspending);
        }
        private void Init()
        {
            Template10.Common.WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                System.Diagnostics.Debug.WriteLine("Ci sono "+ DownloadManager.Instance.ListaDownload.Count + " download in corso");
                foreach(var item in DownloadManager.Instance.ListaDownload)
                {
                    DownloadList.Add(item);
                }
                IsEmpty = DownloadList.Count == 0;
            });
        }
        public ObservableCollection<DownloadItem> DownloadList { get; private set; }
        private bool _isEmpty = true;
        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
            set
            {
                Set(ref _isEmpty, value);
            }
        }
    }
}
