using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Framework;
using Stratigen.Datatypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stratigen
{
    public static class Globals
    {
        public static Kernel Kernel;
        public static Random Random = new Random();
        public static Camera Camera;
        public static Scene Scene;
        public static int ProcessedCubes = 0;
        public static int VisibleCubes = 0;
        public static int ChunkRenderCount = 0;
        public static GraphicsDevice GraphicsDevice;
        public static int ChunkHeight = 256;
        public static int ChunkSize = 16;
        public static int Width = 800;
        public static int Height = 600;
        public static Effect Effect;
        public static int ChunkCounter = 0;
        //public static bool FPSGraph = false;
    }
}
