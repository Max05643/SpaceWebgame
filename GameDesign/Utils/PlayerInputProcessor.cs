using GameDesign.Models;
using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    public class PlayerInputProcessor : IPlayerInputProcessor<PlayerInput>
    {
        PlayerInput IPlayerInputProcessor<PlayerInput>.GetDefaultInput()
        {
            return new PlayerInput() { };
        }

        PlayerInput IPlayerInputProcessor<PlayerInput>.PopInput(PlayerInput storedInput, out PlayerInput newStoredInput)
        {
            newStoredInput = new PlayerInput()
            {
                MovementPower = storedInput.MovementPower,
                Angle = storedInput.Angle,
                IsFiring = storedInput.IsFiring,
                InvestmentRequest = null,
                RepairRequest = false,
                ReviveRequest = false
            };

            return storedInput;
        }

        PlayerInput IPlayerInputProcessor<PlayerInput>.StoreNewInput(PlayerInput storedInput, PlayerInput newInput)
        {
            var inputToStore = new PlayerInput
            {
                MovementPower = newInput.MovementPower,
                Angle = newInput.Angle,
                IsFiring = newInput.IsFiring,
                RepairRequest = storedInput.RepairRequest || newInput.RepairRequest,
                InvestmentRequest = newInput.InvestmentRequest == null ? storedInput.InvestmentRequest : newInput.InvestmentRequest,
                ReviveRequest = storedInput.ReviveRequest || newInput.ReviveRequest
            };

            return inputToStore;
        }
    }
}
