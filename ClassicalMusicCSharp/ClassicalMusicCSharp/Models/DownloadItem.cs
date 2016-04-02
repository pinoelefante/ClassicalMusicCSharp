using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.Models
{
    public class DownloadItem : INotifyPropertyChanged
    {
        public string ComposerName { get; set; }
        public string TrackName { get; set; }
        public string OperaName { get; set; }
        public string Url { get; set; }
        public string IdDownload { get; set; }
        private string _progress = string.Empty;
        public string ProgressText { get { return _progress; } set { _progress = value; Notify(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify([CallerMemberName] string p = null)
        {
            if (p != null && PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
