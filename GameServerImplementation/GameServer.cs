using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GameServerImplementation
{
    /// <summary>
    /// Game server for any web-based game
    /// </summary>
    /// <typeparam name="PlayerInput">Type that stores input of the player</typeparam>
    /// <typeparam name="PlayerUpdate">Type that stores information that are sent to players on every tick</typeparam>
    public class GameServer<GameState, PlayerInput, PlayerUpdate> : IGameServer<GameState, PlayerInput, PlayerUpdate>, IGameController<PlayerUpdate, PlayerInput> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        private readonly IPlayersCommunication<PlayerUpdate> playersCommunication;
        private readonly GameState gameState;
        private readonly GameServerSettings serverSettings;
        private readonly PlayerInputStorage<PlayerInput> playerInputStorage;
        private readonly ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger;

        private readonly CancellationTokenSource cancellationTokenSource;

        public GameServer(IGameStateFactory<GameState, PlayerInput, PlayerUpdate> gameStateFactory, IPlayersCommunication<PlayerUpdate> playersCommunication, IPlayerInputProcessor<PlayerInput> playerInputProcessor, GameServerSettings serverSettings, IPlayerInputStorageFactory<PlayerInput> playerInputStorageFactory, ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger)
        {
            cancellationTokenSource = new CancellationTokenSource();

            this.playersCommunication = playersCommunication;
            this.serverSettings = serverSettings;
            playerInputStorage = playerInputStorageFactory.CreateNewStorage(playerInputProcessor);
            this.logger = logger;

            gameState = gameStateFactory.StartNewGame();
        }

        bool IGameServer<GameState, PlayerInput, PlayerUpdate>.IsRunning => !cancellationTokenSource.IsCancellationRequested;

        async Task<IEnumerable<PlayerId>> IGameServer<GameState, PlayerInput, PlayerUpdate>.GetCurrentPlayers()
        {
            throw new NotImplementedException();
        }
        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.AcceptPlayerInput(PlayerInput playerInput, PlayerId playerId)
        {
            if (await IsPlayerInGame(playerId))
            {
                playerInputStorage.StoreNewInput(playerInput, playerId);
            }
        }

        async Task<bool> IGameServer<GameState, PlayerInput, PlayerUpdate>.AddPlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsPlayerInGame(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.KickPlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.LeaveGame(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        PlayerInput IGameController<PlayerUpdate, PlayerInput>.PopPlayerInput(PlayerId playerId)
        {
            return playerInputStorage.PopPlayerInput(playerId);
        }

        void IGameController<PlayerUpdate, PlayerInput>.SendUpdate(PlayerUpdate playerUpdate, PlayerId playerId)
        {
            playersCommunication.SendUpdate(playerUpdate, playerId);
        }

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.StopGame()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
