using Genbox.VelcroPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models.Components.Interfaces
{

    /// <summary>
    /// Provides an option to get or set position of this object
    /// </summary>
    public interface IPositionProvider
    {
        /// <summary>
        /// Object's position in 2d game space relative to the parent (or global if parent is not present)
        /// </summary>
        public Vector2 Position { get; set; }
    }
}
