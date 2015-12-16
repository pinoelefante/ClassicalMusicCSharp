using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    public class Categoria
    {
        public string Nome { get; set; }
        public Compositore Compositore { get; set; }
        public List<Opera> Opere { get; set; }
    }
}
