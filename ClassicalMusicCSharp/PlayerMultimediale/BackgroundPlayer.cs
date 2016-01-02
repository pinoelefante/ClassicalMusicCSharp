using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;

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
        }

        private void TaskIstance_Cancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Stop();
            deferral.Complete();
        }
        private bool Stopped = true;
        private void BackgroundMediaPlayerOnMessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            
            switch (e.Data["Command"].ToString())
            {
                case "Init":
                    break;
                case "Stop":
                    Stop();
                    break;
                case "Play":
                    if (Stopped && player.Source == null)
                    {
                        Stopped = false;
                        Track t = playlist.Current;
                        PlayTrack(t);
                    }
                    else
                        player.Play();
                    break;
                case "Pause":
                    Stopped = false;
                    player.Pause();
                    break;
                case "Next":
                    Stopped = false;
                    if (playlist.HasNext)
                    {
                        Track t = playlist.GetNext();
                        PlayTrack(t);
                    }
                    break;
                case "Prev":
                    Stopped = false;
                    if (playlist.HasPrev)
                    {
                        Track t = playlist.GetPrev();
                        PlayTrack(t);
                    }
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
                case "RemTrack":
                    {
                        var index = (int)e.Data["Index"];
                        playlist.RemoveTrack(index);
                    }
                    break;
                case "CleanPlaylist":
                    playlist.CleanPlaylist();
                    Stop();
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
                            { "CurrentIndex", playlist.CurrentIndex.ToString() }
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

                    break;
            }
        }
        private async void Stop()
        {
            BackgroundMediaPlayer.Current.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
            BackgroundMediaPlayer.Current.Position = BackgroundMediaPlayer.Current.NaturalDuration;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///stop.mp3"));
            BackgroundMediaPlayer.Current.SetFileSource(file);
            Stopped = true;
        }
        private void PlayTrack(Track t)
        {
            player.SetUriSource(new Uri(t.Link));
            player.Play();
            UpdateAudioController();
        }
        private void BackgroundMediaPlayerCurrentStateChanged(MediaPlayer sender, object args)
        {
            // Update UVC button state
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }
        private void UpdateAudioController()
        {
            Track track = playlist.Current;
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
                    BackgroundMediaPlayer.SendMessageToForeground(new Windows.Foundation.Collections.ValueSet()
                    {
                        { "Command","Next" }
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    BackgroundMediaPlayer.SendMessageToForeground(new Windows.Foundation.Collections.ValueSet()
                    {
                        { "Command","Prev" }
                    });
                    break;
            }
        }
    }
}
