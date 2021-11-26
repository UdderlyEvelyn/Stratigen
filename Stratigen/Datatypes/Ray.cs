using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Framework;

namespace Stratigen.Datatypes
{
    public class Ray : ICollisionObject
    {
        private long id = 0;
        private Vec3 _position;
        private Vec3 _originalPosition;
        private Vec3 _direction;
        private float _collisionRadius = 0f;
        private object _owner = null;

        public long ID
        {
            get
            {
                return id;
            }
        }

        public Vec3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Vec3 OriginalPosition
        {
            get
            {
                return _originalPosition;
            }
            set
            {
                _originalPosition = value;
            }
        }

        public Vec3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public float CollisionRadius
        {
            get
            {
                return _collisionRadius;
            }
            set
            {
                _collisionRadius = value;
            }
        }

        public object Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public bool DrawMe { get; set; }

        public Ray(Vec3 position, Vec3 direction)
        {
            Position = position;
            Direction = direction;
            DrawMe = false;
        }
    }
}
