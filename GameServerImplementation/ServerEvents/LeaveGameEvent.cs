using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class LeaveGameEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly PlayerId playerId;
        readonly TaskCompletionSource gameStateCompletionSource;
        public LeaveGameEvent(PlayerId playerId, TaskCompletionSource gameStateCompletionSource)
        {
            this.playerId = playerId;
            this.gameStateCompletionSource = gameStateCompletionSource;
        }


        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            gameState.RemovePlayer(playerId);
            playerInputStorage.DisposePlayer(playerId);
            playersCommunication.DisposePlayerConnection(playerId);
            gameStateCompletionSource.SetResult();
        }
    }
}
