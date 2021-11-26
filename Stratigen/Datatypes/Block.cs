using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Stratigen.Framework;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Stratigen.Datatypes
{
    [DataContract, StructLayout(LayoutKind.Sequential), Serializable]
    public class Block : ICollidable3, ISerializable
    {
        private static ulong _nextID = 0;
        [DataMember]
        public ulong ID;

        [DataMember]
        private Vec3 _position;
        public Vec3 Position 
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                BoundingBox = new Microsoft.Xna.Framework.BoundingBox(_position - HalfSize, _position + HalfSize);
            }
        }
        public float Size
        {
            get
            {
                return HalfSize * 2;
            }
            set
            {
                HalfSize = value / 2;
            }
        }
        [DataMember]
        public float HalfSize;
        [DataMember]
        public float Yaw;
        [DataMember]
        public float Pitch;
        [DataMember]
        public float Roll;

        [DataMember]
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

        public static Plane NegXPlane = new Plane(-1, 0, 0, 0);
        public static Plane PosYPlane = new Plane(0, -1, 0, 0);
        public static Plane PosZPlane = new Plane(0, 0, -1, 0);
        public static Plane PosXPlane = new Plane(1, 0, 0, 1);
        public static Plane NegYPlane = new Plane(0, 1, 0, 1);
        public static Plane NegZPlane = new Plane(0, 0, 1, 1);

        public static Block NullBlock = new Block(Vec3.NNN, BlockType.Get("Test"));

        #region Vertices
        static float ao = .35f;
        static float no = 1;

        public LitTextureVertex[] NegXVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.NegX)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, -Vec3.UnitX, Type.TexSideX, NegXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitX, Type.TexSideY, NegXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitX, Type.TexSideW, NegXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitX, Type.TexSideY, NegXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitX, Type.TexSideZ, NegXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitX, Type.TexSideW, NegXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitX, Type.TexSideZ, NegXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, -Vec3.UnitX, Type.TexSideX, NegXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitX, Type.TexSideY, NegXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitX, Type.TexSideZ, NegXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitX, Type.TexSideW, NegXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, -Vec3.UnitX, Type.TexSideX, NegXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public LitTextureVertex[] NegYVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.NegY)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitY, Type.TexBottomX, NegYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitY, Type.TexBottomY, NegYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitY, Type.TexBottomW, NegYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitY, Type.TexBottomY, NegYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, -Vec3.UnitY, Type.TexBottomZ, NegYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitY, Type.TexBottomW, NegYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, -Vec3.UnitY, Type.TexBottomZ, NegYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitY, Type.TexBottomX, NegYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitY, Type.TexBottomY, NegYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, -Vec3.UnitY, Type.TexBottomZ, NegYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, -Vec3.UnitY, Type.TexBottomW, NegYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitY, Type.TexBottomX, NegYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public LitTextureVertex[] NegZVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.NegZ)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitZ, Type.TexSideX, NegZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, -Vec3.UnitZ, Type.TexSideY, NegZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitZ, Type.TexSideW, NegZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, -Vec3.UnitZ, Type.TexSideY, NegZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitZ, Type.TexSideZ, NegZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitZ, Type.TexSideW, NegZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitZ, Type.TexSideZ, NegZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitZ, Type.TexSideX, NegZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, -Vec3.UnitZ, Type.TexSideY, NegZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, -Vec3.UnitZ, Type.TexSideZ, NegZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNN, -Vec3.UnitZ, Type.TexSideW, NegZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, -Vec3.UnitZ, Type.TexSideX, NegZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public LitTextureVertex[] PosZVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.PosZ)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitZ, Type.TexSideX, PosZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitZ, Type.TexSideY, PosZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitZ, Type.TexSideW, PosZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitZ, Type.TexSideY, PosZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, Vec3.UnitZ, Type.TexSideZ, PosZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitZ, Type.TexSideW, PosZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, Vec3.UnitZ, Type.TexSideZ, PosZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitZ, Type.TexSideX, PosZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitZ, Type.TexSideY, PosZOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NNP, Vec3.UnitZ, Type.TexSideZ, PosZOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitZ, Type.TexSideW, PosZOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitZ, Type.TexSideX, PosZOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public LitTextureVertex[] PosXVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.PosX)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitX, Type.TexSideX, PosXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitX, Type.TexSideY, PosXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, Vec3.UnitX, Type.TexSideW, PosXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitX, Type.TexSideY, PosXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitX, Type.TexSideZ, PosXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, Vec3.UnitX, Type.TexSideW, PosXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitX, Type.TexSideZ, PosXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitX, Type.TexSideX, PosXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitX, Type.TexSideY, PosXOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNP, Vec3.UnitX, Type.TexSideZ, PosXOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PNN, Vec3.UnitX, Type.TexSideW, PosXOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitX, Type.TexSideX, PosXOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public LitTextureVertex[] PosYVertices
        {
            get
            {
                if (Flips.HasFlag(Faces.PosY)) return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitY, Type.TexTopX, PosYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitY, Type.TexTopY, PosYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, Vec3.UnitY, Type.TexTopW, PosYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitY, Type.TexTopY, PosYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitY, Type.TexTopZ, PosYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, Vec3.UnitY, Type.TexTopW, PosYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                };
                else return new LitTextureVertex[6]
                {
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitY, Type.TexTopZ, PosYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitY, Type.TexTopX, PosYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPP, Vec3.UnitY, Type.TexTopY, PosYOcclusion.HasFlag(FaceCorners.Y) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.PPN, Vec3.UnitY, Type.TexTopZ, PosYOcclusion.HasFlag(FaceCorners.Z) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPN, Vec3.UnitY, Type.TexTopW, PosYOcclusion.HasFlag(FaceCorners.W) ? ao : no, Type.Material.Reflectivity),
                    new LitTextureVertex(Position + HalfSize * Vec3.NPP, Vec3.UnitY, Type.TexTopX, PosYOcclusion.HasFlag(FaceCorners.X) ? ao : no, Type.Material.Reflectivity),
                };
            }
        }

        public List<LitTextureVertex> GetVertices()
        {
            List<LitTextureVertex> vertices = new List<LitTextureVertex>();
            if (Facing.HasFlag(Faces.PosX)) vertices.AddRange(PosXVertices);
            if (Facing.HasFlag(Faces.PosY)) vertices.AddRange(PosYVertices);
            if (Facing.HasFlag(Faces.PosZ)) vertices.AddRange(PosZVertices);
            if (Facing.HasFlag(Faces.NegX)) vertices.AddRange(NegXVertices);
            if (Facing.HasFlag(Faces.NegY)) vertices.AddRange(NegYVertices);
            if (Facing.HasFlag(Faces.NegZ)) vertices.AddRange(NegZVertices);
            return vertices;
        }

        #endregion

        [DataMember]
        public bool IsVisible;

        protected Block(SerializationInfo info, StreamingContext ctxt)
        {
            _position = (Vec3)info.GetValue("Position", typeof(Vec3));
            ID = info.GetUInt64("ID");
            HalfSize = info.GetSingle("HalfSize");
            Yaw = info.GetSingle("Yaw");
            Pitch = info.GetSingle("Pitch");
            Roll = info.GetSingle("Roll");
            Box bbox = (Box)info.GetValue("BoundingBox", typeof(Box));
            _boundingBox = new BoundingBox(bbox.Min, bbox.Max);
            Flips = (Faces)info.GetValue("Flips", typeof(Faces));
            NegXOcclusion = (FaceCorners)info.GetValue("NegXOcclusion", typeof(FaceCorners));
            PosXOcclusion = (FaceCorners)info.GetValue("PosXOcclusion", typeof(FaceCorners));
            NegYOcclusion = (FaceCorners)info.GetValue("NegYOcclusion", typeof(FaceCorners));
            PosYOcclusion = (FaceCorners)info.GetValue("PosYOcclusion", typeof(FaceCorners));
            NegZOcclusion = (FaceCorners)info.GetValue("NegZOcclusion", typeof(FaceCorners));
            PosZOcclusion = (FaceCorners)info.GetValue("PosZOcclusion", typeof(FaceCorners));
            IsVisible = info.GetBoolean("IsVisible");
            Facing = (Faces)info.GetValue("Facing", typeof(Faces));
            Type = (BlockType)info.GetValue("Type", typeof(BlockType));
            Health = info.GetSingle("Health");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Position", _position);
            info.AddValue("ID", ID);
            info.AddValue("HalfSize", HalfSize);
            info.AddValue("Yaw", Yaw);
            info.AddValue("Pitch", Pitch);
            info.AddValue("Roll", Roll);
            info.AddValue("BoundingBox", Box.FromMinMax(_boundingBox.Min, _boundingBox.Max));
            info.AddValue("Flips", Flips);
            info.AddValue("NegXOcclusion", NegXOcclusion);
            info.AddValue("PosXOcclusion", PosXOcclusion);
            info.AddValue("NegYOcclusion", NegYOcclusion);
            info.AddValue("PosYOcclusion", PosYOcclusion);
            info.AddValue("NegZOcclusion", NegZOcclusion);
            info.AddValue("PosZOcclusion", PosZOcclusion);
            info.AddValue("IsVisible", IsVisible);
            info.AddValue("Facing", Facing);
            info.AddValue("Type", Type);
            info.AddValue("Health", Health);
        }

        public Block(Vec3 position, BlockType type, float size = 1, float yaw = 0, float pitch = 0, float roll = 0)
        {
            ID = _nextID++;
            HalfSize = size / 2;
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
            Type = type;
            _position = position;
            IsVisible = true; 
            _boundingBox = new Microsoft.Xna.Framework.BoundingBox(_position - HalfSize, _position + HalfSize);
            Facing = Faces.None;
            Flips = Faces.None;
            PosXOcclusion = FaceCorners.None;
            PosYOcclusion = FaceCorners.None;
            PosZOcclusion = FaceCorners.None;
            NegXOcclusion = FaceCorners.None;
            NegYOcclusion = FaceCorners.None;
            NegZOcclusion = FaceCorners.None;
            Health = 100;
        }

        [Flags]
        public enum Faces : byte
        {
            None = 0,
            PosX = 1,
            PosY = 2,
            PosZ = 4,
            NegX = 8,
            NegY = 16,
            NegZ = 32,
            All = 127,
        };

        [Flags]
        public enum FaceCorners : byte
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            W = 8,
            All = 127,
        }

        [DataMember]
        public FaceCorners PosXOcclusion;
        [DataMember]
        public FaceCorners PosYOcclusion;
        [DataMember]
        public FaceCorners PosZOcclusion;
        [DataMember]
        public FaceCorners NegXOcclusion;
        [DataMember]
        public FaceCorners NegYOcclusion;
        [DataMember]
        public FaceCorners NegZOcclusion;

        [DataMember]
        public BlockType Type;
        [DataMember]
        public Faces Facing;
        [DataMember]
        public Faces Flips;
        [DataMember]
        public float Health;
    }
}
    