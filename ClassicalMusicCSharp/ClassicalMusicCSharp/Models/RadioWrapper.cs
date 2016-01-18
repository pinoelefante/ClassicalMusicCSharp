using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.Models
{
    public class RadioWrapper
    {
        [DataContract]
        public class Radio
        {
            [DataMember]
            public string title { get; set; }
            [DataMember]
            public string descr { get; set; }
            [DataMember]
            public string link { get; set; }
            [DataMember]
            public string image { get; set; }
        }
        [DataContract]
        public class RootObject
        {
            [DataMember]
            public List<Radio> radio { get; set; }
        }
    }
}
