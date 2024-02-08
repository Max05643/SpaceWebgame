using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represents players input that is used in PlayerControllerComponent
    /// </summary>
    public class PlayerInput
    {
        /// <summary>
        /// Direction of player's object, in rad
        /// </summary>
        public float Angle { get; set; } = 0;

        /// <summary>
        /// Movement power of player's object towards its direction, from 0 to 1
        /// </summary>
        public float MovementPower { get; set; } = 0;

        /// <summary>
        /// Is player holding "Fire" button
        /// </summary>
        public bool IsFiring { get; set; } = false;

        /// <summary>
        /// Request from player to invest point into specified investment type.
        /// Can be null, if there is no investment requests on this frame
        /// </summary>
        public PlayerInvestmentState.InvestmentType? InvestmentRequest { get; set; } = null;

        /// <summary>
        /// Request from player to spend money on restoring health to max level.
        /// Can be false, if there is no request on this frame
        /// </summary>
        public bool RepairRequest { get; set; } = false;
    }
}
