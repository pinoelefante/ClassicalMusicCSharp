using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.Classes.Playlist
{
    public class PlaylistManager
    {
        private const int FREE_PLAYLISTS = 2;
        private int PlaylistAvailable = FREE_PLAYLISTS;
        private List<Playlist> playlists;
        private Playlist RiproduzioneInCorso;
        private long current_id = 0;
        private static PlaylistManager _inst;
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
            playlists = new List<Playlist>();
            LoadSavedPlaylists();
            RiproduzioneInCorso = new Playlist(string.Empty) { Id = 0 };
        }
        public void LoadPlaylistAvailable()
        {
            if (IAPManager.Instance.IsProductActive(IAPCodes.UNLIMITED_PLAYLISTS))
                PlaylistAvailable = int.MaxValue;
            else
                PlaylistAvailable = FREE_PLAYLISTS;
            Debug.WriteLine("Playlist attive = " + PlaylistAvailable);
        }
        public bool CanAddNewPlaylist()
        {
            return playlists.Count < PlaylistAvailable;
        }
        public bool CanPurchase()
        {
            return PlaylistAvailable != int.MaxValue;
        }
        public bool AddNewPlaylist(string name)
        {
            Playlist playlist = new Playlist(name) { Id = ++current_id};
            playlists.Add(playlist);
            playlist.SaveJson();
            return SavePlaylistsJson();
        }
        private void LoadSavedPlaylists()
        {

        }
        private bool SavePlaylistsJson()
        {
            return true;
        }
        public async Task<bool> PayNewPlaylist()
        {
            bool res = await IAPManager.Instance.RequestProductPurchase(IAPCodes.UNLIMITED_PLAYLISTS, false);
            if (res)
                LoadPlaylistAvailable();
            return res;
        }
        public Playlist GetPlayingNowPlaylist()
        {
            return RiproduzioneInCorso;
        }
        public void AddToPlayingNow(PlaylistTrack t)
        {
            RiproduzioneInCorso.AddItem(t);
        }
        public void AddToPlayingNow(List<PlaylistTrack> t)
        {
            foreach (var item in t)
            {
                RiproduzioneInCorso.AddItem(item);
            }
        }
    }
    public class Playlist
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<PlaylistTrack> List { get; } = new ObservableCollection<PlaylistTrack>();
        public Playlist(string name)
        {
            Name = name;
        }
        public void AddItem(PlaylistTrack t, bool save = false)
        {
            List.Add(t);
            if(save)
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
        public int Count { get { return List.Count; } }
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
        public void SaveJson()
        {

        }
        public void LoadJson()
        {

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
