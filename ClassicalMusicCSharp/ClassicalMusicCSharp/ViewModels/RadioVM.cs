using ClassicalMusicCSharp.Classes.Radio;
using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassicalMusicCSharp.ViewModels
{
    public class RadioVM : Mvvm.ViewModelBase
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += MessageReceived;
            BackgroundMediaPlayer.Current.CurrentStateChanged += MediaPlayerStateChanged;
            if (!RadioManager.IsLoaded)
                await RadioManager.LoadRadio();
            RadioList = RadioManager.Radios.radio;
            RequestIsRadioPlaying();
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground -= MessageReceived;
            BackgroundMediaPlayer.Current.CurrentStateChanged -= MediaPlayerStateChanged;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        private List<RadioWrapper.Radio> _list;
        public List<RadioWrapper.Radio> RadioList
        {
            get
            {
                return _list;
            }
            set
            {
                Set<List<RadioWrapper.Radio>>(ref _list, value);
            }
        }
        public void RadioSelected(object sender, ItemClickEventArgs e)
        {
            RadioWrapper.Radio radio = e.ClickedItem as RadioWrapper.Radio;
            Debug.WriteLine("Selezionata " + radio?.title);
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                {"Command", "PlayRadio"},
                {"Title", radio.title },
                {"Link", radio.link }
            });
            RequestIsRadioPlaying();
        }
        private void RequestIsRadioPlaying()
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","IsRadioPlaying" }
            });
        }
        public void Stop(object s, object e)
        {
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet()
            {
                { "Command","Stop" }
            });
        }
        private void MessageReceived(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            Template10.Common.WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                switch (e.Data["Command"].ToString())
                {
                    case "IsRadioPlaying":
                        {
                            RadioPlaying = (bool)e.Data["IsRadioPlaying"];
                            if (RadioPlaying)
                            {
                                RadioTitle = e.Data["Title"].ToString();
                            }
                        }
                        break;
                    case "TrackChanged":
                    case "Stopped":
                        RequestIsRadioPlaying();
                        break;
                }
            });
        }
        private void MediaPlayerStateChanged(MediaPlayer sender, object args)
        {
            if(sender.CurrentState == MediaPlayerState.Playing)
            {
                RequestIsRadioPlaying();
            }
        }
        private bool _radioPlaying;
        public bool RadioPlaying
        {
            get
            {
                return _radioPlaying;
            }
            set
            {
                Set<bool>(ref _radioPlaying, value);
            }
        }
        private string _radioTitle;
        public string RadioTitle
        {
            get
            {
                return _radioTitle;
            }
            set
            {
                Set<string>(ref _radioTitle, value);
            }
        }
    }
}
