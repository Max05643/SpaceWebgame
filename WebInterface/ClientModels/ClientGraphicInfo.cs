using MessagePack;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameDesign.Models.GraphicInfo;

namespace WebInterface.ClientModels
{

    /// <summary>
    /// Represents an information about graphic object,
    /// where part of the information is represented as id of the object in graphic library
    /// </summary>    
    [MessagePackObject]
    public class ClientGraphicInfo
    {

        /// <summary>
        /// Stores information about current status of particular animated object
        /// </summary>
        [MessagePackObject]
        public class ClientAnimationInfo
        {
            /// <summary>
            /// Seconds that are passed since the beginning of the animation.
            /// Used for non-looped animations only
            /// </summary>
            [Key("_scmp")]
            public float SecondsCompleted { get; set; } = 0;
        }

        /// <summary>
        /// Information about current status of the animation.
        /// Can be null if there is no animation
        /// </summary>
        [Key("_an")]
        public ClientAnimationInfo? ObjectAnimationInfo { get; set; } = null;

        /// <summary>
        /// Target sprite's size in global space. If not null, sprite will be scaled on client to fit this size
        /// </summary>
        [Key("_ts")]
        public Vector2 TargetSize { get; set; } = Vector2.Zero;

        /// <summary>
        /// A reference to an information about graphics that is stored in GraphicLibrary
        /// </summary>
        [Key("_gl")]
        public uint GraphicLibraryEntryId { get; set; } = 0;
    }
}
