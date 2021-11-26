using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;

namespace Stratigen.Framework
{
    public interface ICollisionObject
    {
        long ID { get; }
        Vec3 Position { get; set; }
        Vec3 OriginalPosition { get; set; }
        float CollisionRadius { get; set; }
        object Owner { get; set; }
    }
}
