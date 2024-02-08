using GameDesign.Utils;
using GameServerDefinitions;
using Genbox.VelcroPhysics.Collision.Narrowphase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Represents a zone where player become invincible and can not attack
    /// </summary>
    public class SpaceStationComponent : Component
    {

        readonly GameObject spriteObject;
        readonly PhysicalComponent physicalComponent;
        public SpaceStationComponent(GameObject parentObject, GameObject spriteObject) : base(parentObject)
        {
            this.spriteObject = spriteObject;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("SpaceStationComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();
        }


        public override void Destroy()
        {
            spriteObject.RemoveThisObject();
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                if (!collision.gameObjectB.IsDestroyed && collision.gameObjectB.HasComponent<PlayerControllerComponent>())
                {
                    var playerController = collision.gameObjectB.GetComponent<PlayerControllerComponent>();
                    playerController.EnterSafeZone();
                }
            }
            foreach (var separation in physicalComponent.CurrentFrameSeparations)
            {
                var playerController = separation.gameObjectB.GetComponent<PlayerControllerComponent>();
                playerController.ExitSafeZone();
            }
        }
    }
}
