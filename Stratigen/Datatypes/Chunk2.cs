using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stratigen.Libraries;

namespace Stratigen.Datatypes
{
    public class Chunk2
    {
        public Block2[,,] Blocks;

        public Vertex[] Vertices;
        public Index[] Indices;

        public GraphicsDevice GraphicsDevice;
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public bool Dirty;

        public Chunk2(GraphicsDevice graphicsDevice, Array2<byte> heightField = null)
        {
            Blocks = new Block2[16, 16, 16];
            int vertexCount = 36 * Blocks.Length;
            Vertices = new Vertex[vertexCount];
            //Indices currently unused.
            VertexBuffer = new VertexBuffer(GraphicsDevice = graphicsDevice, Vertex.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            InitializeChunk();
        }

        public void InitializeChunk(Array2<byte> heightField = null)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        Block2 b = new Block2 { Type = Block2.Types[0], Facing = Block2.Facings.Up, Lighting = 255 };
                        Blocks[x, y, z] = b;
                        BuildBlockVertices(Vertices, VertexBuffer, b, new Vec3(x, y, z));
                    }
                }
            }
        }

        public void BuildVertices()
        {

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        BuildBlockVertices(Vertices, VertexBuffer, Blocks[x, y, z], new Vec3(x, y, z));
                    }
                }
            }
            Dirty = false;
        }

        private void BuildBlockVertices(Vertex[] vertices, VertexBuffer buffer, Block2 block, Vec3 position)
        {
            if (((byte)block.Facing & (byte)Block2.Facings.Left) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
            if (((byte)block.Facing & (byte)Block2.Facings.Right) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
            if (((byte)block.Facing & (byte)Block2.Facings.Down) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
            if (((byte)block.Facing & (byte)Block2.Facings.Up) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
            if (((byte)block.Facing & (byte)Block2.Facings.Backward) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
            if (((byte)block.Facing & (byte)Block2.Facings.Forward) > 0) BuildFaceVertices(position, buffer, ref vertices, block);
        }

        private void BuildFaceVertices(Vec3 position, VertexBuffer buffer, ref Vertex[] vertices, Block2 block)
        {
            int index = (int)block.Facing;
            switch (block.Facing)
            {
                case Block2.Facings.Right:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z), block.Lighting.ToColor());
                    }
                    break;

                case Block2.Facings.Left:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X, position.Y, position.Z), block.Lighting.ToColor());
                    }
                    break;

                case Block2.Facings.Up:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                    }
                    break;

                case Block2.Facings.Down:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X, position.Y, position.Z), block.Lighting.ToColor());
                    }
                    break;

                case Block2.Facings.Forward:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z + 1), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X, position.Y, position.Z + 1), block.Lighting.ToColor());
                    }
                    break;

                case Block2.Facings.Backward:
                    {
                        vertices[index] = new Vertex(new Vec3(position.X + 1, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 1] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 2] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z), block.Lighting.ToColor());
                        vertices[index + 3] = new Vertex(new Vec3(position.X + 1, position.Y, position.Z), block.Lighting.ToColor());
                        vertices[index + 4] = new Vertex(new Vec3(position.X, position.Y + 1, position.Z), block.Lighting.ToColor());
                        vertices[index + 5] = new Vertex(new Vec3(position.X, position.Y, position.Z), block.Lighting.ToColor());
                    }
                    break;
            }
            index += 6;
        }
    }
}