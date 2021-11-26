using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using Stratigen.Framework;

namespace Stratigen.Datatypes
{
    [DataContract, StructLayout(LayoutKind.Sequential)]
    public struct TextureVertex : IVertexType, IVertex
    {
        [DataMember]
        private Vec3 _position;
        [DataMember]
        private Vec3 _normal;
        [DataMember]
        private Vec2 _textureCoordinate;

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
        public Vec3 Normal 
        {
            get
            {
                return _normal;
            }
            set
            {
                _normal = value;
            }
        }
        public Vec2 TextureCoordinate 
        {
            get
            {
                return _textureCoordinate;
            }
            set
            {
                _textureCoordinate = value;
            }
        }

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        VertexDeclaration IVertex.VertexDeclaration { get { return VertexDeclaration; } }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
        );

        public TextureVertex(float x, float y, float z, float nx, float ny, float nz, float tx, float ty)
        {
            _position = new Vec3(x, y, z);
            _normal = new Vec3(nx, ny, nz);
            _textureCoordinate = new Vec2(tx, ty);
        }

        public TextureVertex(Vec3 v, Vec3 n, Vec2 t)
        {
            _position = v;
            _normal = n;
            _textureCoordinate = t;
        }

        public TextureVertex(float x, float y, float z, float tx, float ty)
        {
            _position = new Vec3(x, y, z);
            _normal = _position;
            _normal.Normalize();
            _textureCoordinate = new Vec2(tx, ty);
        }

        public TextureVertex(Vec3 v, Vec2 t)
        {
            _position = v;
            _normal = v;
            _normal.Normalize();
            _textureCoordinate = t;
        }
    }
}
