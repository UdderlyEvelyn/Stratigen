using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;
using Microsoft.Xna.Framework;

namespace Stratigen.Framework
{
    public interface ICollidable3
    {
        Vec3 Position { get; set; }
        BoundingBox BoundingBox { get; set; }
    }
}
