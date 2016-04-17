using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    [DataContract]
    public class Categoria
    {
        [DataMember(Name = "categoria")]
        public string Nome { get; set; }
        [DataMember(Name ="opere")]
        public List<Opera> Opere { get; set; }
    }
}
