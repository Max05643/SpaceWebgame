using GameServerDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    /// <summary>
    /// Checks whether any player is timed out and kicks them if necessary
    /// </summary>
    internal class TimeoutCheckEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {

        TaskCompletionSource taskCompletionSource;

        public TimeoutCheckEvent(TaskCompletionSource taskCompletionSource)
        {
            this.taskCompletionSource = taskCompletionSource;
        }

        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            foreach (var player in gameState.CurrentPlayers)
            {
                DateTime? lastInputTime = playerInputStorage.GetLastInputTime(player);

                if (lastInputTime == null || (DateTime.Now - lastInputTime) > serverSettings.PlayerKickTimeout)
                {
                    gameState.RemovePlayer(player);
                    playersCommunication.NotifyPlayerThatHeIsKicked(player);
                    playerInputStorage.DisposePlayer(player);
                    playersCommunication.DisposePlayerConnection(player);
                }
            }


            taskCompletionSource.SetResult();
        }
    }
}
