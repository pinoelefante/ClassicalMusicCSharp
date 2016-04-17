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
        [DataMember(Name = "nome")]
        public string Nome { get; set; }
        [DataMember(Name = "categorie")]
        public List<Categoria> Categorie { get; set; }
        [DataMember(Name = "opere")]
        public List<Opera> Opere { get; set; }

        public Boolean HasCategorie
        {
            get
            {
                return Categorie != null;
            }
        }
        public string Image
        {
            get
            {
                return $"ms-appx:///Assets/composers/square/{Nome.ToLower().Replace(" ", "_")}_square.jpg";
            }
        }
        public string ImageWide
        {
            get
            {
                return $"ms-appx:///Assets/composers/square/{Nome.ToLower().Replace(" ", "_")}_wide.jpg";
            }
        }
    }
}
