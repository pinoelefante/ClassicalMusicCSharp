using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Notifications;
using NotificationsExtensions.Tiles; // NotificationsExtensions.Win10

namespace PlayerMultimediale
{
    public sealed class BackgroundPlayer : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private SystemMediaTransportControls systemmediatransportcontrol;
        private Playlist playlist;
        private MediaPlayer player;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize SMTC object to talk with UniversalVolumeControl (UVC)
            // Note that, this is intended to run after app is paused and hence all the logic must be written to run in background process
            systemmediatransportcontrol = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            systemmediatransportcontrol.ButtonPressed += SystemControlsButtonPressed;
            systemmediatransportcontrol.IsEnabled = true;
            systemmediatransportcontrol.IsPauseEnabled = true;
            systemmediatransportcontrol.IsStopEnabled = true;
            systemmediatransportcontrol.IsPlayEnabled = true;
            systemmediatransportcontrol.IsNextEnabled = true;
            systemmediatransportcontrol.IsPreviousEnabled = true;

            // Add handlers for MediaPlayer
            BackgroundMediaPlayer.Current.CurrentStateChanged -= BackgroundMediaPlayerCurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromForeground -= BackgroundMediaPlayerOnMessageReceivedFromForeground;
            BackgroundMediaPlayer.Current.CurrentStateChanged -= MediaPlayerStateChanged;

            BackgroundMediaPlayer.Current.CurrentStateChanged += BackgroundMediaPlayerCurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayerOnMessageReceivedFromForeground;
            BackgroundMediaPlayer.Current.CurrentStateChanged += MediaPlayerStateChanged;

            player = BackgroundMediaPlayer.Current;

            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskIstance_Cancelled;
            if (playlist == null)
                playlist = new Playlist();
        }

        private void MediaPlayerStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine(sender.CurrentState.ToString());
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Paused:
                    if (IsTrackEnded() && !Stopped)
                        NextTrack();
                    break;
            }
        }

        private void TaskIstance_Cancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Stop();
            deferral.Complete();
        }
        private bool Stopped = true;
        private void BackgroundMediaPlayerOnMessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            Debug.WriteLine("Background command received : " + e.Data["Command"].ToString());
            switch (e.Data["Command"].ToString())
            {
                case "Init":
                    break;
                case "Stop":
                    Stop();
                    break;
                case "Play":
                    if (playlist.GetTracksCount() == 0)
                        break;
                    if (Stopped && player.Source == null)
                    {
                        Track t = playlist.Current;
                        PlayTrack(t);
                    }
                    else
                        player.Play();
                    break;
                case "PlayTrack":
                    {
                        var title = e.Data["Title"].ToString();
                        var compo = e.Data["Composer"].ToString();
                        var link = e.Data["Link"].ToString();
                        var album = e.Data["Album"].ToString();
                        Track t = new Track() { Title = title, Album = album, Link = link, Composer = compo };
                        playlist.AddTrack(t);
                        playlist.SetLast();
                        PlayTrack(t);
                    }
                    break;
                case "PlayIndex":
                    {
                        var index = (int)e.Data["Index"];
                        if (index >= 0 && index < playlist.GetTracksCount())
                        {
                            Track track = playlist.TrackAt(index);
                            playlist.SetIndex(index);
                            PlayTrack(track);
                        }
                    }
                    break;
                case "PlayAlbum":
                    {
                        var indexFirstTrack = playlist.GetTracksCount();
                        var count = (int)e.Data["Count"];
                        for (int i = 0; i < count; i++)
                        {
                            var title = e.Data[$"Track{i}_Title"].ToString();
                            var compo = e.Data[$"Track{i}_Composer"].ToString();
                            var link = e.Data[$"Track{i}_Link"].ToString();
                            var album = e.Data[$"Track{i}_Album"].ToString();
                            playlist.AddTrack(new Track() { Title = title, Album = album, Link = link, Composer = compo });
                        }
                        playlist.CurrentIndex = indexFirstTrack;
                        Track cur = playlist.Current;
                        PlayTrack(cur);
                    }
                    break;
                case "PlayRadio":
                    {
                        var title = e.Data["Title"].ToString();
                        var link = e.Data["Link"].ToString();
                        Track t = new Track() { Title = title, Link = link, Composer = "Live Radio" };
                        PlayTrack(t, true);
                    }
                    break;
                case "Pause":
                    Stopped = false;
                    player.Pause();
                    break;
                case "Next":
                    NextTrack();
                    break;
                case "Prev":
                    PrevTrack();
                    break;
                case "Repeat":
                    break;
                case "HasNext":
                    BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                    {
                        { "Command", "HasNext" },
                        { "HasNext", playlist.HasNext.ToString() }
                    });
                    break;
                case "HasPrev":
                    BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                    {
                        { "Command", "HasPrev" },
                        { "HasPrev", playlist.HasPrev.ToString() }
                    });
                    break;
                case "AddTrack":
                    {
                        var title = e.Data["Title"].ToString();
                        var compo = e.Data["Composer"].ToString();
                        var link = e.Data["Link"].ToString();
                        var album = e.Data["Album"].ToString();
                        playlist.AddTrack(new Track() { Title = title, Album = album, Link = link, Composer = compo });
                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "AddTrack" }
                        });
                    }
                    break;
                case "AddTracks":
                    {
                        var count = (int)e.Data["Count"];
                        for (int i = 0; i < count; i++)
                        {
                            var title = e.Data[$"Track{i}_Title"].ToString();
                            var compo = e.Data[$"Track{i}_Composer"].ToString();
                            var link = e.Data[$"Track{i}_Link"].ToString();
                            var album = e.Data[$"Track{i}_Album"].ToString();
                            playlist.AddTrack(new Track() { Title = title, Album = album, Link = link, Composer = compo });
                        }

                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "AddTracks" }
                        });
                    }
                    break;
                case "RemTrack":
                    {
                        var index = (int)e.Data["Index"];
                        if (index < 0 || index >= playlist.GetTracksCount())
                            return;
                        playlist.RemoveTrack(index);
                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "TrackRemoved" },
                            { "Index", index }
                        });
                    }
                    break;
                case "CleanPlaylist":
                    playlist.CleanPlaylist();
                    Stop();
                    BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                    {
                        { "Command", "PlaylistCleaned" }
                    });
                    break;
                case "HasTracks":
                    {
                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "HasTracks" },
                            { "HasTracks", playlist.HasTracks.ToString() }
                        });
                    }
                    break;
                case "Current":
                    Track current = playlist.Current;
                    BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                    {
                        { "Command", "Current" },
                        { "Title", current.Title },
                        { "Composer", current.Composer },
                        { "Album", current.Album },
                        { "Link", current.Link }
                    });
                    break;
                case "CurrentIndex":
                    {
                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "CurrentIndex" },
                            { "CurrentIndex", playlist.CurrentIndex }
                        });
                    }
                    break;
                case "GetPosition":
                    {
                        var length = player.NaturalDuration.TotalSeconds;
                        var position = player.Position.TotalSeconds;
                        BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
                        {
                            { "Command", "GetPosition" },
                            { "Position", (int)position },
                            { "Length", (int)length }
                        });
                    }
                    break;
                case "Tracks":
                    {
                        ValueSet vs = new ValueSet();
                        var count = playlist.GetTracksCount();
                        vs.Add("Command", "Tracks");
                        vs.Add("Count", count);
                        for (int i = 0; i < count; i++)
                        {
                            Track t = playlist.TrackAt(i);
                            vs.Add($"Track{i}_Title", t.Title);
                            vs.Add($"Track{i}_Composer", t.Composer);
                            vs.Add($"Track{i}_Link", t.Link);
                            vs.Add($"Track{i}_Album", t.Album);
                        }
                        BackgroundMediaPlayer.SendMessageToForeground(vs);
                    }
                    break;
            }
        }
        private async void Stop()
        {
            BackgroundMediaPlayer.Current.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
            BackgroundMediaPlayer.Current.Position = BackgroundMediaPlayer.Current.NaturalDuration;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///stop.mp3"));
            BackgroundMediaPlayer.Current.SetFileSource(file);
            DefaultLiveTile();
            Stopped = true;
        }
        private void PlayTrack(Track t, bool pass = false)
        {
            player.SetUriSource(new Uri(t.Link));
            player.Play();
            UpdateAudioController(pass?t:null);
            SetLiveTile(t);
            Stopped = false;
            BackgroundMediaPlayer.SendMessageToForeground(new ValueSet()
            {
                { "Command", "TrackChanged" }
            });
        }
        private async void SetLiveTile(Track t)
        {
            string composer = t.Composer;
            string opera = t.Album;
            string track = t.Title;

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText() { Text = composer, Style = TileTextStyle.Subtitle },
                                new TileText() { Text = opera, Style = TileTextStyle.CaptionSubtle, Wrap = true },
                                new TileText() { Text = track, Style = TileTextStyle.CaptionSubtle }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText() { Text = composer, Style = TileTextStyle.Subtitle },
                                new TileText() { Text = opera, Style = TileTextStyle.CaptionSubtle, Wrap = true },
                                new TileText() { Text = track, Style = TileTextStyle.CaptionSubtle }
                            }
                        }
                    }
                }
            };
            (content.Visual.TileMedium.Content as TileBindingContentAdaptive).BackgroundImage = new TileBackgroundImage() {Source = new TileImageSource(await GetImageTile(t.Composer, TileType.Square)) };
            (content.Visual.TileWide.Content as TileBindingContentAdaptive).BackgroundImage = new TileBackgroundImage() { Source = new TileImageSource (await GetImageTile(t.Composer, TileType.Wide)) };
            var notification = new TileNotification(content.GetXml());

            TileUpdateManager.CreateTileUpdaterForApplication("App").Update(notification);
        }
        private void DefaultLiveTile()
        {
            TileUpdateManager.CreateTileUpdaterForApplication("App").Clear(); //set default live tile
        }
        private void BackgroundMediaPlayerCurrentStateChanged(MediaPlayer sender, object args)
        {
            // Update UVC button state
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Paused:
                    systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaPlayerState.Playing:
                    systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaPlayerState.Stopped:
                    systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaPlayerState.Closed:
                    systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
            }
        }
        private async Task<string> GetImageTile(string composer, string type)
        {
            string to_search = composer.ToLower().Replace(" ", "_") + "_" + type;

            StorageFolder instPath = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder folder = await instPath.GetFolderAsync($@"Assets\composers\{type}");

            IEnumerable<StorageFile> result = (await folder.GetFilesAsync()).Where(f => f.DisplayName.Equals(to_search));
            if(result?.Count() > 0)
            {
                string path = result.ElementAt(0).Path;
                Debug.WriteLine("Path = " + path);
                return path;
            }
            return "ms-appx:///Assets/spartito.jpg";
        }
        private bool IsTrackEnded()
        {
            if (player.CurrentState == MediaPlayerState.Paused && player.NaturalDuration == player.Position)
                return true;
            return false;
        }
        private void UpdateAudioController(Track t = null)
        {
            Track track = t == null ? playlist.Current : t;
            if (track != null)
            {
                systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
                systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = track.Title;
                systemmediatransportcontrol.DisplayUpdater.MusicProperties.Artist = track.Composer;
                systemmediatransportcontrol.IsNextEnabled = playlist.HasNext;
                systemmediatransportcontrol.IsPreviousEnabled = playlist.HasPrev;
                systemmediatransportcontrol.DisplayUpdater.Update();
            }
        }
        private void SystemControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            // Pass UVC commands on to the Background player
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    BackgroundMediaPlayer.Current.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    BackgroundMediaPlayer.Current.Pause();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    BackgroundMediaPlayer.Current.Pause();
                    BackgroundMediaPlayer.Current.Position = new TimeSpan(0);
                    break;
                case SystemMediaTransportControlsButton.Next:
                    NextTrack();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    PrevTrack();
                    break;
            }
        }
        private void NextTrack()
        {
            if (playlist.HasNext)
            {
                Track track = playlist.GetNext();
                PlayTrack(track);
            }
            else
            {
                DefaultLiveTile();
            }
        }
        private void PrevTrack()
        {
            if (playlist.HasPrev)
            {
                Track track = playlist.GetPrev();
                PlayTrack(track);
            }
        }
    }
    class TileType
    {
        public const string Square = "square";
        public const string Wide = "wide";
    }
}
