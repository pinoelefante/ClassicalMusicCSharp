using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMultimediale
{
    public sealed class Playlist
    {
        public Playlist()
        {
            _listTracks = new List<Track>();
            _currentTrack = 0;
        }
        private List<Track> _listTracks;
        private int _currentTrack;
        public int CurrentIndex
        {
            get
            {
                return _currentTrack;
            }
        }
        public bool HasNext {
            get
            {
                if (_currentTrack < _listTracks.Count - 1)
                    return true;
                return false;
            }
        }
        public bool HasPrev
        {
            get
            {
                if (_currentTrack > 0)
                    return true;
                return false;
            }
        }
        public void AddTrack(Track t)
        {
            _listTracks.Add(t);
        }
        public void RemoveTrack(int index)
        {
            if (index <= _currentTrack)
                _currentTrack--;
            _listTracks.RemoveAt(index);
        }
        public Track GetNext()
        {
            _currentTrack++;
            _currentTrack = _currentTrack % _listTracks.Count;
            return _listTracks[_currentTrack];
        }
        public Track GetPrev()
        {
            _currentTrack--;
            _currentTrack = _currentTrack % _listTracks.Count;
            return _listTracks[_currentTrack];
        }
        public Track Current
        {
            get
            {
                if(HasTracks)
                    return _listTracks[_currentTrack];
                return null;
            }
        }
        public void CleanPlaylist()
        {
            _listTracks.Clear();
            _currentTrack = 0;
        }
        public bool HasTracks
        {
            get
            {
                return _listTracks.Count > 0;
            }
        }
    }
}
