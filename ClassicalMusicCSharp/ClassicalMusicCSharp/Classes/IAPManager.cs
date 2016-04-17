using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace ClassicalMusicCSharp.Classes
{
    public class IAPManager
    {
        private LicenseInformation licenseInfo;
        private static IAPManager _instance;
        public static IAPManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IAPManager();
                return _instance;
            }
        }
        private IAPManager()
        {
#if DEBUG
            //DEVELOPING
            //StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //StorageFile file = await folder.GetFileAsync("IAPTest.xml");
            //await CurrentAppSimulator.ReloadSimulatorAsync(file);
            licenseInfo = CurrentAppSimulator.LicenseInformation;
#else
            //RELEASE
            licenseInfo = CurrentApp.LicenseInformation;
#endif
        }
        public bool IsProductActive(string code)
        {
            return licenseInfo.ProductLicenses[code].IsActive;
        }
        public void LoadInfoFromStore()
        {
            
        }
        public async Task<bool> RequestProductPurchase(string code, bool error_return = false)
        {
            if (!licenseInfo.ProductLicenses[code].IsActive)
            {
                try
                {

#if DEBUG
                    //DEVELOPING
                    await CurrentAppSimulator.RequestProductPurchaseAsync(code, false);
#else
                    //RELEASE
                    await CurrentApp.RequestProductPurchaseAsync(code,false);
#endif
                    if (licenseInfo.ProductLicenses[code].IsActive)
                        return !error_return;
                    return error_return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return error_return;
                }
            }
            else
                return !error_return;
        }
        public async Task<List<IAPItem>> GetIAPItemList()
        {
            List<IAPItem> products = new List<IAPItem>();
#if DEBUG
            var listProd = await CurrentAppSimulator.LoadListingInformationAsync();
#else
            var listProd = await CurrentApp.LoadListingInformationAsync();
#endif
            foreach(var p in listProd.ProductListings)
            {
                Debug.WriteLine(p.Key);
                var descr = p.Value.Description;
                var name = p.Value.Name;
                var cost = p.Value.FormattedPrice;
                var buyed = licenseInfo.ProductLicenses[p.Key].IsActive;
                var symbol = GetSymbolProduct(p.Key);
                products.Add(new IAPItem()
                {
                    Buyed = buyed,
                    Description = descr,
                    Price = cost,
                    Title = name,
                    Symbol = symbol,
                    ProductCode = p.Key
                });
            }
            return products;
        }
        public Symbol GetSymbolProduct(string code)
        {
            switch (code)
            {
                case IAPCodes.REMOVE_ADS:
                    return Symbol.Document;
                case IAPCodes.UNLIMITED_PLAYLISTS:
                    return Symbol.Bullets;
            }
            return Symbol.Document;
        }
    }
    public class IAPCodes
    {
        public const string
            REMOVE_ADS = "RemoveAds",
            UNLIMITED_PLAYLISTS = "UnlimitedPlaylist",
            ADS_PLUS_PLAYLISTS = "AdsPlaylists";
    }
    public class IAPItem
    {
        public string Title { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public Symbol Symbol { get; set; }
        public bool Buyed { get; set; }
        public string ProductCode { get; set; }
    }
}
