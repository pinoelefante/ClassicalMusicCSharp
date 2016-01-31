﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;

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
            //DEVELOPING
            //StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //StorageFile file = await folder.GetFileAsync("IAPTest.xml");
            //await CurrentAppSimulator.ReloadSimulatorAsync(file);
            //licenseInfo = CurrentAppSimulator.LicenseInformation;

            //RELEASE
            licenseInfo = CurrentApp.LicenseInformation;
            foreach (var item in licenseInfo.ProductLicenses)
            {
                Debug.WriteLine(item.Key + "=" + item.Value);
            }
        }
        public bool IsProductActive(string code)
        {
            return licenseInfo.ProductLicenses[code].IsActive;
        }
        public async Task<bool> RequestProductPurchase(string code, bool error_return = false)
        {
            if (!licenseInfo.ProductLicenses[code].IsActive)
            {
                try
                {
                    //DEVELOPING
                    //PurchaseResults res = await CurrentAppSimulator.RequestProductPurchaseAsync(code);
                    //RELEASE
                    /*PurchaseResults res = */
                    await CurrentApp.RequestProductPurchaseAsync(code,false);
                    /*
                    Debug.WriteLine("Stato acquisto: " + res.Status.ToString() + "\n" + res.ReceiptXml.ToString());
                    Debug.WriteLine(code + " isActive: " + licenseInfo.ProductLicenses[code].IsActive);
                    if (res.Status == ProductPurchaseStatus.Succeeded || res.Status == ProductPurchaseStatus.AlreadyPurchased)
                    {
                        return !error_return;
                    }
                    */
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
    }
    public class IAPCodes
    {
        public static readonly string
            REMOVE_ADS = "RemoveAds",
            UNLIMITED_PLAYLISTS = "UnlimitedPlaylist";
    }
}
