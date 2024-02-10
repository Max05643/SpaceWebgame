using GameDesign.GameMaps;
using GameDesign.GameState;
using GameDesign.Models;
using GameDesign.Utils;
using GameServerDefinitions;
using Microsoft.Xna.Framework;

namespace WebInterface.Utils
{
    public class GameStateFactory : IGameStateFactory<GameStateManager, PlayerInput, PlayerUpdate>
    {

        private readonly IGraphicLibraryProvider graphicLibrary;
        private readonly IConfiguration configuration;

        public GameStateFactory(IGraphicLibraryProvider graphicLibrary, IConfiguration configuration)
        {
            this.graphicLibrary = graphicLibrary;
            this.configuration = configuration;
        }

        GameStateManager IGameStateFactory<GameStateManager, PlayerInput, PlayerUpdate>.StartNewGame()
        {
            var gameSettings = new GameDesign.Models.GameSettings()
            {
                Gravity = Vector2.Zero,
                WorldSize = Vector2.One * 800,
                ObjectsSpawnSettings = new GameDesign.Models.ObjectsSettings()
                {
                    SpawnObjectsRandomly = true,
                    MinDistance = 60,
                    RandomSpawnInterval = TimeSpan.FromMinutes(1),
                    TargetAsteroidCount = 200
                },
            };

            var map = new GameMap();
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(10, 10) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(20, -10) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(-10, -15) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(30, 10) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(-20, 14) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.BlackHole, Position = new Vector2(50, 50) });
            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.SpaceStation, Position = new Vector2(-50, -50) });

            var result = new GameStateManager(gameSettings, graphicLibrary);

            map.SpawnObjects(result);

            return result;
        }
    }
}
