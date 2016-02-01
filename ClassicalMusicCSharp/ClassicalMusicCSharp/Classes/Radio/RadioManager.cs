using ClassicalMusicCSharp.Models;
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
using Windows.Storage;

namespace ClassicalMusicCSharp.Classes.Radio
{
    public class RadioManager : INotifyPropertyChanged
    {
        private static RadioManager _inst;
        public static RadioManager Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new RadioManager();
                return _inst;
            }
        }
        private bool _loaded;
        public bool IsLoaded
        {
            get
            {
                return _loaded;
            }
            set
            {
                _loaded = value;
                Notify();
            }
        }
        private RadioManager() { }
        public RadioWrapper.RootObject Radios { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<List<RadioWrapper.Radio>> LoadRadio()
        {
            if (!IsLoaded)
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///radio.json"));
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(RadioWrapper.RootObject));
                Radios = (RadioWrapper.RootObject)ds.ReadObject(await file.OpenStreamForReadAsync());
                IsLoaded = true;
            }
            return Radios.radio;
        }
        private void Notify([CallerMemberName]string n = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(n));
            }
        }
    }
}
