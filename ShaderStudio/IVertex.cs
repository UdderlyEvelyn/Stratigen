using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderStudio
{
    public interface IVertex
    {
        Vec3 Position { get; set; }
        VertexDeclaration VertexDeclaration { get; }        
    }
}