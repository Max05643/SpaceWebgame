using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Stores settings related to random spawning of objects in the game world
    /// </summary>
    public class ObjectsSettings
    {
        /// <summary>
        /// Should game server add objects to the scene randomly?
        /// </summary>
        public bool SpawnObjectsRandomly { get; set; } = false;

        /// <summary>
        /// How often should game server try to spawn new batches of objects?
        /// </summary>
        public TimeSpan RandomSpawnInterval { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Minimal distance between existing objects and the objects to be spawned randomly
        /// </summary>
        public float MinDistance { get; set; } = 10;
        
        /// <summary>
        /// Random spawner will not spawn more asteroids than this number 
        /// </summary>
        public int TargetAsteroidCount { get; set; } = 100;

    }
}
