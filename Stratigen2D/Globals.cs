using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Framework;
/*using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;*/
/*using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;*/
using Stratigen.Datatypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stratigen2D
{
    public static class Globals
    {
        public static Stratigen.Datatypes.Model Cube;
        public static Random Random = new Random();
        public static Camera Camera;
        public static Scene Scene;
        public static float CameraHeightOffset = -10; 
        public static bool VertexGroups = false;
        public static int ProcessedCubes = 0;
        public static int ChunkRenderCount = 0;
        public static GraphicsDevice GraphicsDevice;
        public static int ChunkHeight = 256;
        public static int ChunkSize = 16;
        public static int Width = 800;
        public static int Height = 600;
        public static RenderTarget2D OcclusionTarget;
        public static Effect Effect;
        public static bool Test = false;
    }
}
