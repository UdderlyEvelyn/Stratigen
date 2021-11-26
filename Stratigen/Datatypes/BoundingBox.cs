using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Framework;
using Stratigen.Libraries;
//using SharpDX;
//using SharpDX.Toolkit;
//using SharpDX.Toolkit.Graphics;

namespace Stratigen.Datatypes
{
    public class BoundingBox : ICollisionObject
    {
        #region statics

        private static long count;

        #endregion

        #region declarations

        private long id;
        private Vec3 _position;
        private Vec3 _originalPosition;
        private float _collisionRadius;
        private object _owner;
        public float Size = 1;
        public object Tag;

        #endregion

        #region accessors

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

        #endregion

        #region constructors

        public BoundingBox(Vec3 position, float size)
        {
            _position = position;
            _originalPosition = position;
            Size = size;
            _collisionRadius = (float)(size + size * .3);
            id = ++count;
        }

        #endregion
    }
}
