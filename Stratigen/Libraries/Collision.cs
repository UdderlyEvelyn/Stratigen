using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;
using Stratigen.Framework;

namespace Stratigen.Libraries
{
    public static class Collision
    {
        public static List<ICollisionObject> CollisionObjects = new List<ICollisionObject>();

        /// <summary>
        /// Detect collisions along a ray.
        /// </summary>
        /// <param name="r">the ray to check</param>
        /// <param name="limit">maximum distance to take objects into account from</param>
        /// <param name="increment">the amount of distance along the ray to move between each check</param>
        /// <returns>the first ICollisionObject in the collision system that was encountered</returns>
        public static ICollisionObject Cast(this Ray r, float limit, float increment = 1f)
        {
            IEnumerable<ICollisionObject> cobjs = CollisionObjects.Where(co => co.Position.FastDistance(r.Position) < limit * limit); for (float f = 0; f < limit; f += increment)
            {
                foreach (ICollisionObject co in cobjs)
                {
                    if (co.Position.Distance(r.Position + r.Direction * f) <= co.CollisionRadius)
                        return co;
                }
            }
            return null;
        }

        /// <summary>
        /// Detect collisions at a fixed distance along a ray.
        /// </summary>
        /// <param name="r">the ray to check</param>
        /// <param name="distance">the distance along the ray to check</param>
        /// <returns>the first ICollisionObject in the collision system that was encountered</returns>
        public static ICollisionObject Offset(this Ray r, float distance)
        {
            try
            {
                return CollisionObjects.First(co => co.Position.Distance(r.Position + r.Direction * distance) < co.CollisionRadius);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Find the closest CollisionObject to this point.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject in the system that was within the limit</returns>
        public static ICollisionObject Closest(this Vec3 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistance(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistance(v)).FirstOrDefault();
        }

        /// <summary>
        /// Find the closest CollisionObject to this point, ignoring the height.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject to the X/Z coordinates of the vector</returns>
        public static ICollisionObject ClosestXZ(this Vec3 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).FirstOrDefault();
        }

        /// <summary>
        /// Find the closest CollisionObject to this point, ignoring the height.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject to the coordinates of the vector (along the 3d X/Z plane)</returns>
        public static ICollisionObject ClosestXZ(this Vec2 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).FirstOrDefault();
        }

        //Just realized this is pointless, since finding the single closest isn't too slow, and we can translate that back to the grid and grab data from there.
        //Note that this is VERY SLOW - could be improved, most likely.
        /// <summary>
        /// Find the closest (count) CollisionObjects to this point, ignoring the height (SLOW!)
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <param name="count">the number of objects to return</param>
        /// <returns>an IEnumerable of ICollisionObject containing the closest (count) CollisionObjects to the point</returns>
        public static IEnumerable<ICollisionObject> ClosestXZ(this Vec3 v, float limit, int count)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).Take(count);
        }

        public enum State : byte
        {
            Inside = 0,
            Outside = 1,
            Intersect = 2,
        }
    }
}
