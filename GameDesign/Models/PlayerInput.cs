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


        /// <summary>
        /// Updates this instance accordingly to new information
        /// </summary>
        public void Update(PlayerInput newInput)
        {
            MovementPower = newInput.MovementPower;
            Angle = newInput.Angle;
            IsFiring = newInput.IsFiring;
            if (newInput.InvestmentRequest != null)
                InvestmentRequest = newInput.InvestmentRequest;
            RepairRequest |= newInput.RepairRequest;
        }

        /// <summary>
        /// Removes the information about instant commands from this instance and returns the copy of it with that information
        /// </summary>
        public PlayerInput RemoveInstantInformation()
        {

            var res = new PlayerInput()
            {
                MovementPower = MovementPower,
                Angle = Angle,
                IsFiring = IsFiring,
                InvestmentRequest = InvestmentRequest,
                RepairRequest = RepairRequest
            };

            RepairRequest = false;
            InvestmentRequest = null;

            return res;
        }
    }
}
