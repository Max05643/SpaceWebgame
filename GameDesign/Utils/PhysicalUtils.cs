using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    /// <summary>
    /// Helper methods for physical system 
    /// </summary>
    public static class PhysicalUtils
    {
        public static Genbox.VelcroPhysics.Collision.Filtering.Category ToPhysicalEngineCategory(this Models.CollisionCategory collisionCategory)
        {
            return (Genbox.VelcroPhysics.Collision.Filtering.Category)collisionCategory;
        }
    }
}
