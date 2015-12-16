using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;

namespace PlayerMultimediale
{
    public sealed class BackgroundPlayer : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private SystemMediaTransportControls systemmediatransportcontrol;
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

            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskIstance_Cancelled;
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

        private void BackgroundMediaPlayerOnMessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            
            switch (e.Data["Command"].ToString())
            {
                case "Stop":
                    //toStop = true;
                    Stop();
                    return;
                case "Play":
                    BackgroundMediaPlayer.Current.SetUriSource(new Uri(e.Data["Source"].ToString()));
                    // Update the UVC text
                    systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
                    systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = e.Data["Title"].ToString();
                    systemmediatransportcontrol.DisplayUpdater.MusicProperties.Artist = e.Data["Artist"].ToString();
                    systemmediatransportcontrol.IsNextEnabled = (bool)e.Data["HasNext"];
                    systemmediatransportcontrol.IsPreviousEnabled = (bool)e.Data["HasPrev"];
                    systemmediatransportcontrol.DisplayUpdater.Update();
                    //Task.Delay(500);
                    BackgroundMediaPlayer.Current.Play();
                    break;
                case "Pause":
                    BackgroundMediaPlayer.Current.Pause();
                    break;
            }
            //toStop = false;
        }
        //private bool toStop = false; //serve per determinare se al termine della traccia deve essere riprodotta la successiva
        private async void Stop()
        {
            BackgroundMediaPlayer.Current.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
            BackgroundMediaPlayer.Current.Position = BackgroundMediaPlayer.Current.NaturalDuration;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///stop.mp3"));
            BackgroundMediaPlayer.Current.SetFileSource(file);
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

        private static void SystemControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
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
