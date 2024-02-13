using GameServerDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation
{
    public class GameServerFactory<GameState, PlayerInput, PlayerUpdate> : IGameServerFactory<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        private readonly GameServerSettings serverSettings;
        private readonly ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger;
        private readonly IGameStateFactory<GameState, PlayerInput, PlayerUpdate> gameStateFactory;
        private readonly IPlayerInputProcessor<PlayerInput> playerInputProcessor;
        private readonly IPlayerInputStorageFactory<PlayerInput> playerInputStorageFactory;

        public GameServerFactory(IPlayerInputProcessor<PlayerInput> playerInputProcessor, GameServerSettings serverSettings, IPlayerInputStorageFactory<PlayerInput> playerInputStorageFactory, ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger, IGameStateFactory<GameState, PlayerInput, PlayerUpdate> gameStateFactory)
        {
            this.serverSettings = serverSettings;
            this.logger = logger;
            this.gameStateFactory = gameStateFactory;
            this.playerInputStorageFactory = playerInputStorageFactory;
            this.playerInputProcessor = playerInputProcessor;
        }

        IGameServer<GameState, PlayerInput, PlayerUpdate> IGameServerFactory<GameState, PlayerInput, PlayerUpdate>.CreateServer(IPlayersCommunication<PlayerUpdate> playersCommunication)
        {
            return new GameServer<GameState, PlayerInput, PlayerUpdate>(gameStateFactory, playersCommunication, playerInputProcessor, serverSettings, playerInputStorageFactory, logger);
        }
    }
}
