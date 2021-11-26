using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stratigen.Datatypes;
using Stratigen.Libraries;
using Microsoft.Xna.Framework.Input;
using Ray = Microsoft.Xna.Framework.Ray;

namespace Stratigen.Framework
{
    public class RenderCamera : Camera, ICollidable3
    {
        public Vec3 Velocity = Vec3.Zero;

        public float LegLength = 2;

        private BoundingBox _boundingBox;

        public BoundingBox BoundingBox
        {
            get
            {
                return _boundingBox;
            }
            set
            {
                _boundingBox = value;
            }
        }

        private BoundingBox _horizontalBoundingBox;

        public BoundingBox HorizontalBoundingBox
        {
            get
            {
                return _horizontalBoundingBox;
            }
        }

        public Ray UpRay
        {
            get
            {
                return new Ray(_position, Vec3.Up);
            }
        }

        public Ray DownRay
        {
            get
            {
                return new Ray(new Vec3(_position.X, (_position.Y - LegLength), _position.Z), Vec3.Down);
            }
        }

        public Ray[] DownRays
        {
            get
            {
                Vec3 c1 = new Vec3(_boundingBox.Min.X, (_position.Y - LegLength), _boundingBox.Min.Z);
                Vec3 c2 = new Vec3(_boundingBox.Min.X, (_position.Y - LegLength), _boundingBox.Max.Z);
                Vec3 c3 = new Vec3(_boundingBox.Max.X, (_position.Y - LegLength), _boundingBox.Min.Z);
                Vec3 c4 = new Vec3(_boundingBox.Max.X, (_position.Y - LegLength), _boundingBox.Max.Z);
                return new Ray[4]
                {
                    new Ray(c1, Vec3.Down),
                    new Ray(c2, Vec3.Down),
                    new Ray(c3, Vec3.Down),
                    new Ray(c4, Vec3.Down),
                };
            }
        }

        public Vec2 ChunkCoordinates
        {
            get
            {
                return new Vec2(_position.X / 16, _position.Z / 16);
            }
        }
        public Vec3 LocalCoordinates
        {
            get
            {
                return new Vec3(_position.X % 16, _position.Y, _position.Z % 16);
            }
        }
        public void Move(Vec3 v)
        {
            Position = PreviewMove(v);
        }

        public Vec3 PreviewMove(Vec3 v)
        {
            return _position + (Vec3)Vector3.Transform(v, Matrix.CreateRotationY(_rotation.Y));
        }

        public void MoveXZ(Vec3 v)
        {
            Position = PreviewMoveXZ(v);
        }

        public Vec3 PreviewMoveXZ(Vec3 v)
        {
            Vec3 result = Vector3.Transform(v, Matrix.CreateRotationY(_rotation.Y));
            return new Vec3(_position.X + result.X, _position.Y, _position.Z + result.Z);
        }

        public void DiscardVelocity(bool x, bool y, bool z)
        {
            Velocity = new Vec3(x ? 0 : Velocity.X, y ? 0 : Velocity.Y, z ? 0 : Velocity.Z);
        }

        public RenderCamera(Game game) : base(game)
        {

        }

        public new Vec3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateBoundingBox();
                UpdateLookAt();
            }
        }

        public float MoveSpeed = 10f;
        public float MouseSpeed = .1f;

        public void UpdateBoundingBox()
        {
            float divisor = 5; //make the horizontal dimensions of the bounding box smaller so it can fit through "cracks", or diagonals between blocks
            _boundingBox.Min = new Vector3(_position.X - (Size / divisor), _position.Y - LegLength, _position.Z - (Size / divisor));
            _boundingBox.Max = new Vector3(_position.X + (Size / divisor), _position.Y, _position.Z + (Size / divisor));
            _horizontalBoundingBox = _boundingBox;
            _horizontalBoundingBox.Min.Y = _position.Y;
            _horizontalBoundingBox.Max.Y = _position.Y;
        }

        public float Size = 1;
    }
}
