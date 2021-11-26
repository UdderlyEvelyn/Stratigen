using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stratigen.Framework;

namespace Stratigen.Datatypes
{
    public class Sprite : ICollidable2
    {
        private Vec2 _position;

        public Vec2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                GenerateBoundingBox();
            }
        }
        
        public Vec2 Rotation;
        public Texture2D Data;
        public BoundingBox BoundingBox { get; set; }

        public void GenerateBoundingBox()
        {
            BoundingBox = new BoundingBox(new Vector3(Position.ToXNA(), 0), new Vector3(Position.X + Data.Width * Scale, Position.Y + Data.Height * Scale, 0));
        }

        public float Scale = 1;
    }
}
