using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation.ServerEvents
{
    internal class GetPlayersEvent<GameState, PlayerInput, PlayerUpdate> : ServerEvent<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        readonly TaskCompletionSource<IEnumerable<PlayerId>> players;

        public GetPlayersEvent(TaskCompletionSource<IEnumerable<PlayerId>> players)
        {
            this.players = players;
        }

        public override void Perform(GameState gameState, GameServerSettings serverSettings, IPlayersCommunication<PlayerUpdate> playersCommunication, PlayerInputStorage<PlayerInput> playerInputStorage)
        {
            players.SetResult(gameState.CurrentPlayers.ToList());
        }
    }
}
