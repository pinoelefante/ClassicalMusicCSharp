using ClassicalMusicCSharp.OneClassical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using Windows.Media.Playback;
using System.Diagnostics;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Notifications;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using ClassicalMusicCSharp.Classes.Playlist;
using ClassicalMusicCSharp.Classes;
using ClassicalMusicCSharp.Views.ContentDialogs;
using ClassicalMusicCSharp.Classes.FileManager;

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlayerPageVM : Mvvm.ViewModelBase
    {
        private DispatcherTimer dt;
        
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += MessageReceived;
            if(dt==null)
                dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            dt.Tick += Update;
            dt.Start();
            ReadPlayerCurrentStatus();
            Tracks(); //richiede la playlist
            RequestIsRadioPlaying();
            BackgroundMediaPlayer.Current.CurrentStateChanged += MediaPlayerStateChanged;
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground -= MessageReceived;
            dt.Tick -= Update;
            dt.Stop();
            BackgroundMediaPlayer.Current.CurrentStateChanged -= MediaPlayerStateChanged;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        private void MessageReceived(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            string command = e.Data["Command"].ToString();
            Template10.Common.WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                switch (command)
                {
                    case "HasNext":
                        HasNext = bool.Parse(e.Data["HasNext"].ToString());
                        break;
                    case "HasPrev":
                        HasPrev = bool.Parse(e.Data["HasPrev"].ToString());
                        break;
                    case "HasTracks":
                        HasTracks = bool.Parse(e.Data["HasTracks"].ToString());
                        break;
                    case "TrackChanged":
                        RequestTrackInfo();
                        break;
                    case "Current":

                        break;
                    case "CurrentIndex":
                        CurrentIndex = (int)e.Data["CurrentIndex"];
                        Debug.WriteLine("CurrentIndex = " + CurrentIndex);
                        break;
                    case "GetPosition":
                        var length = (int)e.Data["Length"] < 1 ? 1 : (int)e.Data["Length"];
                        CurrentLength = length;
                        CurrentPosition = (int)e.Data["Position"];
                        CurrLengthString = TimeSpan.FromSeconds(CurrentLength).ToString();
                        CurrTimeString = TimeSpan.FromSeconds(CurrentPosition).ToString();
                        Debug.WriteLine($"Positions {CurrentPosition}/{CurrentLength}");
                        break;
                    case "AddTrack":
                        Debug.WriteLine("Track added");
                        break;
                    case "TrackRemoved":
                        var index = (int)e.Data["Index"];
                        Playlist.RemoveAt(index);
                        break;
                    case "PlaylistCleaned":
                        Playlist.Clear();
                        RaisePropertyChanged(nameof(Playlist));
                        RequestTrackInfo();
                        break;
                    case "Tracks":
                        {
                            Playlist.Clear();
                            RaisePropertyChanged(nameof(Playlist));
                            var count = (int)e.Data["Count"];
                            for(int i=0;i< count; i++)
                            {
                                var title = e.Data[$"Track{i}_Title"].ToString();
                                var compo = e.Data[$"Track{i}_Composer"].ToString();
                                var link = e.Data[$"Track{i}_Link"].ToString();
                                var album = e.Data[$"Track{i}_Album"].ToString();
                                PlaylistTrack t = new PlaylistTrack() { Track = title, Album = album, Composer = compo, Link = link };
                                Playlist.AddItem(t);
                            }
                            RequestTrackInfo();
                        }
                        break;
                    case "IsRadioPlaying":
                        {
                            _radioPlaying = (bool)e.Data["IsRadioPlaying"];
                            if (_radioPlaying)
                            {
                                var title = e.Data["Title"].ToString();
                                RadioTrack = new PlaylistTrack() { Track = title };
                            }
                            RaisePropertyChanged(nameof(CurrentTrack));
                        }
                        break;
                }
            });
        }
        //public ObservableCollection<PlaylistTrack> Playlist { get; } = new ObservableCollection<PlaylistTrack>();
        public Playlist Playlist { get; } = PlaylistManager.Instance.GetPlayingNowPlaylist();
        private bool _hasNext, _hasPrev, _hasTracks, _playing,_buffering, _radioPlaying;
        private int _curIndex;
        private int _curPos, _curLength;
        private string curTime = string.Empty, curLengthS = string.Empty;
        public string CurrTimeString
        {
            get
            {
                return curTime;
            }
            set
            {
                Set(ref curTime, value);
            }
        }
        public string CurrLengthString
        {
            get
            {
                return curLengthS;
            }
            set
            {
                Set(ref curLengthS, value);
            }
        }
        public bool HasNext
        {
            get
            {
                return _hasNext;
            }
            set
            {
                Set<bool>(ref _hasNext, value);
            }
        }
        public bool HasPrev
        {
            get
            {
                return _hasPrev;
            }
            set
            {
                Set<bool>(ref _hasPrev, value);
            }
        }
        public bool HasTracks
        {
            get
            {
                return _hasTracks;
            }
            set
            {
                Set<bool>(ref _hasTracks, value);
            }
        }
        public int CurrentIndex
        {
            get
            {
                return _curIndex;
            }
            set
            {
                Set<int>(ref _curIndex, value);
                RaisePropertyChanged(nameof(CurrentTrack));
            }
        }
        public int CurrentPosition
        {
            get
            {
                return _curPos;
            }
            set
            {
                Set<int>(ref _curPos, value);
            }
        }
        public int CurrentLength
        {
            get
            {
                return _curLength;
            }
            set
            {
                Set<int>(ref _curLength, value);
            }
        }
        private PlaylistTrack RadioTrack;
        private static readonly PlaylistTrack EMPTYTRACK = new PlaylistTrack();
        public PlaylistTrack CurrentTrack
        {
            get
            {
                if (_radioPlaying && RadioTrack != null)
                    return RadioTrack;
                if(!Playlist.IsEmpty())
                    return Playlist.ElementAt(CurrentIndex);
                return EMPTYTRACK;
            }
        }
        public bool IsPlaying
        {
            get
            {
                return _playing;
            }
            set
            {
                Set<bool>(ref _playing, value);
            }
        }
        public bool IsBuffering
        {
            get
            {
                return _buffering;
            }
            set
            {
                Set<bool>(ref _buffering, value);
            }
        }
        private void Update(object s, object e)
        {
            //Debug.WriteLine("Updating...");
            GetPositionRequest();
        }
        private void GetPositionRequest()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","GetPosition" }
            });
        }
        private void HasPrevRequest()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","HasPrev" }
            });
        }
        private void HasNextRequest()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","HasNext" }
            });
        }
        private void HasTracksRequest()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","HasTracks" }
            });
        }
        public static void AddTrack(Traccia track, string composer, string opera)
        {
            Debug.WriteLine("AddTrack adding track");
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","AddTrack" },
                { "Title", track.Titolo },
                { "Composer", composer },
                { "Album", opera },
                { "Link", track.Link }
            });
        }
        public static void PlayTrack(Traccia track, string composer, string opera)
        {
            Debug.WriteLine("PlayTrack adding track");
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","PlayTrack" },
                { "Title", track.Titolo },
                { "Composer", composer },
                { "Album", opera },
                { "Link",  track.Link }
            });
        }
        public static void PlayTrack(PlaylistTrack track)
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","PlayTrack" },
                { "Title", track.Track },
                { "Composer", track.Composer },
                { "Album", track.Album },
                { "Link", track.Link }
            });
        }
        public static void AddTracks(List<Traccia> l, string composer, string opera, bool play = false)
        {
            ValueSet vs = new ValueSet();
            vs.Add("Command", play == false ? "AddTracks" : "PlayAlbum");
            vs.Add("Count", l.Count);
            for (int i = 0; i < l.Count; i++)
            {
                Traccia t = l[i];
                vs.Add($"Track{i}_Title", t.Titolo);
                vs.Add($"Track{i}_Composer", composer);
                vs.Add($"Track{i}_Link", t.Link);
                vs.Add($"Track{i}_Album", opera);
            }
            BackgroundMediaPlayer.SendMessageToBackground(vs);
        }
        public static void ChangePlaylist(Playlist playlist, bool play = false)
        {

            ValueSet vs = new ValueSet();
            vs.Add("Command", "ChangePlaylist");
            vs.Add("Count", playlist.List.Count);
            for (int i = 0; i < playlist.List.Count; i++)
            {
                PlaylistTrack t = playlist.List[i];
                vs.Add($"Track{i}_Title", t.Track);
                vs.Add($"Track{i}_Composer", t.Composer);
                vs.Add($"Track{i}_Link", t.Link);
                vs.Add($"Track{i}_Album", t.Album);
            }
            BackgroundMediaPlayer.SendMessageToBackground(vs);
        }
        public static void Init()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Init" }
            });
        }
        public void Play()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Play" }
            });
        }
        public void ItemClicked(object sender, ItemClickEventArgs e)
        {
            PlaylistTrack t = e.ClickedItem as PlaylistTrack;
            int index = Playlist.IndexOf(t);
            PlayAt(index);
        }
        public void PlayAt(int index)
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","PlayIndex" },
                { "Index", index }
            });
        }
        
        public void Pause()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Pause" }
            });
        }
        public void Stop()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Stop" }
            });
        }
        public void Next()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Next" }
            });
        }
        public void Prev()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Prev" }
            });
        }
        public void CleanPlaylistAction(object s, object e)
        {
            CleanPlaylist();
        }
        public static void CleanPlaylist()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","CleanPlaylist" }
            });
        }
        public void RemoveTrackAt(int index)
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","RemTrack" },
                { "Index",index }
            });
        }
        private void MediaPlayerStateChanged(MediaPlayer sender, object args)
        {
            Template10.Common.WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                Debug.WriteLine("Foreground MediaState: " + sender.CurrentState.ToString());
                ReadPlayerCurrentStatus();
            });
        }
        private void RequestCurrentIndex()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","CurrentIndex" }
            });
        }
        private void Tracks()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Tracks" }
            });
        }
        private void RequestTrackInfo()
        {
            HasTracksRequest();
            HasNextRequest();
            HasPrevRequest();
            RequestCurrentIndex();
        }
        private void RequestIsRadioPlaying()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","IsRadioPlaying" }
            });
        }
        private void ReadPlayerCurrentStatus()
        {
            switch (BackgroundMediaPlayer.Current.CurrentState)
            {
                case MediaPlayerState.Playing:
                    IsBuffering = false;
                    IsPlaying = true;
                    if (!dt.IsEnabled)
                        dt.Start();
                    break;
                case MediaPlayerState.Opening:
                case MediaPlayerState.Buffering:
                    IsBuffering = true;
                    break;
                case MediaPlayerState.Paused:
                case MediaPlayerState.Stopped:
                case MediaPlayerState.Closed:
                    IsBuffering = false;
                    IsPlaying = false;
                    dt.Stop();
                    break;
            }
        }

        public async void SavePlaylist(object s, object e)
        {
            if (await PlaylistPageVM.CreateNewPlaylist())
            {
                Playlist playlist = PlaylistManager.Instance.Playlists.Last();
                PlaylistManager.Instance.AddTrackToPlaylist(Playlist.List.ToList(), playlist, true);
            }
            else
            {
                //TODO show error
            }
        }
    }
}
