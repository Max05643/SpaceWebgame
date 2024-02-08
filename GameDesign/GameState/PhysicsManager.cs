using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.Utils;
using GameDesign.Models;

namespace GameDesign.GameState
{
    /// <summary>
    /// Manages interaction with physical engine
    /// </summary>
    public class PhysicsManager
    {

        /// <summary>
        /// Physical engine world
        /// </summary>
        public World World { get; private set; }

        readonly GameStateManager gameStateManager;

        /// <param name="debugTime">PhysicsManager will measure its perfomance and make GetAverageTickTimeAndReset() avalible if true</param>
        public PhysicsManager(World world, GameStateManager gameStateManager)
        {
            World = world;
            this.gameStateManager = gameStateManager;
        }

        /// <summary>
        /// Runs a physics update with specified delta time in seconds
        /// </summary>
        public virtual void Update(float deltaTime)
        {
            World.Step(deltaTime);
        }

        /// <summary>
        /// Creates new PhysicsManager with a world that is restricted by rectangular boundaries with specified size
        /// </summary>
        public static PhysicsManager CreateWithBoundaries(Vector2 gravity, Vector2 worldSize, GameStateManager gameStateManager)
        {
            World world = new World(gravity);

            Body leftWall = BodyFactory.CreateEdge(world, -worldSize * 0.5f, new Vector2(-worldSize.X * 0.5f, worldSize.Y * 0.5f)); //Left 
            Body rightWall = BodyFactory.CreateEdge(world, new Vector2(worldSize.X * 0.5f, - worldSize.Y * 0.5f), new Vector2(worldSize.X * 0.5f, worldSize.Y * 0.5f)); // Right
            Body topWall = BodyFactory.CreateEdge(world, new Vector2(-worldSize.X * 0.5f, worldSize.Y * 0.5f), new Vector2(worldSize.X * 0.5f, worldSize.Y * 0.5f)); //Top
            Body bottomWall = BodyFactory.CreateEdge(world, new Vector2(-worldSize.X * 0.5f, -worldSize.Y * 0.5f), new Vector2(worldSize.X * 0.5f, -worldSize.Y * 0.5f)); //Bottom

            foreach (var fixture in leftWall.FixtureList.Concat(rightWall.FixtureList).Concat(topWall.FixtureList).Concat(bottomWall.FixtureList))
            {
                fixture.CollidesWith = CollisionCategory.All.ToPhysicalEngineCategory();
                fixture.CollisionCategories = CollisionCategory.StaticWorldWall.ToPhysicalEngineCategory();
            }

            return new PhysicsManager(world, gameStateManager);
        }




    }
}
