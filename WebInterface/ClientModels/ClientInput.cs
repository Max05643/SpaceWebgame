using GameDesign.Models;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebInterface.ClientModels
{
    [MessagePackObject]
    public class ClientInput
    {
        [Key("_an")]
        public float Angle { get; set; } = 0;

        [Key("_mp")]
        public float MovementPower { get; set; } = 0;

        [Key("_if")]
        public bool IsFire { get; set; } = false;

        [Key("_ir")]
        public PlayerInvestmentState.InvestmentType? InvestmentRequest { get; set; } = null;

        [Key("_rr")]
        public bool RepairRequest { get; set; } = false;

        [Key("_rv")]
        public bool ReviveRequest { get; set; } = false;


        public override string ToString()
        {
            return $"Angle: {Angle}, Movement power: {MovementPower}, IsFire:{IsFire}";
        }
    }
}
