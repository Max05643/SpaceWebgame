using Genbox.VelcroPhysics.Collision.Narrowphase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components.Interfaces
{
    /// <summary>
    /// Represents an object that can iflict and receive damage during the collision with other IDestructible
    /// </summary>
    public interface IDestructible
    {
        /// <summary>
        /// Can this object be destructed? 
        /// </summary>
        bool IsInvincible { get; }

        /// <summary>
        /// Will this object inflict infinite damage on other IDestructible?
        /// </summary>
        bool IsInfiniteDamage { get; }

        /// <summary>
        /// This IDestructible will receive specified damage
        /// </summary>
        /// <returns>Was this object destroyed during this collision?</returns>
        bool ReceiveDamage(int damageAmount);

        /// <summary>
        /// Returns the damage that this object inflicts dureint the collision
        /// </summary>
        int GetInflictedDamage();

        /// <summary>
        /// Is called when this IDestructible touches IDestructible with IsInfiniteDamage = true.
        /// </summary>
        /// <returns>Was this object destroyed during this collision?</returns>
        bool DieImmediatly();

    }
}