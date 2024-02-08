using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Filtering;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Dynamics.Solver;
using Genbox.VelcroPhysics.Factories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.Utils;
using GameDesign.Models.Components.Interfaces;
using GameServerDefinitions;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Physical representation of the object 
    /// </summary>
    public class PhysicalComponent : Component, IPositionProvider
    {

        /// <summary>
        /// Physical body type
        /// </summary>
        public BodyType BodyType => Body.BodyType;
        public bool IsSensor { get; private set; }

        /// <summary>
        /// Physical body 
        /// </summary>
        private Body Body { get; set; }

        /// <summary>
        /// Object's position in 2d game space relative to the parent (or global if parent is not present)
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return Body.Position;
            }
            set
            {
                Body.Position = value;
            }
        }

        /// <summary>
        /// Object's velocity in 2d game space. Set directly with caution
        /// </summary>
        public Vector2 Velocity
        {
            get => Body.LinearVelocity;
            set
            {
                Body.LinearVelocity = value;
            }
        }


        /// <summary>
        /// List of all object's separations that happened on the current frame
        /// </summary>
        private List<Collision> CurrentFrameSeparationsList { get; set; } = new List<Collision>();

        /// <summary>
        /// IEnumerable of all object's separations that happened on the current frame
        /// </summary>
        public IEnumerable<Collision> CurrentFrameSeparations => (IsDetectingCollisions ? CurrentFrameSeparationsList : Enumerable.Empty<Collision>());





        /// <summary>
        /// List of all object's collisions that started on the current frame
        /// </summary>
        private List<Collision> CurrentFrameCollisionsList { get; set; } = new List<Collision>();

        /// <summary>
        /// IEnumerable of all object's collisions that started on the current frame
        /// </summary>
        public IEnumerable<Collision> CurrentFrameCollisions => (IsDetectingCollisions ? CurrentFrameCollisionsList : Enumerable.Empty<Collision>());



        /// <summary>
        /// Collection of all object's current collisions
        /// </summary>
        private List<GameObject> CurrentCollisionsList { get; set; } = new List<GameObject>();

        /// <summary>
        /// IEnumerable of all object's current collisions
        /// </summary>
        public IEnumerable<GameObject> CurrentCollisions => (IsDetectingCollisions ? CurrentCollisionsList : Enumerable.Empty<GameObject>());






        /// <summary>
        /// Setups new collision categories for this object
        /// </summary>
        public void ChangeObjCollisionsCategories(CollisionCategory collisionCategory)
        {

            foreach (var fixture in Body.FixtureList)
            {
                fixture.CollisionCategories = collisionCategory.ToPhysicalEngineCategory();
            }
        }
        /// <summary>
        /// Setups new collision categories for objects that are allowed to collide with this object
        /// </summary>
        public void ChangeObjCollidesWithCategories(CollisionCategory collidesWith)
        {

            foreach (var fixture in Body.FixtureList)
            {
                fixture.CollidesWith = collidesWith.ToPhysicalEngineCategory();
            }
        }


        public bool IsDetectingCollisions { get; private set; } = false;

        /// <summary>
        /// Creates new physical component
        /// </summary>
        /// <param name="parentObject">Parent GameObject</param>
        /// <param name="position">World position</param>
        /// <param name="bodyType">Body type</param>
        /// <param name="world">Physical engine's world</param>
        private PhysicalComponent(GameObject parentObject, Vector2 position, BodyType bodyType, World world, bool isSensor = false) : base(parentObject)
        {
            Body = BodyFactory.CreateBody(world, position, bodyType: bodyType, userData: parentObject);
            IsSensor = isSensor;

            if (parentObject.parent != null)
            {
                throw new InvalidOperationException($"PhysicalComponent can not be added to {parentObject.Name}, because it already has a parent. Objects with parents can not correctly interact with physical engine. Use EmptyPhysicalComponent in order to setup position without any physical interaction.");
            }
        }

        /// <summary>
        /// Creates new physical component with a circle shape
        /// </summary>
        /// <param name="parentObject">Parent GameObject</param>
        /// <param name="position">World position</param>
        /// <param name="bodyType">Body type</param>
        /// <param name="radius">Circle radius</param>
        /// <returns></returns>
        public static PhysicalComponent FromCircle(GameObject parentObject, Vector2 position, BodyType bodyType, World world, float radius, bool detectCollisions = false, float density = 1f, bool isSensor = false, CollisionCategory collisionCategory = CollisionCategory.RegularObject, CollisionCategory collidesWith = CollisionCategory.All)
        {
            var component = new PhysicalComponent(parentObject, position, bodyType, world, isSensor);

            Fixture fixture = FixtureFactory.AttachCircle(radius, density, component.Body);
            fixture.IsSensor = isSensor;

            component.IsDetectingCollisions = detectCollisions;

            if (detectCollisions)
            {
                fixture.OnCollision += component.RegisterCollision;
                fixture.OnSeparation += component.RegisterSeparation;
            }

            fixture.CollisionCategories = collisionCategory.ToPhysicalEngineCategory();
            fixture.CollidesWith = collidesWith.ToPhysicalEngineCategory();

            return component;
        }

        /// <summary>
        /// Creates new physical component with a box shape
        /// </summary>
        /// <param name="parentObject">Parent GameObject</param>
        /// <param name="position">World position</param>
        /// <param name="bodyType">Body type</param>
        /// <param name="dimensions">Box dimensions</param>
        /// <returns></returns>
        public static PhysicalComponent FromBox(GameObject parentObject, Vector2 position, BodyType bodyType, World world, Vector2 dimensions, bool detectCollisions = false, float density = 1f, bool isSensor = false, CollisionCategory collisionCategory = CollisionCategory.RegularObject, CollisionCategory collidesWith = CollisionCategory.All)
        {

            var component = new PhysicalComponent(parentObject, position, bodyType, world, isSensor);

            Fixture fixture = FixtureFactory.AttachRectangle(dimensions.X, dimensions.Y, density, Vector2.Zero, component.Body);
            fixture.IsSensor = isSensor;
            component.IsDetectingCollisions = detectCollisions;

            if (detectCollisions)
            {
                fixture.OnCollision += component.RegisterCollision;
                fixture.OnSeparation += component.RegisterSeparation;
            }

            fixture.CollisionCategories = collisionCategory.ToPhysicalEngineCategory();
            fixture.CollidesWith = collidesWith.ToPhysicalEngineCategory();

            return component;
        }

        /// <summary>
        /// Add new collision to collisions list. Does nothing if collisions are not being detected
        /// </summary>
        private void RegisterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (IsDetectingCollisions)
            {
                if (fixtureB.Body.UserData == null)
                    return;

                if (!CurrentCollisionsList.Contains((GameObject)fixtureB.Body.UserData))
                    CurrentCollisionsList.Add((GameObject)fixtureB.Body.UserData);


                CurrentFrameCollisionsList.Add(new Collision((GameObject)fixtureA.Body.UserData, (GameObject)fixtureB.Body.UserData, contact));
            }
        }
        /// <summary>
        /// Removes a collision from collisions list. Does nothing if collisions are not being detected
        /// </summary>
        private void RegisterSeparation(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (IsDetectingCollisions && CurrentCollisionsList.Contains((GameObject)fixtureB.Body.UserData))
            {
                CurrentCollisionsList.Remove((GameObject)fixtureB.Body.UserData);
                CurrentFrameSeparationsList.Add(new Collision((GameObject)fixtureA.Body.UserData, (GameObject)fixtureB.Body.UserData, contact));
            }
        }
        /// <summary>
        /// Clears current frame's collisions list. Should be called every frame before physical calculations. Does nothing if collisions are not being detected
        /// </summary>
        private void ClearCollisionsList()
        {
            if (IsDetectingCollisions)
            {
                CurrentFrameCollisionsList.Clear();
                CurrentFrameSeparationsList.Clear();
            }
        }

        public override void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {
            ClearCollisionsList();
        }

        public override void Destroy()
        {
            Body.RemoveFromWorld();
            base.Destroy();
        }

        /// <summary>
        /// Applies force to an object using physical engine
        /// </summary>
        public void ApplyForce(Vector2 force)
        {
            Body.ApplyForce(force);
        }
    }
}
