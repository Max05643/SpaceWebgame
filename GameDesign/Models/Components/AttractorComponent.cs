using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Will attract other objects using Physical component and ApplyForce
    /// </summary>
    public class AttractorComponent : Component
    {

        readonly float force;
        readonly float radiusSuqared;
        readonly PhysicalComponent physicalComponent;
        public AttractorComponent(GameObject parentObject, float force, float radius) : base(parentObject)
        {
            this.force = force;
            this.radiusSuqared = radius * radius;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("AttractorComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var obj in physicalComponent.CurrentCollisions)
            {
                if (!obj.IsDestroyed && obj.HasComponent<PhysicalComponent>())
                {
                    var othersPhysicalComponent = obj.GetComponent<PhysicalComponent>();
                    if (othersPhysicalComponent.BodyType == Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic)
                    {
                        var traction = (Object.Position - obj.Position);
                        float currentRadiusNormalized = traction.LengthSquared() / radiusSuqared;

                        currentRadiusNormalized = Math.Clamp(currentRadiusNormalized, 0.3f, 1f);

                        traction.Normalize();
                        traction *= force / currentRadiusNormalized;
                        othersPhysicalComponent.ApplyForce(traction);
                    }
                }
            }
        }

    }
}
