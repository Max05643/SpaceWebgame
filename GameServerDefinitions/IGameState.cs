using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// An object that stores a state of the game and can change it after external calls
    /// </summary>
    /// <typeparam name="PlayerInput">Type that stores input of the player</typeparam>
    /// <typeparam name="PlayerUpdate">Type that stores information that are sent to players on every tick</typeparam>
    public interface IGameState<PlayerInput, PlayerUpdate>
    {
        /// <summary>
        /// Adds player to the game. Does nothing if operation is impossible. Returns true if player is in game (even if he was before this call) and false otherwise
        /// </summary>
        public bool AddPlayer(PlayerId playerId);

        /// <summary>
        /// Removes player from the game. Does nothing if operation is impossible (if player is already not in the game)
        /// </summary>
        public void RemovePlayer(PlayerId playerId);

        /// <summary>
        /// Will process the update of the game state and send individual game state updates to players via gameController
        /// </summary>
        /// <param name="deltaTime">Time passed since the last call to this method</param>
        public void Tick(TimeSpan deltaTime, IGameController<PlayerUpdate> gameController);

        /// <summary>
        /// Returns ids of current players
        /// </summary>
        public IEnumerable<PlayerId> CurrentPlayers { get; }


        /// <summary>
        /// Checks if specified player is in game
        /// </summary>
        bool IsPlayerInGame(PlayerId playerId); 
    }
}
