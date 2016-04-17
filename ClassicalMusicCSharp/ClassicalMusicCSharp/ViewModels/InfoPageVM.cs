using ClassicalMusicCSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class InfoPageVM : Mvvm.ViewModelBase
    {
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LoadProducts();
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        public string Version
        {
            get
            {
                var myPackage = Windows.ApplicationModel.Package.Current;
                var version = myPackage.Id.Version;

                var appVersion = version.Major + "." +
                                 version.Minor + "." +
                                 version.Build;
                return appVersion;
            }
        }
        public async void Vote(object s, object e)
        {
            var uri = new Uri(string.Format("ms-windows-store:reviewapp?appid={0}", CurrentApp.AppId));
            await Launcher.LaunchUriAsync(uri);
        }
        public List<IAPItem> ListProducts { get; private set; }
        public async void LoadProducts(object s = null, object e = null)
        {
            ListProducts = await IAPManager.Instance.GetIAPItemList();
            RaisePropertyChanged(nameof(ListProducts));
        }
        public async void IAPSelected(object s, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as IAPItem;
            if (item.Buyed || IAPManager.Instance.IsProductActive(item.ProductCode))
                Views.Shell.Instance.ShowMessagePopup("You already own this product");
            else
            {
                MessageDialog dlg = new MessageDialog($"{item.Description}", $"{item.Title} at {item.Price}");
                UICommand buy = new UICommand("Buy") { Invoked = async (x) =>
                {
                    Views.Shell.SetBusy(true, "Wait please");
                    bool res = await IAPManager.Instance.RequestProductPurchase(item.ProductCode);
                    if (res && item.ProductCode.Equals(IAPCodes.REMOVE_ADS))
                        Views.Shell.Instance.CheckAdsRemoved();
                    Views.Shell.SetBusy(false, null);
                    LoadProducts();
                }, Id = 0 };
                UICommand no = new UICommand("Cancel") { Id = 1 };
                dlg.Commands.Add(buy);
                dlg.Commands.Add(no);
                dlg.DefaultCommandIndex = 0;
                dlg.CancelCommandIndex = 1;
                await dlg.ShowAsync();
            }
        }
    }
}
