using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerDefinitions
{
    /// <summary>
    /// Provides access to players' input in thread-safe way
    /// </summary>
    public interface IPlayerInputProvider<PlayerInput>
    {
        /// <summary>
        /// Returns last player's input or default value if none was ever received. Should be implemented with IPlayerInputProcessor
        /// </summary>
        PlayerInput PopPlayerInput(PlayerId playerId);
    }
}
