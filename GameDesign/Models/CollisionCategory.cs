using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Collision categories. Wrapper of physical engine's collision categories
    /// </summary>
    [Flags]
    public enum CollisionCategory
    {
        None = 0x0,
        All = int.MaxValue,
        /// <summary>
        /// General category
        /// </summary>
        RegularObject = 1,
        /// <summary>
        /// Player's projectile
        /// </summary>
        Projectile = 2,
        /// <summary>
        /// Player's rocket
        /// </summary>
        Rocket = 4,
        /// <summary>
        /// Player
        /// </summary>
        Player = 8,
        /// <summary>
        /// Static wall that restricts the world
        /// </summary>
        StaticWorldWall = 16,
        /// <summary>
        /// Safe zones where players can attack or be attacked. Should be a sensor
        /// </summary>
        SafeZone = 32
    }

}
