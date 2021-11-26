using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;
using Microsoft.Xna.Framework.Graphics;

namespace Stratigen.Framework
{
    public interface IVertex
    {
        Vec3 Position { get; set; }
        VertexDeclaration VertexDeclaration { get; }        
    }
}