using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Stratigen.Framework;
using System.Runtime.Serialization;
using Stratigen.Libraries;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stratigen.Datatypes
{
    [DataContract]
    public class Chunk : ILoadable, IPersistant, IDisposable
    {
        [DataMember]
        public Vec2 WorldPosition;
        [DataMember]
        public Block[, ,] Blocks = new Block[16, 256, 16];

        public Dictionary<Vec3, Change> Changes = new Dictionary<Vec3, Change>();

        public Microsoft.Xna.Framework.BoundingBox BoundingBox;
        public Vec3 Position;
        public bool Dirty = true;
        public bool Loading = false;
        public bool Menu = false;

        public DynamicVertexBuffer VertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, 1, BufferUsage.WriteOnly);
        public DynamicVertexBuffer TransparencyVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, 1, BufferUsage.WriteOnly);
        public DynamicVertexBuffer LiquidVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, 1, BufferUsage.WriteOnly);
        public World World;

        private Vec2 _region = Vec2.NN;
        public Vec2 Region
        {
            get
            {
                return (_region == Vec2.NN ? (_region = new Vec2(WorldPosition.Xi / 16, WorldPosition.Yi / 16)) : _region);
            }
        }

        public bool UpdateBlock(int x, int y, int z)
        {
            //if (Loading) return false; //ABORT
            if (!InRange(x, y, z)) return false; //ABORT
            Block block = Blocks[x, y, z]; //get block
            if (block == null) return false; //skip null blocks

            #region Update Block Faces
            block.PosYOcclusion = Block.FaceCorners.None;
            block.NegYOcclusion = Block.FaceCorners.None;
            block.PosXOcclusion = Block.FaceCorners.None;
            block.NegXOcclusion = Block.FaceCorners.None;
            block.PosZOcclusion = Block.FaceCorners.None;
            block.NegZOcclusion = Block.FaceCorners.None;

            block.Facing = Block.Faces.None; //reset block to no faces visible
            //Each of the below's logic is "if it's in range for this chunk, look there, otherwise if it's a horizontal direction check the adjacent chunk, otherwise it's null - if it's null, make this face visible and calculate AO."
            bool yPos = y < 255;
            bool yNeg = y > 0;
            bool xPos = x < 15;
            bool xNeg = x > 0;
            bool zPos = z < 15;
            bool zNeg = z > 0;
            bool Xp = false;
            bool Xn = false;
            bool Zp = false;
            bool Zn = false;
            #region Face Culling
            if (yPos)
            {
                Block b = Blocks[x, y + 1, z];
                //if (b == null || (b.Type.Transparency && block.Type.Transparency && (b.Type != block.Type)) || (b.Type.Transparency && !block.Type.Transparency))
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.PosY;
            }
            if (yNeg)
            {
                Block b = Blocks[x, y - 1, z];
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.NegY;
            }
            if (xNeg)
            {
                Block b = Blocks[x - 1, y, z];
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.NegX;
            }
            else
            {
                Block b = World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y, z);
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.NegX;
            }
            if (xPos)
            {
                Block b = Blocks[x + 1, y, z];
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.PosX;
            }
            else
            {
                Block b = World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y, z);
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.PosX;
            }
            if (zNeg)
            {
                Block b = Blocks[x, y, z - 1];
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.NegZ;
            }
            else
            {
                Block b = World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x, y, 15);
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.NegZ;
            }
            if (zPos)
            {
                Block b = Blocks[x, y, z + 1];
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.PosZ;
            }
            else
            {
                Block b = World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x, y, 0);
                if (b == null || (b.Type.Transparency && !block.Type.Transparency) || (b.Type.Transparency && (b.Type.Material.Type != Material.MaterialType.Liquid || block.Type.Material.Type != Material.MaterialType.Liquid)))
                    block.Facing |= Block.Faces.PosZ;
            }
            #endregion
            #region Ambient Occlusion Calculation
            if (!Loading && Loaded)
            {
                //Y+
                //PP_ (P_)
                if ((xPos ? Blocks[x + 1, y + 1, z] : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y + 1, z)) != null)
                {
                    Xp = true;
                    block.PosXOcclusion |= Block.FaceCorners.X | Block.FaceCorners.Y;
                    block.PosYOcclusion |= Block.FaceCorners.Y | Block.FaceCorners.Z;
                }
                //NP_ (N_)
                if ((xNeg ? Blocks[x - 1, y + 1, z] : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y + 1, z)) != null)
                {
                    Xn = true;
                    block.NegXOcclusion |= Block.FaceCorners.X | Block.FaceCorners.Y;
                    block.PosYOcclusion |= Block.FaceCorners.X | Block.FaceCorners.W;
                }
                //_PP (_P)
                if ((zPos ? Blocks[x, y + 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x, y + 1, 0)) != null)
                {
                    Zp = true;
                    block.PosYOcclusion |= Block.FaceCorners.X | Block.FaceCorners.Y;
                    block.PosZOcclusion |= Block.FaceCorners.X | Block.FaceCorners.Y;
                }
                //_PN (_N)
                if ((zNeg ? Blocks[x, y + 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x, y + 1, 15)) != null)
                {
                    Zn = true;
                    block.PosYOcclusion |= Block.FaceCorners.Z | Block.FaceCorners.W;
                    block.NegZOcclusion |= Block.FaceCorners.X | Block.FaceCorners.Y;
                }
                //YPos Corners
                //PPP
                if ((Xp && Zp) || (xPos ? (zPos ? Blocks[x + 1, y + 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x + 1, y + 1, 0)) : (zPos ? World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y + 1, z + 1) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi + 1, 0, y + 1, 0))) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.Y;
                    block.PosYOcclusion |= Block.FaceCorners.Y;
                    block.PosZOcclusion |= Block.FaceCorners.X;
                }
                //PPN
                if ((Xp && Zn) || (xPos ? (zNeg ? Blocks[x + 1, y + 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x + 1, y + 1, 15)) : (zNeg ? World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y + 1, z - 1) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi - 1, 0, y + 1, 15))) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.X;
                    block.PosYOcclusion |= Block.FaceCorners.Z;
                    block.NegZOcclusion |= Block.FaceCorners.Y;
                }
                //NPP
                if ((Xn && Zp) || (xNeg ? (zPos ? Blocks[x - 1, y + 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x - 1, y + 1, 0)) : (zPos ? World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y + 1, z + 1) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi + 1, 15, y + 1, 0))) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.X;
                    block.PosYOcclusion |= Block.FaceCorners.X;
                    block.PosZOcclusion |= Block.FaceCorners.Y;
                }
                //NPN
                if ((Xn && Zn) || (xNeg ? (zNeg ? Blocks[x - 1, y + 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x - 1, y + 1, 15)) : (zNeg ? World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y + 1, z - 1) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi - 1, 15, y + 1, 15))) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.Y;
                    block.PosYOcclusion |= Block.FaceCorners.W;
                    block.NegZOcclusion |= Block.FaceCorners.X;
                }
                Xp = Xn = Zp = Zn = false;
                //Y-
                //PN_ (P_)
                if ((xPos ? Blocks[x + 1, y - 1, z] : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y - 1, z)) != null)
                {
                    Xp = true;
                    block.PosXOcclusion |= Block.FaceCorners.Z;
                    block.PosXOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.Y;
                    block.NegYOcclusion |= Block.FaceCorners.Z;
                }
                //NN_ (N_)
                if ((xNeg ? Blocks[x - 1, y - 1, z] : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y - 1, z)) != null)
                {
                    Xn = true;
                    block.NegXOcclusion |= Block.FaceCorners.Z;
                    block.NegXOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.X;
                    block.NegYOcclusion |= Block.FaceCorners.W;
                }
                //_NP (_P)
                if ((zPos ? Blocks[x, y - 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x, y - 1, 0)) != null)
                {
                    Zp = true;
                    block.PosZOcclusion |= Block.FaceCorners.Z;
                    block.PosZOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.Z;
                    block.NegYOcclusion |= Block.FaceCorners.W;
                }
                //_NN (_N)
                if ((zNeg ? Blocks[x, y - 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x, y - 1, 15)) != null)
                {
                    Zn = true;
                    block.NegZOcclusion |= Block.FaceCorners.Z;
                    block.NegZOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.X;
                    block.NegYOcclusion |= Block.FaceCorners.Y;
                }
                //YNeg Corners            
                //PNP
                if ((Xp && Zp) || (xPos ? (zPos ? Blocks[x + 1, y - 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x + 1, y - 1, 0)) : (zPos ? World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y - 1, z + 1) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi + 1, 0, y - 1, 0))) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.Z;
                    block.NegYOcclusion |= Block.FaceCorners.Z;
                    block.PosZOcclusion |= Block.FaceCorners.W;
                }
                //PNN
                if ((Xp && Zn) || (xPos ? (zNeg ? Blocks[x + 1, y - 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x + 1, y - 1, 15)) : (zNeg ? World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi, 0, y - 1, z - 1) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi - 1, 0, y - 1, 15))) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.Y;
                    block.NegZOcclusion |= Block.FaceCorners.Z;
                }
                //NNP
                if ((Xn && Zp) || (xNeg ? (zPos ? Blocks[x - 1, y - 1, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x - 1, y - 1, 0)) : (zPos ? World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y - 1, z + 1) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi + 1, 15, y - 1, 0))) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.W;
                    block.NegYOcclusion |= Block.FaceCorners.W;
                    block.PosZOcclusion |= Block.FaceCorners.Z;
                }
                //NNN
                if ((Xn && Zn) || (xNeg ? (zNeg ? Blocks[x - 1, y - 1, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x - 1, y - 1, 15)) : (zNeg ? World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi, 15, y - 1, z - 1) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi - 1, 15, y - 1, 15))) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.Z;
                    block.NegYOcclusion |= Block.FaceCorners.X;
                    block.NegZOcclusion |= Block.FaceCorners.W;
                }
                Xp = Xn = Zp = Zn = false;
                //Y
                //P_P
                if ((xPos ? (zPos ? Blocks[x + 1, y, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x + 1, y, 0)) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi + 1, 0, y, 0)) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.Y;
                    block.PosXOcclusion |= Block.FaceCorners.Z;
                    block.PosZOcclusion |= Block.FaceCorners.X;
                    block.PosZOcclusion |= Block.FaceCorners.W;
                }
                //P_N
                if ((xPos ? (zNeg ? Blocks[x + 1, y, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x + 1, y, 15)) : World.GetBlock(WorldPosition.Xi + 1, WorldPosition.Yi - 1, 0, y, 15)) != null)
                {
                    block.PosXOcclusion |= Block.FaceCorners.X;
                    block.PosXOcclusion |= Block.FaceCorners.W;
                    block.NegZOcclusion |= Block.FaceCorners.Y;
                    block.NegZOcclusion |= Block.FaceCorners.Z;
                }
                //N_P
                if ((xNeg ? (zPos ? Blocks[x - 1, y, z + 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi + 1, x - 1, y, 0)) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi + 1, 15, y, 0)) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.X;
                    block.NegXOcclusion |= Block.FaceCorners.W;
                    block.PosZOcclusion |= Block.FaceCorners.Y;
                    block.PosZOcclusion |= Block.FaceCorners.Z;
                }
                //N_N
                if ((xNeg ? (zNeg ? Blocks[x - 1, y, z - 1] : World.GetBlock(WorldPosition.Xi, WorldPosition.Yi - 1, x - 1, y, 15)) : World.GetBlock(WorldPosition.Xi - 1, WorldPosition.Yi - 1, 15, y, 15)) != null)
                {
                    block.NegXOcclusion |= Block.FaceCorners.Y;
                    block.NegXOcclusion |= Block.FaceCorners.Z;
                    block.NegZOcclusion |= Block.FaceCorners.X;
                    block.NegZOcclusion |= Block.FaceCorners.W;
                }
                //Flip Quad If Relevant (Anisotropy Fix)
                if (block.PosXOcclusion.HasFlag(Block.FaceCorners.X) && block.PosXOcclusion.HasFlag(Block.FaceCorners.Z) && (block.PosXOcclusion.HasFlag(Block.FaceCorners.Y) | block.PosXOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.PosX;
                if (block.PosYOcclusion.HasFlag(Block.FaceCorners.X) && block.PosYOcclusion.HasFlag(Block.FaceCorners.Z) && (block.PosYOcclusion.HasFlag(Block.FaceCorners.Y) | block.PosYOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.PosY;
                if (block.PosZOcclusion.HasFlag(Block.FaceCorners.X) && block.PosZOcclusion.HasFlag(Block.FaceCorners.Z) && (block.PosZOcclusion.HasFlag(Block.FaceCorners.Y) | block.PosZOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.PosZ;
                if (block.NegXOcclusion.HasFlag(Block.FaceCorners.X) && block.NegXOcclusion.HasFlag(Block.FaceCorners.Z) && (block.NegXOcclusion.HasFlag(Block.FaceCorners.Y) | block.NegXOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.NegX;
                if (block.NegYOcclusion.HasFlag(Block.FaceCorners.X) && block.NegYOcclusion.HasFlag(Block.FaceCorners.Z) && (block.NegYOcclusion.HasFlag(Block.FaceCorners.Y) | block.NegYOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.NegY;
                if (block.NegZOcclusion.HasFlag(Block.FaceCorners.X) && block.NegZOcclusion.HasFlag(Block.FaceCorners.Z) && (block.NegZOcclusion.HasFlag(Block.FaceCorners.Y) | block.NegZOcclusion.HasFlag(Block.FaceCorners.W))) block.Flips |= Block.Faces.NegZ;
                /*block.PosXFlip = block.PosXOcclusion.X + block.PosXOcclusion.Z > block.PosXOcclusion.Y + block.PosXOcclusion.W;
                block.NegXFlip = block.NegXOcclusion.X + block.NegXOcclusion.Z > block.NegXOcclusion.Y + block.NegXOcclusion.W;
                block.PosYFlip = block.PosYOcclusion.X + block.PosYOcclusion.Z > block.PosYOcclusion.Y + block.PosYOcclusion.W;
                block.NegYFlip = block.NegYOcclusion.X + block.NegYOcclusion.Z > block.NegYOcclusion.Y + block.NegYOcclusion.W;
                block.PosZFlip = block.PosZOcclusion.X + block.PosZOcclusion.Z > block.PosZOcclusion.Y + block.PosZOcclusion.W;
                block.NegZFlip = block.NegZOcclusion.X + block.NegZOcclusion.Z > block.NegZOcclusion.Y + block.NegZOcclusion.W;*/
            }
            #endregion
            #endregion
            if (block.Facing == Block.Faces.None) return block.IsVisible = false; //if no faces are visible, the whole block is invisible (cheaper to skip for culling)
            else return block.IsVisible = true; //if any faces are visible, the block is visible
        }

        public void Update()
        {
            float yMin = 255; //set minimum as highest possible value so that anything will override it
            float yMax = 0; //set maximum as lowest possible value so that anything will override it
            for (int z = 0; z < 16; z++) //iterate Z
                for (int x = 0; x < 16; x++) //iterate X
                {
                    bool topFound = false; //set this boolean for the new column
                    for (int y = 255; y > 0; y--) //iterate Y (top to bottom)
                    {
                        Block b = Blocks[x, y, z]; //get block
                        if (b != null) //skip null blocks
                        {
                            if (!topFound) //if we haven't seen a block in this column yet
                            {
                                //b.Light = 1; //this block is hit by the sun
                                topFound = true; //we now have seen a block
                                if (b.Position.Y > yMax) yMax = b.Position.Y; //if this top block is higher than others we've seen, update the max Y for our bounding box calculations
                            }
                            //else b.Light = 0; //this isn't the top block, so it isn't hit by the sun
                            if (UpdateBlock(x, y, z) && b.Position.Y < yMin) yMin = b.Position.Y;
                        }
                    }
                }
            BuildBoundingBox(yMin, yMax);
        }

        /*public void UpdateLight(int x, int z)
        {
            bool topFound = false; //we haven't found a top block yet
            for (int y = 255; y > 0; y--) //iterate Y (top to bottom)
            {
                Block b = Blocks[x, y, z]; //get block
                if (b != null) //skip null blocks
                {
                    if (!topFound) //if we haven't seen a block in this column yet
                    {
                        b.Light = 1; //this block is hit by the sun
                        topFound = true; //we now have seen a block
                    }
                    else b.Light = 0; //this isn't the top block, so it isn't hit by the sun
                }
            }
        }*/

        /*public void UpdateLight()
        {
            for (int z = 0; z < 16; z++) //iterate Z
                for (int x = 0; x < 16; x++) //iterate X
                    Task.Run(delegate { UpdateLight(x, z); }); //run Y in a new thread since it's stand-alone
        }*/

        public void BuildBoundingBox(float minY, float maxY)
        {
            Vec3 adjust = new Vec3(.5f);
            BoundingBox = new Microsoft.Xna.Framework.BoundingBox(new Vec3(Position.X, minY, Position.Z) - adjust, new Vec3(Position.X + 16, maxY, Position.Z + 16) + adjust);
        }

        public void Build()
        {
            IEnumerable<Block> blocks = Blocks.AsParallel().Cast<Block>().Where(b => b != null && b.IsVisible);
            IEnumerable<Block> opaqueBlocks = blocks.AsParallel().Cast<Block>().Where(b => !b.Type.Transparency && b.Type.Material.Type != Material.MaterialType.Liquid);
            IEnumerable<Block> transparentBlocks = blocks.AsParallel().Cast<Block>().Where(b => b.Type.Transparency && b.Type.Material.Type != Material.MaterialType.Liquid);
            IEnumerable<Block> liquidBlocks = blocks.AsParallel().Cast<Block>().Where(b => b.Type.Material.Type == Material.MaterialType.Liquid);
            List<LitTextureVertex> vertices = new List<LitTextureVertex>();
            foreach (Block b in opaqueBlocks) vertices.AddRange(b.GetVertices());
            if (vertices.Count > 0)
            {
                lock (VertexBuffer)
                {
                    if (vertices.Count > VertexBuffer.VertexCount) VertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
                    VertexBuffer.SetData<LitTextureVertex>(vertices.ToArray(), 0, vertices.Count, SetDataOptions.Discard);
                }
            }
            vertices.Clear();
            foreach (Block b in transparentBlocks) vertices.AddRange(b.GetVertices());
            if (vertices.Count > 0)
            {
                lock (TransparencyVertexBuffer)
                {
                    if (vertices.Count > TransparencyVertexBuffer.VertexCount) TransparencyVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
                    TransparencyVertexBuffer.SetData<LitTextureVertex>(vertices.ToArray(), 0, vertices.Count, SetDataOptions.Discard);
                }
            }
            vertices.Clear();
            foreach (Block b in liquidBlocks) vertices.AddRange(b.GetVertices());
            if (vertices.Count > 0)
            {
                lock (LiquidVertexBuffer)
                {
                    if (vertices.Count > LiquidVertexBuffer.VertexCount) LiquidVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, LitTextureVertex.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
                    LiquidVertexBuffer.SetData<LitTextureVertex>(vertices.ToArray(), 0, vertices.Count, SetDataOptions.Discard);
                }
            }
            Dirty = false;
        }

        public enum Layer
        {
            Opaque,
            Transparent,
            Liquid,
        }

        public bool BlockAt(int x, int y, int z)
        {
            if (!InRange(x, y, z)) return false;
            else return Blocks[x, y, z] != null;
        }

        public bool BlockAt(Vec3 v)
        {
            return BlockAt((int)v.X, (int)v.Y, (int)v.Z);
        }

        public bool InRange(int x, int y, int z)
        {
            if (x > 15) return false;
            if (x < 0) return false;
            if (y > 255) return false;
            if (y < 0) return false;
            if (z > 15) return false;
            if (z < 0) return false;
            return true;
        }

        public Block TopBlock(int x, int z)
        {
            for (int y = 255; y > -1; y--)
            {
                Block b = null;
                try { b = Blocks[x, y, z]; }
                catch { /*Pass*/ }
                //if (b.HasValue) return b.Value;
                if (b != null) return b;
            }
            //Console.WriteLine("No top block at " + x + ", " + z + " of chunk " + WorldPosition.Xi + ", " + WorldPosition.Yi + ".");
            return null;
            //return Blocks.AsParallel().OfType<Block>().OrderByDescending(b => b.Position.Y).First();
        }

        //Doesn't account for the added AO system, and if it did it'd lose the performance boost that made it relevant.
        //public void UpdateAroundRemoved(int x, int y, int z)
        //{
        //    if (InRange(x + 1, y, z))
        //    {
        //        Block b = Blocks[x + 1, y, z];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.NegX;
        //        }
        //    }
        //    if (InRange(x - 1, y, z))
        //    {
        //        Block b = Blocks[x - 1, y, z];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.PosX;
        //        }
        //    }
        //    if (InRange(x, y + 1, z))
        //    {
        //        Block b = Blocks[x, y + 1, z];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.NegY;
        //        }
        //    }
        //    if (InRange(x, y - 1, z))
        //    {
        //        Block b = Blocks[x, y - 1, z];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.PosY;
        //        }
        //    }
        //    if (InRange(x, y, z + 1))
        //    {
        //        Block b = Blocks[x, y, z + 1];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.NegZ;
        //        }
        //    }
        //    if (InRange(x, y, z - 1))
        //    {
        //        Block b = Blocks[x, y, z - 1];
        //        if (b != null)
        //        {
        //            b.IsVisible = true;
        //            b.Facing |= Block.Facings.PosZ;
        //        }
        //    }
        //}

        public void UpdateSurrounding(int x, int y, int z)
        {
            if (InRange(x + 1, y, z))
                UpdateBlock(x + 1, y, z);
            if (InRange(x - 1, y, z))
                UpdateBlock(x - 1, y, z);
            if (InRange(x, y + 1, z))
                UpdateBlock(x, y + 1, z);
            if (InRange(x, y - 1, z))
                UpdateBlock(x, y - 1, z);
            if (InRange(x, y, z + 1))
                UpdateBlock(x, y, z + 1);
            if (InRange(x, y, z - 1))
                UpdateBlock(x, y, z - 1);
        }

        public void UpdateSelfAndSurrounding(int x, int y, int z)
        {
            UpdateBlock(x, y, z);
            UpdateSurrounding(x, y, z);
            UpdateBlock(x, y, z);
        }

        public bool Loaded
        {
            get
            {               
                return World[WorldPosition] == this;
            }
            set
            {
                if (value) Load();
                else Unload();
            }
        }

        public void Unload()
        {
            if (!Menu)
            {
                Save();
                World.ChunkCache[WorldPosition] = new WeakReference<Chunk>(this);
                World[WorldPosition] = null;
                if (World.LoadedChunks.Contains(this)) World.LoadedChunks.Remove(this);
            }
        }

        public void Load()
        {
            World[WorldPosition] = this;
            World.ChunkCache[WorldPosition] = null;
            if (!World.LoadedChunks.Contains(this)) World.LoadedChunks.Add(this);
        }

        public void Save()
        {
            Console.WriteLine("Saving changes for chunk " + WorldPosition.Xi + ", " + WorldPosition.Yi + "..");
            using (TextWriter tw = new StreamWriter(File.Create(FileName)))
            {
                //BinaryFormatter bf = new BinaryFormatter();
                /*for (int cz = 0; cz < 16; cz++) //iterate Z
                    for (int cx = 0; cx < 16; cx++) //iterate X
                    {
                        for (int cy = 255; cy > 0; cy--) //iterate Y (top to bottom)
                        {
                            bf.Serialize(stream, Blocks[cx, cy, cz] ?? Block.NullBlock);
                        }
                    }*/
                foreach (Change c in Changes.Values)
                {
                    tw.WriteLine(c.GetData());
                    //bf.Serialize(stream, c);
                }
            }
        }

        public string FileName
        {
            get
            {
                //return _region.Xi + "-" + _region.Yi + ".ykreg";
                return WorldPosition.Xi + "-" + WorldPosition.Yi + ".ykch";
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool includeManaged)
        {
            VertexBuffer.Dispose();
            TransparencyVertexBuffer.Dispose();
            LiquidVertexBuffer.Dispose();
            if (includeManaged)
            {
                //?
            }
        }

        public void Change(Block block)
        {
            Blocks[block.Position.Xi, block.Position.Yi, block.Position.Zi] = block;
            Changes.Add(block.Position, new Change(block.Position, block.Type.ID));
        }
    }
}
