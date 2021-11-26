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
    public class Chunk3
    {
        public Block[, ,] Blocks;
        public Microsoft.Xna.Framework.BoundingBox BoundingBox;

        public bool Dirty;
        public Vec3 Position;

        public Chunk3(GraphicsDevice graphicsDevice, Vec3 position, List<Instance> instances)
        {
            Position = position;
            BoundingBox = new Microsoft.Xna.Framework.BoundingBox(position, new Vector3(position.X + 16, position.Y + 16, position.Z + 16));
            Blocks = new Block[16, 16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        if (instances.Count(i => i.Position.Xi == x && i.Position.Zi == z) > 0)
                        {
                            Instance i = instances.Where(inst => inst.Position.Xi == x && inst.Position.Zi == z).OrderByDescending(inst => inst.Position.Yi).First();
                            Block b = new Block { Type = Block.Types["Dirt"], Instance = i, Facing = Block.Facings.Up | Block.Facings.Down | Block.Facings.Left | Block.Facings.Right | Block.Facings.Backward | Block.Facings.Forward, Lighting = 255 };
                            Blocks[x, y, z] = b;
                        }
                    }
                }
            }
            /*foreach (Instance i in instances)
            {
                if (BoundingBox.Contains(i.Position) != ContainmentType.Disjoint)
                {
                    Block b = new Block { Type = Block.Types["Dirt"], Instance = i, Facing = Block.Facings.Up | Block.Facings.Down | Block.Facings.Left | Block.Facings.Right | Block.Facings.Backward | Block.Facings.Forward, Lighting = 255 };
                    Blocks[i.Position.Xi - position.Xi, i.Position.Yi - position.Yi, i.Position.Zi - position.Zi] = b;
                    BuildBlockVertices(Vertices, VertexBuffer, b, i.Position);
                }
            }*/
        }

        public bool IsBlock(int x, int y, int z)
        {
            return Blocks[x, y, z] != null;
        }
    }
}
