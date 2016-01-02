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

namespace ClassicalMusicCSharp.ViewModels
{
    public class PlayerPageVM : Mvvm.ViewModelBase
    {
        private DispatcherTimer dt;
        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += MessageReceived;

            dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            dt.Tick += Update;
            dt.Start();
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground -= MessageReceived;
            dt.Stop();
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
                    case "Current":

                        break;
                    case "CurrentIndex":
                        CurrentIndex = int.Parse(e.Data["CurrentIndex"].ToString());
                        break;
                    case "GetPosition":
                        var length = (int)e.Data["Length"] < 1 ? 1 : (int)e.Data["Length"];
                        CurrentLength = length;
                        CurrentPosition = (int)e.Data["Position"];
                        Debug.WriteLine($"Positions {CurrentPosition}/{CurrentLength}");
                        break;
                    case "AddTrack":
                        Debug.WriteLine("Track added");
                        break;
                    case "Tracks":

                        break;
                }
            });
        }
        private bool _hasNext, _hasPrev, _hasTracks;
        private int _curIndex;
        private int _curPos, _curLength;
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
        private void Update(object s, object e)
        {
            Debug.WriteLine("Updating...");
            HasTracksRequest();
            HasNextRequest();
            HasPrevRequest();
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
        public static async Task AddTrack(Traccia track)
        {
            Debug.WriteLine("AddTrack adding track");
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","AddTrack" },
                { "Title", track.Titolo },
                { "Composer", track.Compositore.Nome },
                { "Album", track.Opera.Nome },
                { "Link", track.Link }
            });
            await Task.Delay(1);
        }
        public async Task AddTracks(List<Traccia> l)
        {
            foreach(Traccia t in l)
            {
                await AddTrack(t);
            }
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
    }
    public class PlaylistTrack
    {
        public string Composer { get; set; }
        public string Track { get; set; }
        public string Link { get; set; }
        public string Album { get; set; }
    }
}
