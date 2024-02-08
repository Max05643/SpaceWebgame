using Boxed.Mapping;
using GameDesign.Models;
using System.ComponentModel;
using WebInterface.ClientModels;

namespace WebInterface.Mappers
{
    public class PlayerInputMapper : IMapper<ClientInput, PlayerInput>
    {
        void IMapper<ClientInput, PlayerInput>.Map(ClientInput source, PlayerInput destination)
        {
            destination.Angle = source.Angle % (MathF.PI * 2);
            destination.MovementPower = Math.Clamp(source.MovementPower, 0, 1);
            destination.IsFiring = source.IsFire;
            destination.InvestmentRequest = source.InvestmentRequest;
            destination.RepairRequest = source.RepairRequest;
        }
    }
}
