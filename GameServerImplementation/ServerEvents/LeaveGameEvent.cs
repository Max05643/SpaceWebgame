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
        readonly PlayerInputStorage<PlayerUpdate> playerInputStorage;
        public LeaveGameEvent(PlayerId playerId, PlayerInputStorage<PlayerUpdate> playerInputStorage)
        {
            this.playerId = playerId;
            this.playerInputStorage = playerInputStorage;
        }


        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            gameState.RemovePlayer(playerId);
            playerInputStorage.DisposePlayer(playerId);
            playersCommunication.DisposePlayerConnection(playerId);
        }
    }
}
