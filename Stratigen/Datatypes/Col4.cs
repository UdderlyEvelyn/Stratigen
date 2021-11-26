using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Stratigen.Datatypes
{
    [DataContract]
    public struct Col4
    {
        [DataMember]
        public float A;
        [DataMember]
        public float R;
        [DataMember]
        public float G;
        [DataMember]
        public float B;

        public Col4(float r, float g, float b, float a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator Color(Col4 c)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }

        public static implicit operator Col4(Color c)
        {
            return new Col4(c.R, c.G, c.B, c.A);
        }

        public static implicit operator Col3(Col4 c)
        {
            return new Col3(c.R, c.G, c.B);
        }

        public static implicit operator Col4(Col3 c)
        {
            return new Col4(c.R, c.G, c.B, Color.White.A);
        }

        public static readonly Col4 White = Color.White;
        public static readonly Col4 Red = Color.Red;
        public static readonly Col4 Blue = Color.Blue;
        public static readonly Col4 Green = Color.Green;
        public static readonly Col4 Black = Color.Black;
    }
}
