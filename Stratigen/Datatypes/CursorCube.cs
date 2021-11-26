using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Stratigen.Datatypes
{
    class CursorCube : Cube
    {
        public new PrimitiveType? DrawMode = PrimitiveType.Lines;
    }
}
