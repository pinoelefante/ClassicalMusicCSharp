using ClassicalMusicCSharp.Classes.Radio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace ClassicalMusicCSharp.OneClassical
{
    public class OneClassicalHub : INotifyPropertyChanged
    {
        private static OneClassicalHub instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static OneClassicalHub Instance
        {
            get
            {
                if (instance == null)
                    instance = new OneClassicalHub();
                return instance;
            }
        }
        private bool _loaded = false;
        public bool Loaded
        {
            get
            {
                return _loaded;
            }
            set
            {
                _loaded = value;
                NotifyProperty();
            }
        }
        private OneClassicalHub()
        {
            readJson();
        }
        public List<Compositore> ListaCompositori { get; private set; }
        public Compositore GetComposerByName(string name)
        {
            return ListaCompositori.Where(x => x.Nome.Equals(name)).First();
        }
        public async Task<List<Compositore>> readJson()
        {
            if (ListaCompositori == null || ListaCompositori.Count == 0)
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///music.json"));
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(List<Compositore>));
                ListaCompositori = (List<Compositore>)ds.ReadObject(await file.OpenStreamForReadAsync());
                ListaCompositori.TrimExcess();
                // inizio trim
                foreach (var comp in ListaCompositori)
                {
                    if (comp.HasCategorie)
                    {
                        comp.Categorie.TrimExcess();
                        foreach (var op in comp.Categorie)
                        {
                            op.Opere.TrimExcess();
                            foreach (var opera in op.Opere)
                                opera.Tracce.TrimExcess();
                        }
                    }
                    else
                    {
                        comp.Opere.TrimExcess();
                        foreach (var opera in comp.Opere)
                            opera.Tracce.TrimExcess();
                    }
                }
                // fine trim
                Loaded = true;
            }
            return ListaCompositori;
        }

        private async Task<string> GetImageTile(string composer, string type = "square")
        {
            string to_search = composer.ToLower().Replace(" ", "_") + "_" + type;

            StorageFolder instPath = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder folder = await instPath.GetFolderAsync($@"Assets\composers\{type}");

            IEnumerable<StorageFile> result = (await folder.GetFilesAsync()).Where(f => f.DisplayName.Equals(to_search));
            if (result?.Count() > 0)
            {
                string path = result.ElementAt(0).Path;
                return path;
            }
            return "ms-appx:///Assets/spartito.jpg";
        }
        public void NotifyProperty([CallerMemberName]string p = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
