using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// An object that provides a way to create new game states
    /// </summary>
    public interface IGameStateFactory<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        /// <summary>
        /// Starts new game
        /// </summary>
        GameState StartNewGame();
    }
}
