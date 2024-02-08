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
    /// Represents an object that destroys any IDestructible on touch
    /// </summary>
    public class BlackHoleComponent : Component, IDestructible
    {
        readonly PhysicalComponent physicalComponent;
        readonly GameObject attractor;


        /// <param name="attractor">Game objects with attractor component. Will be removed if this component is destroyed</param>
        public BlackHoleComponent(GameObject parentObject, GameObject attractor) : base(parentObject)
        {

            this.attractor = attractor;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("BlackHoleComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();
        }


        public override void Destroy()
        {
            attractor.RemoveThisObject();
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                DestructionUtils.CalculateCollisionResults(Object, this, collision.gameObjectB);
            }
        }
        bool IDestructible.IsInvincible => true;

        bool IDestructible.IsInfiniteDamage => true;

        bool IDestructible.DieImmediatly() => false;

        int IDestructible.GetInflictedDamage() => 0;

        bool IDestructible.ReceiveDamage(int damageAmount) => false;
    }
}
