using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represents an information about graphics of some object
    /// </summary>
    [Serializable]
    public class GraphicLibraryEntry
    {
        public enum GraphicType
        {
            /// <summary>
            /// Nothing is displayed
            /// </summary>
            None = 0,
            /// <summary>
            /// Static sprite
            /// </summary>
            Static = 1,
            /// <summary>
            /// Animated sprite
            /// </summary>
            Animated = 2
        }

        /// <summary>
        /// Type of this graphic
        /// </summary>
        public GraphicType Type { get; set; } = GraphicType.None;

        public AnimatedSpriteInfo? AnimatedSpriteInfo { get; set; } = null;

        public SpriteInfo? SpriteInfo { get; set; } = null;
    }

    [Serializable]
    public class SpriteInfo
    {
        /// <summary>
        /// Name of the object's sprite
        /// </summary>
        public string? Sprite { get; set; } = null;
    }

    [Serializable]
    public class AnimatedSpriteInfo
    {
        /// <summary>
        /// Animation name. Used if IsAnimated
        /// </summary>
        public string? AnimationName { get; set; } = null;

        /// <summary>
        /// Is animation looped?
        /// </summary>
        public bool IsLoop { get; set; } = true;

        /// <summary>
        /// Target time of one animation loop on frontend
        /// </summary>
        public float TargetTimeInSeconds { get; set; } = 1;

        /// <summary>
        /// Used only for animations where IsLoop = false. Seconds since the beginning of the animtion
        /// </summary>
        public float CompletedTimeInSeconds { get; set; } = 0;

    }

}
