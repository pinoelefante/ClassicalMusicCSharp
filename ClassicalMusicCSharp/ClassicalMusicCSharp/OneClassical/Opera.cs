using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    [DataContract]
    public class Opera
    {
        private static readonly List<Traccia> EMPTY = new List<Traccia>();
        [DataMember]
        public string Nome { get; set; } = string.Empty;
        [DataMember]
        public List<Traccia> Tracce { get; set; } = EMPTY;

        //public string Libretto { get; }
        //public string Immagine { get; }

        //public Categoria Categoria { get; set; }
        //public Compositore Compositore { get; set; }
    }
}
