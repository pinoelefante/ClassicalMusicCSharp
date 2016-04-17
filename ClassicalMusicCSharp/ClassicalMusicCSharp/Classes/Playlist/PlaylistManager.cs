using ClassicalMusicCSharp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassicalMusicCSharp.Classes.Playlist
{
    public class PlaylistManager
    {
        private const int FREE_PLAYLISTS = 2;
        private int PlaylistAvailable = FREE_PLAYLISTS;
        public ObservableCollection<Playlist> Playlists { get; }
        private Playlist NowPlayingPlaylist { get; }
        private long current_id = 0;
        private static PlaylistManager _inst;
        private bool IsLoaded { get; set; }
        public static PlaylistManager Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new PlaylistManager();
                return _inst;
            }
        }

        private PlaylistManager()
        {
            LoadPlaylistAvailable();
            Playlists = new ObservableCollection<Playlist>();
            LoadSavedPlaylists();
            NowPlayingPlaylist = new Playlist(string.Empty) { Id = 0 };
        }
        public void LoadPlaylistAvailable()
        {
            if (IAPManager.Instance.IsProductActive(IAPCodes.UNLIMITED_PLAYLISTS) || IAPManager.Instance.IsProductActive(IAPCodes.ADS_PLUS_PLAYLISTS))
                PlaylistAvailable = int.MaxValue;
            else
                PlaylistAvailable = FREE_PLAYLISTS;
            Debug.WriteLine("Playlist attive = " + PlaylistAvailable);
        }
        public bool CanAddNewPlaylist()
        {
            return Playlists.Count < PlaylistAvailable;
        }
        public bool CanPurchase()
        {
            return PlaylistAvailable != int.MaxValue;
        }
        public async Task<bool> AddNewPlaylist(string name)
        {
            if (IsNomeOk(name))
            {
                Playlist playlist = new Playlist(name) { Id = ++current_id };
                Playlists.Add(playlist);
                if (await SavePlaylistsJson())
                {
                    playlist.SaveJson();
                    return true;
                }
                else
                {
                    current_id--;
                    Playlists.Remove(playlist);
                    return false;
                }
            }
            return false;
        }
        public bool IsNomeOk(string name)
        {
            if (Playlists.Where(x => x.Name.ToLower().CompareTo(name.ToLower()) == 0).Count() > 0)
                return false;
            return true;
        }
        private async void LoadSavedPlaylists()
        {
            /* Read Json file */
            try
            {
                StorageFile sfile = await ApplicationData.Current.LocalFolder.GetFileAsync("playlists.json");
                string json = File.ReadAllText(sfile.Path);
                Debug.WriteLine(json);
                //StorageFile sfile = await StorageFile.GetFileFromPathAsync("playlists.json");

                PlaylistWrapper.PlaylistExternal _playlist = null;
                using (StreamReader file = new StreamReader(await sfile.OpenStreamForReadAsync(), true))
                {
                    string fileContent = await file.ReadToEndAsync();
                    _playlist = JsonConvert.DeserializeObject<PlaylistWrapper.PlaylistExternal>(fileContent);
                }

                /* Inizializza campi dopo creazione file Json */
                current_id = _playlist.currentId;
                foreach (var p in _playlist.playlists)
                {
                    if (CanAddNewPlaylist())
                    {
                        Playlist playlist = new Playlist(p);
                        //playlist.LoadJson();
                        Playlists.Add(playlist);
                    }
                    else
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("playlists.json");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.GetType() + " " + e.Message + "\n" + e.StackTrace);
            }
            IsLoaded = true;
        }
        private async Task<bool> SavePlaylistsJson(bool secondRound = false)
        {
            List<string> playlistsName = new List<string>();
            try
            {
                StorageFile sfile = await ApplicationData.Current.LocalFolder.GetFileAsync("playlists.json");
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartObject();
                    writer.WritePropertyName("currentId");
                    writer.WriteValue(current_id);

                    writer.WritePropertyName("playlists");
                    writer.WriteStartArray();
                    foreach (var item in Playlists)
                    {
                        writer.WriteValue(item.Name);
                    }
                    writer.WriteEndArray(); //Chiude l'array dei nomi delle playlist
                    writer.WriteEndObject(); //chiude l'oggetto esterno
                }

                await FileIO.WriteTextAsync(sfile, sb.ToString());
                return true;
            }
            catch (FileNotFoundException)
            {
                if (!secondRound)
                {
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("playlists.json");
                    return await SavePlaylistsJson(true);
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.GetType() + " " + e.Message + "\n" + e.StackTrace);
                return false;
            }

        }
        public async Task<bool> BuyUnlimitedPlaylists()
        {
            bool res = await IAPManager.Instance.RequestProductPurchase(IAPCodes.UNLIMITED_PLAYLISTS, false);
            LoadPlaylistAvailable();
            return res;
        }
        public async void DeletePlaylist(Playlist p)
        {
            string name = $"playlist_{p.Name}.json";
            Playlists.Remove(p);
            try
            {
                await SavePlaylistsJson();
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(name);
                await file.DeleteAsync();
            }
            catch
            {
                Debug.WriteLine($"Remove file {name} failed");
            }
            
        }
        public Playlist GetPlaylistById(long id)
        {
            IEnumerable<Playlist> select = Playlists.Where(x => x.Id == id);
            if (select.Count() > 0 && select.Count() == 1)
            {
                return select.First();
            }
            return null;
        }
        public bool IsPlaylistExists(string name)
        {
            IEnumerable<Playlist> select = Playlists.Where(x => x.Name.ToLower().Equals(name.ToLower()));
            if (select!=null && select.Count() > 0)
                return true;
            return false;
        }
        public Playlist GetPlayingNowPlaylist()
        {
            return NowPlayingPlaylist;
        }
        public void AddTrackToPlaylist(PlaylistTrack t, Playlist playlist, bool save = true)
        {
            playlist.AddItem(t, save);
        }
        public void AddTrackToPlaylist(List<PlaylistTrack> t, Playlist playlist, bool save = true)
        {
            playlist.AddItem(t, save);
        }
    }
    public class Playlist
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<PlaylistTrack> List { get; private set; } = new ObservableCollection<PlaylistTrack>();
        public Playlist(string name)
        {
            Name = name;
            LoadJson();
        }
        public void AddItem(PlaylistTrack t, bool save = false)
        {
            List.Add(t);
            if (save)
                SaveJson();
        }
        public void AddItem(List<PlaylistTrack> t, bool save = false)
        {
            foreach (var i in t)
                List.Add(i);
            if (save)
                SaveJson();
        }
        public void RemoveAt(int i)
        {
            List.RemoveAt(i);
        }
        public int IndexOf(PlaylistTrack t)
        {
            return List.IndexOf(t);
        }
        public bool IsEmpty()
        {
            return List.Count == 0;
        }
        public PlaylistTrack ElementAt(int index)
        {
            return List[index];
        }
        public void Clear(bool save = false)
        {
            List.Clear();
            if (save)
                SaveJson();
        }
        public async void SaveJson(bool secondRound = false)
        {
            Debug.WriteLine("Saving json");
            try
            {
                StorageFile sfile = await ApplicationData.Current.LocalFolder.GetFileAsync($"playlist_{Name}.json");
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                await FileIO.WriteTextAsync(sfile, json);
            }
            catch (FileNotFoundException)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync($"playlist_{Name}.json");
                if(!secondRound)
                    SaveJson(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.GetType() + " " + e.Message + "\n" + e.StackTrace);
            }
        }
        public async void LoadJson()
        {
            try
            {
                StorageFile sfile = await ApplicationData.Current.LocalFolder.GetFileAsync($"playlist_{Name}.json");
                string json = File.ReadAllText(sfile.Path);
                if (json.Length > 0)
                {
                    Playlist l = JsonConvert.DeserializeObject<Playlist>(json);
                    this.Id = l.Id;
                    this.List = l.List;
                }
            }
            catch (FileNotFoundException)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync($"playlist_{Name}.json");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.GetType() + " = " + e.Message+"\n"+e.StackTrace);
            }
        }
    }
    public class PlaylistTrack
    {
        public string Track { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Composer { get; set; } = string.Empty;
    }
}
