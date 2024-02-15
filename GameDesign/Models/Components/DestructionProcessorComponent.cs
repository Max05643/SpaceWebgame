using GameDesign.Models.Components.Interfaces;
using GameDesign.Utils;
using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Used with IDestructibles on the same GameObject in order to correctly process collisions
    /// </summary>
    public class DestructionProcessorComponent : Component
    {
        readonly IDestructible destructibleComponent;
        readonly PhysicalComponent physicalComponent;

        public DestructionProcessorComponent(GameObject parentObject) : base(parentObject)
        {
            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("DestructionProcessorComponent can only work when IDestructible is attached");
            }
            destructibleComponent = parentObject.GetComponent<IDestructible>();

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("DestructionProcessorComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                CalculateCollisionResults(Object, destructibleComponent, collision.gameObjectB);
            }
        }

        /// <summary>
        /// Calculates the results of the collision.
        /// Should be called in AfterPhysicalCalculation
        /// </summary>
        static void CalculateCollisionResults(GameObject thisObj, IDestructible thisDestructible, GameObject other)
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
