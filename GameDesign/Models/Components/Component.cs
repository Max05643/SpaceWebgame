using GameServerDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{

    /// <summary>
    /// Represents a part of GameObject that handles one of its apsects
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Parent object
        /// </summary>
        public GameObject Object { get; private set; }

        /// <summary>
        /// Creates new component with specified parent GameObject. Will automatically attach itself to parentObject
        /// </summary>
        public Component(GameObject parentObject)
        {
            Object = parentObject;
            parentObject.AttachComponent(this);
        }

        /// <summary>
        /// Is called when the GameObject is deleted. Does all the cleanup
        /// </summary>
        public virtual void Destroy()
        {

        }

        /// <summary>
        /// Called every frame before physical calculation
        /// </summary>
        public virtual void BeforePhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {

        }
        /// <summary>
        /// Called every frame after physical calculation
        /// </summary>
        public virtual void AfterPhysicalCalculation(float deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {

        }
    }
}
