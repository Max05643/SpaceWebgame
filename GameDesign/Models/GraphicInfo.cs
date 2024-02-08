using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represents an information about graphic object
    /// </summary>
    public class GraphicInfo
    {


        /// <summary>
        /// Stores information about current status of particular animated object
        /// </summary>
        public class AnimationInfo
        {
            /// <summary>
            /// Seconds that are passed since the beginning of the animation.
            /// Used for non-looped animations only
            /// </summary>
            public float SecondsCompleted { get; set; } = 0;
        }

        /// <summary>
        /// Information about current status of the animation.
        /// Can be null if there is no animation
        /// </summary>
        public AnimationInfo? ObjectAnimationInfo { get; set; } = null;

        /// <summary>
        /// Target sprite's size in global space. If not null, sprite will be scaled on client to fit this size
        /// </summary>
        public Vector2 TargetSize { get; set; } = Vector2.Zero;

        /// <summary>
        /// A reference to an information about graphics that is stored in GraphicLibrary.
        /// Default value "None" is an empty object
        /// </summary>
        public string GraphicLibraryEntryName = "None";

    }
}
