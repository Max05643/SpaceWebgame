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
    /// Represents an object that can have health, receive damage and inflict damage on others
    /// </summary>
    public class AsteroidComponent : Component, IDestructible, IHasHealth
    {
        /// <summary>
        /// Health of this object
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Object's max possible health
        /// </summary>
        public int MaxHealth { get; private set; }

        /// <summary>
        /// The amount of damage this object inflicts on other IDestructibles
        /// </summary>
        public int Damage { get; private set; }

        /// <summary>
        /// Is this object invincible? Health is ignored on true
        /// </summary>
        public bool IsInvincible { get; private set; }

        /// <summary>
        /// Will this object inflict infinite damage? Damage is ignored on true
        /// </summary>
        public bool IsInfiniteDamage { get; private set; }


        readonly PhysicalComponent physicalComponent;
        readonly AnimatorComponent animatorComponent;

        readonly bool createCrystalAfterDestruction;
        readonly int crystalPoints;

        public AsteroidComponent(GameObject parentObject, int health, int damage, bool isInvincible, bool isInfiniteDamage, bool createCrystalAfterDestruction = false, int crystalPoints = 0) : base(parentObject)
        {
            this.createCrystalAfterDestruction = createCrystalAfterDestruction;
            this.crystalPoints = crystalPoints;

            Health = MaxHealth = health;
            Damage = damage;
            IsInfiniteDamage = isInfiniteDamage;
            IsInvincible = isInvincible;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("DestructorComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();


            if (!parentObject.HasComponent<AnimatorComponent>())
            {
                throw new InvalidOperationException("DestructorComponent can only work when AnimatorComponent is attached");
            }

            animatorComponent = parentObject.GetComponent<AnimatorComponent>();
            animatorComponent.OnAnimationStarted += AfterAnimation;

            animatorComponent.StartAnimation("idle");
        }


        void AfterAnimation(string animation)
        {
            if (animation == "destroyed")
            {
                Object.RemoveThisObject();

                if (createCrystalAfterDestruction)
                    Object.GameStateManager.gameObjectFactory.CreateCrystal(Object.Position, crystalPoints);
            }
        }

        public override void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            foreach (var collision in physicalComponent.CurrentFrameCollisions)
            {
                DestructionUtils.CalculateCollisionResults(Object, this, collision.gameObjectB);
            }
        }

        bool IDestructible.ReceiveDamage(int damageAmount)
        {

            if (IsInvincible || Health <= 0)
                return false;

            Health -= damageAmount;

            if (Health <= 0)
            {
                return ((IDestructible)this).DieImmediatly();
            }
            else
            {
                animatorComponent.StartAnimationIfNotStartedYet("damaging");
                return false;
            }
        }

        int IDestructible.GetInflictedDamage()
        {
            return Damage;
        }

        bool IDestructible.DieImmediatly()
        {
            if (IsInvincible)
                return false;
            else
            {
                Object.GameStateManager.audioManager.AddAudioClipForTheCurrentFrame(new SoundEffect("ex0", Object.Position, 40f));
                animatorComponent.StartAnimationIfNotStartedYet("destroying");
                physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.None);
                return true;
            }
        }
    }
}
