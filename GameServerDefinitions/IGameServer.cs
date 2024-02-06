using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// The interface provided by game server to external components. All methods' calls are thread-safe
    /// </summary>
    public interface IGameServer<GameState, PlayerInput, PlayerUpdate> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        /// <summary>
        /// Adds input by the specified player. Does nothing if operation is impossible. Should be implemented with IPlayerInputProcessor
        /// </summary>
        void AcceptPlayerInput(PlayerInput playerInput, PlayerId playerId);

        /// <summary>
        /// Removes player from the game and notifies player that he has been kicked if he is in the game. Does nothing if operation is impossible (if player is already not in the game)
        /// </summary>
        void KickPlayer(PlayerId playerId);

        /// <summary>
        /// Removes player from the game. Does nothing if operation is impossible (if player is already not in the game)
        /// </summary>
        void LeaveGame(PlayerId playerId);

        /// <summary>
        /// Adds player to the game. Does nothing if operation is impossible. Returns true if player is in game (even if he was before this call) and false otherwise
        /// </summary>
        bool AddPlayer(PlayerId playerId);

        /// <summary>
        /// Stops the game without the option of restarting it. Does nothing if the game is already stopped 
        /// </summary>
        void StopGame();

        /// <summary>
        /// Is this game running? Returns false if the game is already stopped
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Returns ids of current players
        /// </summary>
        IEnumerable<PlayerId> CurrentPlayers { get; }

        /// <summary>
        /// Checks if specified player is in game
        /// </summary>
        bool IsPlayerInGame(PlayerId playerId);
    }
}
