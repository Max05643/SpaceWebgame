using GameDesign.GameState;
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
    /// Component for players' projectiles
    /// </summary>
    public class ProjectileComponent : Component, IDestructible
    {

        public int Damage { get; private set; }
        public Guid ShotByPlayerId { get; private set; }

        readonly PhysicalComponent physicalComponent;

        float timeToLiveInSeconds = 0;


        public override void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            timeToLiveInSeconds -= deltaTime;

            if (timeToLiveInSeconds < 0)
            {
                ((IDestructible)this).DieImmediatly();
            }
        }
        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                DestructionUtils.CalculateCollisionResults(Object, this, collision.gameObjectB);
            }
        }
        public ProjectileComponent(GameObject parentObject, GameObject playersObject, int damage, Guid shotByPlayerId, Vector2 velocity, float timeToLiveInSeconds) : base (parentObject)
        {
            this.timeToLiveInSeconds = timeToLiveInSeconds;

            Damage = damage;
            ShotByPlayerId = shotByPlayerId;


            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("ProjectileComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();

            physicalComponent.Velocity = velocity;


            parentObject.GameStateManager.audioManager.AddAudioClipForTheCurrentFrame(new SoundEffect("lr", playersObject.Position, 20f));

        }

        bool IDestructible.IsInvincible => false;

        bool IDestructible.IsInfiniteDamage => false;

        bool IDestructible.DieImmediatly()
        {
            Object.RemoveThisObject();
            return true;
        }

        int IDestructible.GetInflictedDamage()
        {
            return Damage;
        }

        bool IDestructible.ReceiveDamage(int damageAmount)
        {
            return ((IDestructible)this).DieImmediatly();
        }
    }
}
