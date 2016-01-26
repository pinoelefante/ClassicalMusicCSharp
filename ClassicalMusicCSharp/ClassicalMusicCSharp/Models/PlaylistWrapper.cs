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
        [DataContract]
        public class TrackJson
        {
            [DataMember]
            public string composer { get; set; }
            [DataMember]
            public string album { get; set; }
            [DataMember]
            public string track { get; set; }
            [DataMember]
            public string link { get; set; }
        }
        [DataContract]
        public class PlaylistJson
        {
            [DataMember]
            public List<TrackJson> tracks { get; set; }
            [DataMember]
            public int id { get; set; }
        }
    }
}
