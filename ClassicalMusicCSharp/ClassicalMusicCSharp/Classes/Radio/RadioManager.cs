using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassicalMusicCSharp.Classes.Radio
{
    public class RadioManager
    {
        public static bool IsLoaded { get; set; } = false;
        public static RadioWrapper.RootObject Radios { get; set; } = null;

        public static async Task LoadRadio()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///radio.json"));
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(RadioWrapper.RootObject));
            Radios = (RadioWrapper.RootObject)ds.ReadObject(await file.OpenStreamForReadAsync());
            IsLoaded = true;
        }
    }
}
