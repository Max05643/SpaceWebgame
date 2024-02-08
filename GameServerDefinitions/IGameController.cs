using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// The interface provided to IGameState in order for it to communicate with players
    /// </summary>
    /// <typeparam name="PlayerUpdate">Type that stores information that are sent to players on every tick</typeparam>
    /// <typeparam name="PlayerInput">Type that stores input of the player</typeparam>
    public interface IGameController<PlayerUpdate, PlayerInput> : IPlayerInputProvider<PlayerInput>
    {
        /// <summary>
        /// Sends an update to the specified player
        /// </summary>
        void SendUpdate(PlayerUpdate playerUpdate, PlayerId playerId);
    }
}
