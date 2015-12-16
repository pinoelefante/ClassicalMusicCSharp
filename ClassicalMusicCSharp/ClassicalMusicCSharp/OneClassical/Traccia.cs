using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.OneClassical
{
    public class Traccia
    {
        public string Titolo { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public Opera Opera { get; set; } = null;
        public Compositore Compositore { get; set; } = null;
    }
}
