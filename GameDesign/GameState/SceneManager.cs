using GameDesign.Models;
using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.GameState
{
    /// <summary>
    /// Manages GameObjects
    /// </summary>
    public class SceneManager
    {
        readonly Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();

        readonly GameStateManager gameStateManager;


        /// <summary>
        /// Next free id for game objects
        /// </summary>
        int nextFreeId = 0;

        /// <summary>
        /// Returns unique id for the game object
        /// </summary>
        public virtual int GetUniqueId()
        {
            return nextFreeId++;
        }


        public SceneManager(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }



        /// <summary>
        /// All GameObjects currently present in scene
        /// </summary>
        public virtual IReadOnlyDictionary<int, GameObject> GameObjects { get {  return gameObjects; } }

        /// <summary>
        /// Adds new GameObject to the SceneManager's registry. Do not handle physical engine
        /// </summary>
        public virtual void RegisterGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject.Id, gameObject);
        }

        /// <summary>
        /// Removes GameObject with specified id and calls Destroy() on it.
        /// Nothing happens if there is no GameObject with specified id
        /// </summary>
        public virtual void RemoveGameObject(int gameObjectId)
        {
            if (gameObjects.TryGetValue(gameObjectId, out GameObject? gameObject))
            {
                gameObject.Destroy();
                gameObjects.Remove(gameObjectId);
            }
        }

        /// <summary>
        /// Called every frame before physical calculation
        /// </summary>
        /// <param name="deltaTime">Time between frames in seconds</param>
        public virtual void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var obj in gameObjects.Values.ToList())
            {
                obj.BeforePhysicalCalculation(deltaTime, playerInputProvider);
            }
        }
        /// <summary>
        /// Called every frame after physical calculation
        /// </summary>
        /// <param name="deltaTime">Time between frames in seconds</param>
        public virtual void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var obj in gameObjects.Values.ToList())
            {
                obj.AfterPhysicalCalculation(deltaTime, playerInputProvider);
            }
        }


    }
}
