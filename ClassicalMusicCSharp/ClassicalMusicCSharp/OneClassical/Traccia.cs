﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    [DataContract]
    public class Traccia
    {
        [DataMember(Name ="titolo")]
        public string Titolo { get; set; } = string.Empty;
        [DataMember(Name ="link")]
        public string Link { get; set; } = string.Empty;
        //public Opera Opera { get; set; } = null;
        //public Compositore Compositore { get; set; } = null;
    }
}
