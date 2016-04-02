using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassicalMusicCSharp.Classes
{
    class SettingsManager
    {
        private ApplicationDataContainer local_settings;
        private SettingsManager()
        {
            local_settings = ApplicationData.Current.LocalSettings;
            NumeroAvvii++;
        }
        private static SettingsManager instance;
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsManager();
                return instance;
            }
        }
        public long NumeroAvvii
        {
            get
            {
                if (local_settings.Values["avvii"] == null)
                    return 0;
                return (long)local_settings.Values["avvii"];
            }
            set
            {
                local_settings.Values["avvii"] = value;
            }
        }
        public void AddDownloadId(string guid, string composer, string opera, string track, string link)
        {
            local_settings.Values[$"download_{guid}_composer"] = composer;
            local_settings.Values[$"download_{guid}_opera"] = opera;
            local_settings.Values[$"download_{guid}_track"] = track;
            local_settings.Values[$"download_{guid}_link"] = link;
        }
        public void RemoveDownloadId(string guid)
        {
            local_settings.Values.Remove($"download_{guid}_composer");
            local_settings.Values.Remove($"download_{guid}_opera");
            local_settings.Values.Remove($"download_{guid}_track");
            local_settings.Values.Remove($"download_{guid}_link");
        }
        public DownloadItem GetDownloadItem(string guid)
        {
            if (local_settings.Values.ContainsKey($"download_{guid}_composer"))
            {
                var composer = local_settings.Values[$"download_{guid}_composer"].ToString();
                var opera = local_settings.Values[$"download_{guid}_opera"].ToString();
                var track = local_settings.Values[$"download_{guid}_track"].ToString();
                var link = local_settings.Values[$"download_{guid}_link"].ToString();
                return new DownloadItem() { ComposerName = composer, OperaName = opera, TrackName = track, IdDownload = guid };
            }
            return null;
        }
        public void CleanDownloadItems()
        {
            IEnumerable<string> keys = local_settings.Values.Keys.Where(x => x.StartsWith("download_"));
            foreach(var k in keys)
                local_settings.Values.Remove(k);
        }
    }
}
