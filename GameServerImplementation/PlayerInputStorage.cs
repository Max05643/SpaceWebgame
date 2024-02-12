using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerImplementation
{

    /// <summary>
    /// Provides a thread-safe way to store players' input
    /// </summary>
    public abstract class PlayerInputStorage<PlayerInput>
    {
        protected readonly IPlayerInputProcessor<PlayerInput> playerInputProcessor;
        public PlayerInputStorage(IPlayerInputProcessor<PlayerInput> playerInputProcessor)
        {
            this.playerInputProcessor = playerInputProcessor;
        }

        /// <summary>
        /// Removes specified player's stored input if any
        /// </summary>
        public abstract void DisposePlayer(PlayerId playerId);

        /// <summary>
        /// Stores new input and applies rules from IPlayerInputProcessor to it
        /// </summary>
        public abstract void StoreNewInput(PlayerInput newInput, PlayerId playerId);

        /// <summary>
        /// Returns stored player input and applies rules from IPlayerInputProcessor to it
        /// </summary>
        public abstract PlayerInput PopPlayerInput(PlayerId playerId);

        /// <summary>
        /// Returns the time when last input from specified player was stored or null if none is stored
        /// </summary>
        public abstract DateTime? GetLastInputTime(PlayerId playerId);


    }
}
