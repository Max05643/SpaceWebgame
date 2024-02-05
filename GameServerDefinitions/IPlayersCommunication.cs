using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// Provides a way to send updates to players
    /// </summary>
    public interface IPlayersCommunication<PlayerUpdate>
    {
        /// <summary>
        /// Sends an update to the specified player or does nothing if operation is impossible
        /// </summary>
        void SendUpdate(PlayerUpdate update, PlayerId playerId);

        /// <summary>
        /// If possible sends a notification to player that he is kicked from the game 
        /// </summary>
        void NotifyPlayerThatHeIsKicked(PlayerId playerId);

        /// <summary>
        /// Does all the cleaning that is needed after the specified player is no more in game
        /// </summary>
        void DisposePlayerConnection(PlayerId playerId);
    }
}
