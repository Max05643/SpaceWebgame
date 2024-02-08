using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Configuration of the game related to gamedesign
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Gravity vector
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// Size of the game's world for physical engine
        /// </summary>
        public Vector2 WorldSize { get; set; }

        /// <summary>
        /// Settings related to randomly spawned objects
        /// </summary>
        public ObjectsSettings ObjectsSpawnSettings { get; set; } = new ObjectsSettings();
    }
}
