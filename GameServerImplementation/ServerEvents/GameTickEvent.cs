using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class GameTickEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly Stopwatch stopwatch;
        readonly IGameController<PlayerUpdate, PlayerInput> gameController;

        public GameTickEvent(Stopwatch stopwatch, IGameController<PlayerUpdate, PlayerInput> gameController)
        {
            this.stopwatch = stopwatch;
            this.gameController = gameController;
        }

        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            stopwatch.Stop();

            var deltaTime = stopwatch.Elapsed;

            stopwatch.Restart();

            gameState.Tick(deltaTime, gameController);
        }
    }
}
