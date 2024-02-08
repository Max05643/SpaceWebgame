using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represents a sound effect played on frontend
    /// </summary>
    public class SoundEffect
    {
        /// <summary>
        /// The name of the clip
        /// </summary>
        public string AudioClipName { get; set; }

        /// <summary>
        /// Position of the clip in global space.
        /// May be null, then the effect will be played with the same volume for all players
        /// </summary>
        public Vector2? Position { get; set; } = null;

        /// <summary>
        /// Radius in global space, where the effect will be heard. Should not be null, if Position is not null
        /// </summary>
        public float? Radius { get; set; } = null;


        /// <summary>
        /// Unique id of this sound effect.
        /// The ids always grow with new instances in one game
        /// </summary>
        public ulong Id { get; set; } = 0;

        public SoundEffect(string audioClipName)
        {
            AudioClipName = audioClipName;
        }

        public SoundEffect(string audioClipName, Vector2 position, float radius)
        {
            AudioClipName = audioClipName;
            Position = position;
            Radius = radius;
        }
    }
}
