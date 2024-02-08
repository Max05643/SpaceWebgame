using GameDesign.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Stores an information about player's investments in his ship and provides a method for setting up characteristics in PlayerController
    /// </summary>
    public class PlayerInvestmentState
    {
        public enum InvestmentType
        {
            /// <summary>
            /// Player's maximum amount of health
            /// </summary>
            MaxHealth = 0,
            /// <summary>
            /// Damage that is inflicted on other object that collides with player
            /// </summary>
            PhysicalDamage = 1,
            /// <summary>
            /// Damage of one player's projectile
            /// </summary>
            ProjectileDamage = 2,
            /// <summary>
            /// Player's max speed
            /// </summary>
            Power = 3,
            /// <summary>
            /// Player's acceleration
            /// </summary>
            Acceleration = 4,
            /// <summary>
            /// Player's angular speed
            /// </summary>
            AngularSpeed = 5,
            /// <summary>
            /// Player's reload speed
            /// </summary>
            Reload = 6
        }

        private Dictionary<InvestmentType, int> investedPoints = new Dictionary<InvestmentType, int>();


        /// <summary>
        /// Information about already invested points per caregory
        /// </summary>
        public IReadOnlyDictionary<InvestmentType, int> AlreadyInvested => investedPoints;

        /// <summary>
        /// Can points be invested into specified type?
        /// </summary>
        public bool CanInvest(InvestmentType investmentType)
        {
            return investedPoints[investmentType] < 100;
        }
        public PlayerInvestmentState()
        {
            foreach (var investmentType in Enum.GetValues<InvestmentType>())
            {
                investedPoints[investmentType] = 0;
            }
        }

        /// <summary>
        /// Adds specified amount of investment.
        /// Investments in one category will not exceed 100 even if points are added
        /// </summary>
        public void InvestPoints(InvestmentType investmentType, int amount)
        {
            investedPoints[investmentType] += amount;
            investedPoints[investmentType] = Math.Min(investedPoints[investmentType], 100);
        }

        /// <summary>
        /// Setups all the variables in player controller component accordingly to invested points
        /// </summary>
        public void SetupPlayerControllerComponent(PlayerControllerComponent playerControllerComponent)
        {
            playerControllerComponent.Power = 50 + investedPoints[InvestmentType.Power];
            playerControllerComponent.AngularSpeed = 5f + investedPoints[InvestmentType.AngularSpeed] / 100f * 5f;
            playerControllerComponent.TimePerShot = 1f - investedPoints[InvestmentType.Reload] / 200f;
            playerControllerComponent.Acceleration = 5f + investedPoints[InvestmentType.Acceleration] / 100f * 5f;
            playerControllerComponent.MaxHealth = 100 + investedPoints[InvestmentType.MaxHealth] * 2;
            playerControllerComponent.Damage = 20 + investedPoints[InvestmentType.PhysicalDamage];
            playerControllerComponent.ShotDamage = 20 + investedPoints[InvestmentType.ProjectileDamage];
        }


    }
}
