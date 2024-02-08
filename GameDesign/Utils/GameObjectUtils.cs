using GameDesign.GameState;
using GameDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    /// <summary>
    /// Contains helper methods for Game Objects
    /// </summary>
    public static class GameObjectUtils
    {
        /// <summary>
        /// Removes this object using GameStateManager
        /// </summary>
        /// <param name="gameObject"></param>
        public static void RemoveThisObject(this GameObject gameObject)
        {
            gameObject.GameStateManager.sceneManager.RemoveGameObject(gameObject.Id);
        }


        /// <summary>
        /// Returns random position within 90% of game world borders
        /// </summary>
        public static Vector2 GetRandomPosition(this GameSettings gameSettings)
        {
            Vector2 randomPosition = new Vector2(gameSettings.WorldSize.X * ((float)Random.Shared.NextDouble() - 0.5f) * 0.9f, gameSettings.WorldSize.Y * ((float)Random.Shared.NextDouble() - 0.5f) * 0.9f);
            return randomPosition;
        }
    }
}
