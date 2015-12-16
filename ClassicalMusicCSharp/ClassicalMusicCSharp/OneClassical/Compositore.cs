using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    public class Compositore
    {
        public string Nome { get; set; }
        public List<Categoria> Categorie { get; set; }
        public List<Opera> Opere { get; set; }

        public Boolean HasCategorie
        {
            get
            {
                return Categorie != null;
            }
        }
        public string Immagine { get; set; } = string.Empty;
    }
}
