using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation
{
    /// <summary>
    /// Game server for any web-based game
    /// </summary>
    /// <typeparam name="PlayerInput">Type that stores input of the player</typeparam>
    /// <typeparam name="PlayerUpdate">Type that stores information that are sent to players on every tick</typeparam>
    public class GameServer<GameState, PlayerInput, PlayerUpdate> : IGameServer<GameState, PlayerInput, PlayerUpdate>, IGameController<PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        private readonly IPlayersCommunication<PlayerUpdate> playersCommunication;
        private readonly GameState gameState;
        private readonly GameServerSettings serverSettings;
        public GameServer(IGameStateFactory<GameState, PlayerInput, PlayerUpdate> gameStateFactory, IPlayersCommunication<PlayerUpdate> playersCommunication, GameServerSettings serverSettings)
        {
            this.playersCommunication = playersCommunication;

            gameState = gameStateFactory.StartNewGame();
            this.serverSettings = serverSettings;
        }

        bool IGameServer<GameState, PlayerInput, PlayerUpdate>.IsRunning => throw new NotImplementedException();

        IEnumerable<PlayerId> IGameServer<GameState, PlayerInput, PlayerUpdate>.CurrentPlayers => throw new NotImplementedException();

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.AcceptPlayerInput(PlayerInput playerInput, PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        bool IGameServer<GameState, PlayerInput, PlayerUpdate>.AddPlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        bool IGameServer<GameState, PlayerInput, PlayerUpdate>.IsPlayerInGame(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.KickPlayer(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.LeaveGame(PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        void IGameController<PlayerUpdate>.SendUpdate(PlayerUpdate playerUpdate, PlayerId playerId)
        {
            throw new NotImplementedException();
        }

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.StopGame()
        {
            throw new NotImplementedException();
        }
    }
}
