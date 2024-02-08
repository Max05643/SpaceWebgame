using GameDesign.Models.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components
{
    /// <summary>
    /// Provides a simple way to store object's position that is not connected to the physical engine
    /// </summary>
    public class EmptyPhysicalComponent : Component, IPositionProvider
    {
        /// <summary>
        /// Object's position in 2d game space relative to the parent (or global if parent is not present)
        /// </summary>
        public Vector2 Position {  get; set; } = Vector2.Zero;
        public EmptyPhysicalComponent(GameObject parentObject) : base(parentObject)
        {
        }
    }
}
