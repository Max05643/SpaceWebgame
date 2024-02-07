using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class AddPlayerEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly PlayerId playerId;
        readonly TaskCompletionSource taskCompletionSource;

        public AddPlayerEvent(PlayerId playerId, TaskCompletionSource taskCompletionSource)
        {
            this.playerId = playerId;
            this.taskCompletionSource = taskCompletionSource;
        }


        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            gameState.AddPlayer(playerId);
            taskCompletionSource.SetResult();
        }
    }


}


