using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// Provides a way to create new instances of the game server
    /// </summary>
    public interface IGameServerFactory<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        /// <summary>
        /// Creates new game server
        /// </summary>
        public IGameServer<GameState, PlayerInput, PlayerUpdate> CreateServer(IPlayersCommunication<PlayerUpdate> playersCommunication);
    }
}
