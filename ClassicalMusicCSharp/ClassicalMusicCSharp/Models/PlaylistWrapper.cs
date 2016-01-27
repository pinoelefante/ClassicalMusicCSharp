using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.Models
{
    public class PlaylistWrapper
    {
        [DataContract]
        public class PlaylistExternal
        {
            [DataMember]
            public int currentId { get; set; }
            [DataMember]
            public List<string> playlists { get; set; }
        }

        public class TrackJson
        {
            public string composer { get; set; }
            public string album { get; set; }
            public string track { get; set; }
            public string link { get; set; }
        }

        public class PlaylistJson
        {
            public int id { get; set; }
            public List<TrackJson> tracks { get; set; }
        }
    }
}
