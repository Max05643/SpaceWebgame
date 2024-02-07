using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class GameTickEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly int deltaTimeMs;
        readonly IGameController<PlayerUpdate, PlayerInput> gameController;

        public GameTickEvent(int deltaTimeMs, IGameController<PlayerUpdate, PlayerInput> gameController)
        {
            this.deltaTimeMs = deltaTimeMs;
            this.gameController = gameController;
        }

        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            gameState.Tick(TimeSpan.FromMilliseconds(deltaTimeMs), gameController);
        }
    }
}
