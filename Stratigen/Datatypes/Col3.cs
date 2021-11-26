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
    public struct Col3
    {
        [DataMember]
        public float R;
        [DataMember]
        public float G;
        [DataMember]
        public float B;

        public Col3(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static implicit operator Color(Col3 c)
        {
            return new Color(c.R, c.G, c.B);
        }

        public static implicit operator Col3(Color c)
        {
            return new Col3(c.R, c.G, c.B);
        }

        public static readonly Col3 White = Color.White;
        public static readonly Col3 Red = Color.Red;
        public static readonly Col3 Blue = Color.Blue;
        public static readonly Col3 Green = Color.Green;
        public static readonly Col3 Black = Color.Black;
    }
}
