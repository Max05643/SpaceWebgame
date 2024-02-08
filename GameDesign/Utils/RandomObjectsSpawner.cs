using GameDesign.GameState;
using GameDesign.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    /// <summary>
    /// Contains helper methods for spawning random destructible objects.
    /// Will not spawn static objects (black holes, space stations, etc)
    /// </summary>
    public static class RandomObjectsSpawner
    {

        /// <summary>
        /// Will try to spawn new game objects accordinly to game settings
        /// </summary>
        /// <returns>Number of objects spawned</returns>
        public static int ProcessObjectsSpawn(this GameStateManager gameStateManager)
        {
            int spawnCount = 0;
            int needToSpawnAsteroidsNow = gameStateManager.settings.ObjectsSpawnSettings.TargetAsteroidCount - gameStateManager.sceneManager.GameObjects.Where(obj => obj.Value.HasComponent<AsteroidComponent>()).Count();

            for (int i = 0; i < needToSpawnAsteroidsNow; i++)
            {
                spawnCount += TryAddRandomAsteroid(gameStateManager) ? 1 : 0;
            }

            return spawnCount;
        }

        /// <summary>
        /// Checks if there are any objects within ObjectsSpawnSettings.MinDistance from specified position
        /// </summary>
        static bool CheckObjectsIntersection(GameStateManager gameStateManager, Vector2 position)
        {
            var maxDistSquared = gameStateManager.settings.ObjectsSpawnSettings.MinDistance * gameStateManager.settings.ObjectsSpawnSettings.MinDistance;

            return gameStateManager.sceneManager.GameObjects.Any(obj =>
            {
                var res = obj.Value.Position - position;
                return res.LengthSquared() < maxDistSquared;
            }
            );
        }


        /// <summary>
        /// Tries to add a random asteroid somewhere on the map.
        /// If it fails, because that place is already taken, returns false, otherwise true
        /// </summary>
        static bool TryAddRandomAsteroid(GameStateManager gameStateManager)
        {
            var position = gameStateManager.settings.GetRandomPosition();
            if (!CheckObjectsIntersection(gameStateManager, position))
            {
                gameStateManager.gameObjectFactory.CreateAsteroidGameObject(position);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
