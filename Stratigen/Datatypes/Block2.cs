using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratigen.Datatypes
{
    public class Block2
    {
        public static BlockType[] Types = new BlockType[2]
        {
            new BlockType { Drops = false, Hardness = 0, Name = "Air", BottomTexture = null, SideTexture = null, TopTexture = null },
            new BlockType { Drops = true, Hardness = 1, Name = "Dirt", BottomTexture = null, SideTexture = null, TopTexture = null },
        };

        public enum Facings : byte
        {
            Right = 0,
            Left = 1,
            Up = 2,
            Down = 3,
            Forward = 4,
            Backward = 5,
        };

        public BlockType Type;
        public Facings Facing;
        public float Lighting;
    }
}
