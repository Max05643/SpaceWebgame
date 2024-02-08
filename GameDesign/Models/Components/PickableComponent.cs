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
    /// Represents an object that can be picked by player to receive points
    /// </summary>
    public class PickableComponent : Component
    {

        /// <summary>
        /// The amount of points this object gives
        /// </summary>
        public int Amount { get; private set; }
        readonly PhysicalComponent physicalComponent;

        /// <param name="amount">The amount of points this object gives when picked by a player</param>
        public PickableComponent(GameObject parentObject, int amount) : base(parentObject)
        {

            Amount = amount;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("PickableComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();


        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                if (collision.gameObjectB.HasComponent<PlayerControllerComponent>())
                {
                    collision.gameObjectB.GetComponent<PlayerControllerComponent>().AddPoints(Amount);
                    Object.RemoveThisObject();
                    Object.GameStateManager.audioManager.AddAudioClipForTheCurrentFrame(new SoundEffect("pk", collision.gameObjectB.Position, 10f));
                    break;
                }
            }
        }
    }
}
