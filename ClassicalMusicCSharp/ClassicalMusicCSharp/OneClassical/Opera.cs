using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    public class Opera
    {
        private static readonly List<Traccia> EMPTY = new List<Traccia>();
        public string Nome { get; set; } = string.Empty;
        public List<Traccia> Tracce { get; set; } = EMPTY;

        public string Libretto { get; }
        public string Immagine { get; }

        public Categoria Categoria { get; set; }
        public Compositore Compositore { get; set; }
    }
}
