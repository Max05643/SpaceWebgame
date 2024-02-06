using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// Provides a way to process new input while updating old one and a way to pop data stored in old input
    /// </summary>
    public interface IPlayerInputProcessor<PlayerInput>
    {
        /// <summary>
        /// Returns a default value of player's input used when it is required but yet no is available
        /// </summary>
        PlayerInput GetDefaultInput();

        /// <summary>
        /// Takes stored player input. Returns input that can be used in tick calculations. Also returns new input that should be stored again.
        /// Used for situations when some queued commands from player is stored in input
        /// </summary>
        PlayerInput PopInput(PlayerInput storedInput, out PlayerInput newStoredInput);

        /// <summary>
        /// Takes stored player input and new supplied input. Returns input that should be stored.
        /// Used for situations when some queued commands from player is stored in input
        /// </summary>
        PlayerInput StoreNewInput(PlayerInput storedInput, PlayerInput newInput);
    }
}
