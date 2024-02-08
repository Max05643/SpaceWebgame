using GameDesign.Models.Components;
using GameDesign.Models.Components.Interfaces;
using MessagePack;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebInterface.ClientModels
{
    /// <summary>
    /// Represents game object
    /// </summary>
    [MessagePackObject]
    public class ClientGameObject
    {

        /// <summary>
        /// Rotation of the object in radians
        /// </summary>
        [Key("_ang")]
        public float Angle { get; set; } = 0;

        /// <summary>
        /// Position in global space
        /// </summary>
        [Key("_ps")]
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// Velocity in global space
        /// </summary>
        [Key("_vl")]
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        /// <summary>
        /// Information about displaying game object on client's side
        /// </summary>
        [Key("_gi")]
        public ClientGraphicInfo GraphicInfo { get; set; } = new ClientGraphicInfo();

        /// <summary>
        /// Stores special features for some objects:
        /// - Health (0 to 100) for IHasHealth objects
        /// - Text for IHasText objects
        /// </summary>
        [Key("_fc")]
        public Dictionary<string, string> Features = new Dictionary<string, string>();


        /// <summary>
        /// Children of this object. If object has a parent, this object should be passed in this dictionary
        /// </summary>
        [Key("_chl")]
        public Dictionary<string, ClientGameObject>? Children { get; set; } = null;

    }
}
