using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace ClassicalMusicCSharp.ViewModels
{
    public class InfoPageVM : Mvvm.ViewModelBase
    {
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
    }
}
