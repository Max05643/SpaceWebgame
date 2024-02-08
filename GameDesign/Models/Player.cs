using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{

    /// <summary>
    /// Represents all gamedesign-related information about game player 
    /// </summary>
    public class Player
    {
        public enum PlayerState
        {
            Alive = 0,
            Dead = 1,
            NotEntered = 2
        }

        /// <summary>
        /// Player's state
        /// </summary>
        public PlayerState State { get; set; } = PlayerState.NotEntered;

        /// <summary>
        /// The id of GameObject that represents this player in the game world
        /// </summary>
        public int? PlayersGameObjectId { get; set; } = null;

        /// <summary>
        /// Number of game points that is currently held by this player
        /// </summary>
        public int Points { get; set; } = 0;
    }
}
