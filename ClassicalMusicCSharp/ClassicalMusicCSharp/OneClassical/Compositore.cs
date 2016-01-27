using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassicalMusicCSharp.OneClassical
{
    [DataContract]
    public class Compositore
    {
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public List<Categoria> Categorie { get; set; }
        [DataMember]
        public List<Opera> Opere { get; set; }

        public Boolean HasCategorie
        {
            get
            {
                return Categorie != null;
            }
        }
        public string Image { get; set; }
    }
}
