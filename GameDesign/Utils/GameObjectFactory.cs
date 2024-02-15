using GameDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.GameState;
using GameDesign.Models.Components;

namespace GameDesign.Utils
{
    /// <summary>
    /// Handles creation of GameObjects
    /// </summary>
    public class GameObjectFactory
    {
        readonly GameStateManager gameStateManager;

        public GameObjectFactory(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }

        /// <summary>
        /// Creates a space station and safe zone around it
        /// </summary>
        public virtual GameObject CreateSpaceStation(Vector2 position)
        {


            float raduis = 40f;





            GameObject obj = new GameObject("Space station sprite", gameStateManager);
            PhysicalComponent.FromCircle(
                obj,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Static,
                gameStateManager.physicsManager.World,
                10f,
                false,
                1,
                false,
                CollisionCategory.RegularObject,
                CollisionCategory.None);
            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupTargetSize(Vector2.One * 20f);
            spriteComponent.SetupGraphicLibraryEntryName("SpaceStation");

            gameStateManager.sceneManager.RegisterGameObject(obj);



            var spaceStationZone = new GameObject("Space station zone", gameStateManager);
            PhysicalComponent.FromCircle(
                spaceStationZone,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Static,
                gameStateManager.physicsManager.World,
                raduis,
                true,
                1,
                true,
                CollisionCategory.SafeZone,
                CollisionCategory.Player);
            var spriteComponent1 = new SpriteComponent(spaceStationZone);
            spriteComponent1.SetupNothing();
            _ = new SpaceStationComponent(spaceStationZone, obj);

            gameStateManager.sceneManager.RegisterGameObject(spaceStationZone);
            return spaceStationZone;
        }

        /// <summary>
        /// Creates black hole, which attracts physical objects and destroys them via IDestructible system
        /// </summary>
        public virtual GameObject CreateBlackHole(Vector2 position)
        {

            float raduis = 30f;

            var attractor = new GameObject("Attractor", gameStateManager);
            PhysicalComponent.FromCircle(
                attractor,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Static,
                gameStateManager.physicsManager.World,
                raduis,
                true,
                1,
                true,
                CollisionCategory.RegularObject,
                CollisionCategory.All ^ CollisionCategory.Projectile);
            _ = new AttractorComponent(attractor, 100f, raduis);
            var spriteComponent1 = new SpriteComponent(attractor);
            spriteComponent1.SetupNothing();


            GameObject obj = new GameObject("Black hole", gameStateManager);
            PhysicalComponent.FromCircle(
                obj,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Static,
                gameStateManager.physicsManager.World,
                6f,
                true,
                1,
                false,
                CollisionCategory.RegularObject,
                CollisionCategory.All);
            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupTargetSize(new Vector2(21.6f, 12f));
            spriteComponent.SetupGraphicLibraryEntryName("BlackHole");

            _ = new BlackHoleComponent(obj, attractor);
            _ = new DestructionProcessorComponent(obj);

            gameStateManager.sceneManager.RegisterGameObject(obj);
            gameStateManager.sceneManager.RegisterGameObject(attractor);
            return obj;
        }

        /// <summary>
        /// Creates crystal that gives points to player when picked
        /// </summary>
        public virtual GameObject CreateCrystal(Vector2 position, int amount)
        {
            GameObject obj = new GameObject("Crystal", gameStateManager);
            PhysicalComponent.FromCircle(
                obj,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic,
                gameStateManager.physicsManager.World,
                2.5f,
                true,
                1,
                true,
                CollisionCategory.RegularObject,
                CollisionCategory.Player);
            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupTargetSize(Vector2.One * 5);
            spriteComponent.SetupGraphicLibraryEntryName("Crystal");

            _ = new PickableComponent(obj, amount);

            gameStateManager.sceneManager.RegisterGameObject(obj);
            return obj;
        }

        /// <summary>
        /// Creates projectile shot by specified player
        /// </summary>
        public virtual GameObject CreateProjectile(GameObject playersObject, int damage, int speed)
        {
            var playerController = playersObject.GetComponent<PlayerControllerComponent>();
            var playerId = playerController.PlayerId;
            Vector2 direction = new Vector2(MathF.Cos(playersObject.Angle), MathF.Sin(playersObject.Angle));


            GameObject obj = new GameObject($"Player's {playerId} projectile", gameStateManager);

            obj.Angle = playersObject.Angle;

            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupGraphicLibraryEntryName("Projectile");
            spriteComponent.SetupTargetSize(new Vector2(2.25f, 1f));

            PhysicalComponent.FromBox(
                obj,
                playersObject.Position + direction * 5f,
                Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic,
                gameStateManager.physicsManager.World,
                new Vector2(2.25f, 1f),
                true,
                1,
                true,
                CollisionCategory.Projectile,
                CollisionCategory.All ^ CollisionCategory.Projectile);
            _ = new ProjectileComponent(obj, playersObject, damage, playerId, direction * speed, 5);
            _ = new DestructionProcessorComponent(obj);

            gameStateManager.sceneManager.RegisterGameObject(obj);
            return obj;
        }
        public virtual GameObject CreatePlayerGameObject(Vector2 position, Guid playerId)
        {
            GameObject obj = new GameObject($"Player {playerId}", gameStateManager);
            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupGraphicLibraryEntryName("PlayerNomovement");
            spriteComponent.SetupTargetSize(Vector2.One * 5);

            var animator = new AnimatorComponent(obj, gameStateManager.graphicLibrary);

            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("PlayerNomovement"), "idle");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("PlayerDestroy", "destroyed"), "destroying");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("None"), "destroyed");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("PlayerMovement"), "movement");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("PlayerMovementInvincible"), "movement_invincible");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("PlayerNomovementInvincible"), "idle_invincible");


            PhysicalComponent.FromCircle(
                obj,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic,
                gameStateManager.physicsManager.World,
                2.5f,
                true,
                1,
                false,
                CollisionCategory.Player,
                CollisionCategory.All);
            var playerController = new PlayerControllerComponent(obj, playerId, 5);
            _ = new DestructionProcessorComponent(obj);

            gameStateManager.sceneManager.RegisterGameObject(obj);
            return obj;
        }
        public virtual GameObject CreateAsteroidGameObject(Vector2 position)
        {
            GameObject obj = new GameObject("Asteroid", gameStateManager);
            PhysicalComponent.FromCircle(
                obj,
                position,
                Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic,
                gameStateManager.physicsManager.World,
                5,
                true,
                1,
                false,
                CollisionCategory.RegularObject,
                CollisionCategory.All);
            var spriteComponent = new SpriteComponent(obj);
            spriteComponent.SetupTargetSize(Vector2.One * 10);
            var animator = new AnimatorComponent(obj, gameStateManager.graphicLibrary);

            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("AsteroidIdle"), "idle");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("AsteroidDestroy", "destroyed"), "destroying");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("None"), "destroyed");
            animator.AddAnimationNode(new AnimatorComponent.AnimationNode("AsteroidDamaged", "idle"), "damaging");


            _ = new AsteroidComponent(obj, 200, 10, false, false, true, 10);
            _ = new DestructionProcessorComponent(obj);

            gameStateManager.sceneManager.RegisterGameObject(obj);
            return obj;
        }
    }
}
