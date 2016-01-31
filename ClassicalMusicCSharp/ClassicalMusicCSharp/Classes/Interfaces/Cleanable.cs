using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicalMusicCSharp.Classes.Interfaces
{
    public interface Cleanable
    {
        void Clean();
        void Reload();
        bool IsLoaded { get; set; }
    }
}
