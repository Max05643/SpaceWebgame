using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class CheckIfInGameEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly PlayerId playerId;
        readonly TaskCompletionSource<bool> isInGame;

        public CheckIfInGameEvent(PlayerId playerId, TaskCompletionSource<bool> isInGame)
        {
            this.playerId = playerId;
            this.isInGame = isInGame;
        }

        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            isInGame.SetResult(gameState.IsPlayerInGame(playerId));
        }
    }
}
