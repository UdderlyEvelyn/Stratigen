using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ShaderStudio
{
    public static class Globals
    {
        public static Random Random = new Random();
        public static Vec3 Rotation = new Vec3(0, 0, 0);
        public static Vec3 LightPosition = new Vec3(-1, 1, -7);//new Vec3(-5, 1, -8);
        public static Vec3 CameraPosition = new Vec3(0, -8, -8);
        public static Vec3 TrianglePosition = new Vec3(0, 0, 0);
    }
}
