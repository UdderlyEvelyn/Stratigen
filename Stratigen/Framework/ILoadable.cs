using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;

namespace Stratigen.Framework
{
    public interface ILoadable
    {
        bool Loaded { get; set; }
        void Unload();
        void Load();
    }
}
