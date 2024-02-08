using GameDesign.Models.Components.Interfaces;
using GameDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    /// <summary>
    /// Handles situations when two IDestructible are interacting
    /// </summary>
    public static class DestructionUtils
    {
        /// <summary>
        /// Calculates the results of the collision.
        /// Should be called in AfterPhysicalCalculation
        /// </summary>
        public static void CalculateCollisionResults(GameObject thisObj, IDestructible thisDestructible, GameObject other)
        {
            if (other.IsDestroyed)
            {
                return;
            }
            else if (other.HasComponent<IDestructible>() && other.Id.CompareTo(thisObj.Id) < 0)
            // The object with bigger guid handles collision 
            {
                var otherDestructible = other.GetComponent<IDestructible>();

                if (otherDestructible.IsInfiniteDamage)
                {
                    thisDestructible.DieImmediatly();
                }
                else
                {
                    thisDestructible.ReceiveDamage(otherDestructible.GetInflictedDamage());
                }
                if (thisDestructible.IsInfiniteDamage)
                {
                    otherDestructible.DieImmediatly();
                }
                else
                {
                    otherDestructible.ReceiveDamage(thisDestructible.GetInflictedDamage());
                }

            }
        }
    }
}
