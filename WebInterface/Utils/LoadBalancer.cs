using GameDesign.GameMaps;
using GameServersManager.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebInterface.Utils
{
    /// <summary>
    /// Handles game instances. Starts new ones and ends old ones if needed. Returns best game for a new player
    /// </summary>
    public class LoadBalancer
    {

        readonly FrontBackCommunication frontBackCommunication;
        readonly ILogger<LoadBalancer> logger;
        readonly IConfiguration configuration;
        readonly Guid defaultServerId;
        
        public LoadBalancer(ILogger<LoadBalancer> logger, FrontBackCommunication frontBackCommunication, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.frontBackCommunication = frontBackCommunication;

            try
            {
                defaultServerId = StartDefaultGame();
            }
            catch (Exception e)
            {
                logger.LogError("Error {e} during default server initialization", e);
            }

        }


        /// <summary>
        /// Returns best serverId for a new player
        /// </summary>
        public Task<Guid> GetServerForNewPlayer()
        {
            // Not implemented
            return Task.FromResult(defaultServerId);
        }


        /// <summary>
        /// Starts one default game in GameServersManager
        /// </summary>
        Guid StartDefaultGame()
        {
            var tickTime = int.Parse(configuration["Game:TargetTickTime"] ?? "15");
            var shouldDebugTickTime = bool.Parse(configuration["Game:DebugTickTime"] ?? "false");


            var shouldLogJoin = bool.Parse(configuration["Game:LogGameJoins"] ?? "false");
            var shouldLogLeave = bool.Parse(configuration["Game:LogGameLefts"] ?? "false");

            var playerKickTimeout = int.Parse(configuration["Game:PlayerKickTimeoutInSeconds"] ?? "60"); ;

            var serverId = frontBackCommunication.StartTheGame
                (
                new GameServersManager.Models.GameServerSettings(
                    new GameDesign.Models.GameSettings()
                    {
                        Gravity = Vector2.Zero,
                        TickTime = tickTime,
                        WorldSize = Vector2.One * 800,
                        ShouldDebugTickTime = shouldDebugTickTime,
                        NumberOfTicksToDebugTickTime = 15,
                        ObjectsSpawnSettings = new GameDesign.Models.ObjectsSettings()
                        {
                            SpawnObjectsRandomly = true,
                            MinDistance = 30,
                            RandomSpawnInterval = TimeSpan.FromMinutes(1),
                            TargetAsteroidCount = 500
                        },
                        LogGameJoin = shouldLogJoin,
                        LogGameLeave = shouldLogLeave,
                        PlayerKickTimeout = TimeSpan.FromSeconds(playerKickTimeout)
                    })).Result!.Value;


            var map = new GameMap();

            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(10, 10) });

            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(20, -10) });

            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(-10, -15) });

            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(30, 10) });

            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.Asteroid, Position = new Vector2(-20, 14) });


            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.BlackHole, Position = new Vector2(50, 50) });


            map.GameObjects.Add(new GameMap.GameObjectMapDescriptor() { Type = GameMap.GameObjectMapDescriptor.ObjectType.SpaceStation, Position = new Vector2(-50, -50) });


            frontBackCommunication.ApplyMap(serverId, map).Wait();


            return serverId;
        }
    }
}
