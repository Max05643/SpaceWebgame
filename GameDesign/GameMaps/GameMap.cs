using GameDesign.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.GameMaps
{
    
    /// <summary>
    /// Represents a collection of game objects.
    /// Can be used to fill the game world with objects on startup
    /// </summary>
    public class GameMap
    {
        /// <summary>
        /// A simplified version of information about game object
        /// </summary>
        public class GameObjectMapDescriptor
        {

            public enum ObjectType
            {
                Asteroid = 0,
                BlackHole = 1,
                SpaceStation = 2
            }

            /// <summary>
            /// Game object's position in world space
            /// </summary>
            public Vector2 Position { get; set; } = Vector2.Zero;

            /// <summary>
            /// Game object's type
            /// </summary>
            public ObjectType Type { get; set; } = ObjectType.Asteroid;
        }
    
        public List<GameObjectMapDescriptor> GameObjects { get; set; } = new List<GameObjectMapDescriptor>();

        /// <summary>
        /// Adds game objects from this map to the game scene
        /// </summary>
        public void SpawnObjects(GameStateManager gameStateManager)
        {
            foreach (var gameObject in GameObjects)
            {
                switch (gameObject.Type)
                {
                    case GameObjectMapDescriptor.ObjectType.Asteroid:
                        gameStateManager.gameObjectFactory.CreateAsteroidGameObject(gameObject.Position);
                        break;
                    case GameObjectMapDescriptor.ObjectType.BlackHole:
                        gameStateManager.gameObjectFactory.CreateBlackHole(gameObject.Position);
                        break;
                    case GameObjectMapDescriptor.ObjectType.SpaceStation:
                        gameStateManager.gameObjectFactory.CreateSpaceStation(gameObject.Position);
                        break;
                }
            }
        }
    }
}
