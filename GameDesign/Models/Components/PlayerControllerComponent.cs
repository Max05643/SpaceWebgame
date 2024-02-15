using GameDesign.GameState;
using GameDesign.Models.Components.Interfaces;
using GameDesign.Utils;
using GameServerDefinitions;
using Genbox.VelcroPhysics.Dynamics.Joints;
using Genbox.VelcroPhysics.Dynamics.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Added to a GameObject that is controlled by player.
    /// Will apply force to its own PhysicalComponent to move
    /// </summary>
    public class PlayerControllerComponent : Component, IDestructible, IHasHealth
    {


        /// <summary>
        /// Number of game points that is currently held by this player
        /// </summary>
        public int Points { get; private set; } = 0;

        /// <summary>
        /// The cost of restoring player's current health to max level in points
        /// </summary>
        public int GetRepairCost()
        {
            return MaxHealth - Health;
        }

        /// <summary>
        /// Will spend points to restore health if there is enough points
        /// </summary>
        void RepairHealth()
        {
            int repairCost = GetRepairCost();
            if (Health < MaxHealth && Points >= repairCost)
            {
                RemovePoints(repairCost);
                Health = MaxHealth;
            }
        }

        /// <summary>
        /// Invests 10 points in specified characteristic and updates variables.
        /// Does nothing if operation is impossible, i.e. not in a safe zone or not enough points
        /// </summary>
        /// <param name="investmentType"></param>
        void InvestInCharacteristic(PlayerInvestmentState.InvestmentType investmentType)
        {
            if (Points < 10 || Status != PlayerStatus.SafeZone || !playerInvestmentState.CanInvest(investmentType))
                return;
            playerInvestmentState.InvestPoints(investmentType, 10);
            RemovePoints(10);
            playerInvestmentState.SetupPlayerControllerComponent(this);
        }

        /// <summary>
        /// Information about already invested points per caregory
        /// </summary>
        public IReadOnlyDictionary<PlayerInvestmentState.InvestmentType, int> AlreadyInvested => playerInvestmentState.AlreadyInvested;

        readonly PlayerInvestmentState playerInvestmentState = new PlayerInvestmentState();


        /// <summary>
        /// Possible stages of player's game object lifecycle
        /// </summary>
        public enum PlayerStatus
        {
            /// <summary>
            /// Player just spawned and does not collide with anything
            /// </summary>
            Invincible = 0,
            /// <summary>
            /// Main stage
            /// </summary>
            Alive = 1,
            /// <summary>
            /// Player already died and his game object will be removed after death animation
            /// </summary>
            Dead = 2,
            /// <summary>
            /// Player is in a safe zone, he can't attack and can't be attacked
            /// </summary>
            SafeZone = 3
        }

        /// <summary>
        /// Stage's of player's game object lifecycle
        /// </summary>
        public PlayerStatus Status { get; private set; } = PlayerStatus.Invincible;

        /// <summary>
        /// Health of this player. Do not set values less than 1 directly
        /// </summary>
        public int Health { get; set; } = 100;

        /// <summary>
        /// Player's max possible health
        /// </summary>
        public int MaxHealth { get; set; } = 100;

        /// <summary>
        /// The amount of damage this player inflicts on other IDestructibles
        /// </summary>
        public int Damage { get; set; } = 0;

        /// <summary>
        /// The amount of damage this player inflicts on other IDestructibles via its projectiles
        /// </summary>
        public int ShotDamage { get; set; } = 0;

        /// <summary>
        /// Id of the player controlling this object
        /// </summary>
        public Guid PlayerId { get; private set; }

        /// <summary>
        /// Player's max velocity
        /// </summary>
        public float Power { get; set; } = 0;


        /// <summary>
        /// Player's acceleration
        /// </summary>
        public float Acceleration { get; set; } = 0;

        /// <summary>
        /// Player's angular speed in radians per seconds
        /// </summary>
        public float AngularSpeed { get; set; } = 0;

        /// <summary>
        /// Seconds between player's shots
        /// </summary>
        public float TimePerShot { get; set; } = 0;


        bool IDestructible.IsInvincible => false;

        bool IDestructible.IsInfiniteDamage => false;

        readonly PhysicalComponent physicalComponent;
        readonly SpriteComponent spriteComponent;
        readonly AnimatorComponent animatorComponent;

        /// <summary>
        /// Seconds until the next shot can be made
        /// </summary>
        float timeToNextShot = 0;


        /// <summary>
        /// Seconds until this player switches to PlayerStatus.Alive
        /// </summary>
        float timeUntilAliveStage = 0;

        /// <param name="playerId">Id of the player controlling this object</param>
        /// <param name="power">Player's max velocity</param>
        /// <param name="angularSpeed">Player's angular speed in radians per seconds</param>
        /// <param name="acceleration">Player's acceleration</param>
        public PlayerControllerComponent(GameObject parentObject, Guid playerId, float secondsUntilAliveStage = 5f) : base(parentObject)
        {
            PlayerId = playerId;
            timeUntilAliveStage = secondsUntilAliveStage;

            if (!parentObject.HasComponent<PhysicalComponent>())
            {
                throw new InvalidOperationException("PlayerControllerComponent can only work when PhysicalComponent is attached");
            }

            physicalComponent = parentObject.GetComponent<PhysicalComponent>();


            if (!parentObject.HasComponent<SpriteComponent>())
            {
                throw new InvalidOperationException("PlayerControllerComponent can only work when SpriteComponent is attached");
            }

            spriteComponent = parentObject.GetComponent<SpriteComponent>();

            if (!parentObject.HasComponent<AnimatorComponent>())
            {
                throw new InvalidOperationException("PlayerControllerComponent can only work when AnimatorComponent is attached");
            }

            animatorComponent = parentObject.GetComponent<AnimatorComponent>();
            animatorComponent.OnAnimationStarted += AfterAnimation;

            animatorComponent.StartAnimation("idle_invincible");

            physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.StaticWorldWall | CollisionCategory.SafeZone);

            playerInvestmentState.SetupPlayerControllerComponent(this);
        }

        void AfterAnimation(string animation)
        {
            if (animation == "destroyed")
                KillThisPlayer();
        }


        /// <summary>
        /// Kills this player and removes its Game object immediately
        /// </summary>
        public virtual void KillThisPlayer()
        {
            Object.GameStateManager.playersManager.PlayerDied(PlayerId);
        }

        public override void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {

            if (Status == PlayerStatus.Invincible)
            {
                timeUntilAliveStage -= deltaTime;

                if (timeUntilAliveStage <= 0)
                {
                    Status = PlayerStatus.Alive;
                    physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.All);
                }
            }

            if (Status == PlayerStatus.Dead)
            {
                physicalComponent.Velocity = Vector2.Zero;
            }
            else
            {
                PlayerInput lastInput = playerInputProvider.PopPlayerInput(PlayerId);

                if (Math.Abs(lastInput.MovementPower) > float.Epsilon)
                {
                    animatorComponent.StartAnimationIfNotStartedYet((Status == PlayerStatus.Alive) ? "movement" : "movement_invincible");
                }
                else
                {
                    animatorComponent.StartAnimationIfNotStartedYet((Status == PlayerStatus.Alive) ? "idle" : "idle_invincible");
                }

                HandleMovement(deltaTime, lastInput);
                HandleShot(deltaTime, lastInput);
                HandleInvestmentRequest(lastInput);
                HandleRepairRequest(lastInput);
            }


        }


        /// <summary>
        /// Should be called when player enters safe zone
        /// </summary>
        public void EnterSafeZone()
        {
            Status = PlayerStatus.SafeZone;
            physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.StaticWorldWall | CollisionCategory.SafeZone);
        }
        /// <summary>
        /// Should be called when player exits safe zone
        /// </summary>
        public void ExitSafeZone()
        {
            Status = PlayerStatus.Alive;
            physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.All);
        }

        /// <summary>
        /// Adds amount to this player's points
        /// </summary>
        public void AddPoints(int amount)
        {
            Points += amount;
        }

        /// <summary>
        /// Reduces player's points on specified amount, but points can't be less than 0
        /// </summary>
        void RemovePoints(int amount)
        {
            Points -= amount;
            Points = Math.Max(0, Points);
        }

        /// <summary>
        /// Handles player's movement from current input
        /// </summary>
        void HandleMovement(float deltaTime, PlayerInput input)
        {
            Object.Angle = input.Angle;
            physicalComponent.Velocity = physicalComponent.Velocity.MoveTowards((new Vector2(MathF.Cos(Object.Angle), MathF.Sin(Object.Angle))) * Power * input.MovementPower, deltaTime * Acceleration);
        }
        /// <summary>
        /// Handles investment request from current input
        /// </summary>
        void HandleInvestmentRequest(PlayerInput input)
        {
            if (input.InvestmentRequest != null)
            {
                InvestInCharacteristic(input.InvestmentRequest.Value);
            }
        }
        /// <summary>
        /// Handles repair request from current input
        /// </summary>
        void HandleRepairRequest(PlayerInput input)
        {
            if (input.RepairRequest)
            {
                RepairHealth();
            }
        }
        void HandleShot(float deltaTime, PlayerInput input)
        {

            if (Status != PlayerStatus.Alive)
                return;

            timeToNextShot -= deltaTime;


            if (input.IsFiring && timeToNextShot <= 0)
            {
                Object.GameStateManager.gameObjectFactory.CreateProjectile(Object, ShotDamage, 70);

                timeToNextShot = TimePerShot;
            }

            timeToNextShot = MathF.Max(0, timeToNextShot);
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
            if (Status == PlayerStatus.Dead)
                return true;

            Health -= damageAmount;
            if (Health <= 0)
            {
                return ((IDestructible)this).DieImmediatly();
            }
            else
            {
                Object.GameStateManager.audioManager.AddAudioClipForTheCurrentFrame(new SoundEffect("ex0", Object.Position, 1f));
                return false;
            }
        }

        int IDestructible.GetInflictedDamage()
        {
            return Damage;
        }

        bool IDestructible.DieImmediatly()
        {
            Object.GameStateManager.audioManager.AddAudioClipForTheCurrentFrame(new SoundEffect("ex1", Object.Position, 40f));
            physicalComponent.ChangeObjCollidesWithCategories(CollisionCategory.None);
            animatorComponent.StartAnimationIfNotStartedYet("destroying");
            Status = PlayerStatus.Dead;
            return true;
        }






    }
}
