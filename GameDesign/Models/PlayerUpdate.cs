using GameDesign.Models.Components.Interfaces;
using GameDesign.Models.Components;
using GameDesign.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Models
{
    /// <summary>
    /// Represetns all the information that is sent to player on every tick
    /// </summary>
    public class PlayerUpdate
    {
        /// <summary>
        /// Represetns information about game object that is being sent to player
        /// </summary>
        public class PlayerGameObject
        {
            /// <summary>
            /// Rotation of the object in radians
            /// </summary>
            public float Angle { get; set; } = 0;

            /// <summary>
            /// Position in global space
            /// </summary>
            public Vector2 Position { get; set; } = Vector2.Zero;

            /// <summary>
            /// Velocity in global space
            /// </summary>
            public Vector2 Velocity { get; set; } = Vector2.Zero;

            /// <summary>
            /// Information about displaying game object on client's side
            /// </summary>
            public PlayerGraphicInfo GraphicInfo { get; set; } = new PlayerGraphicInfo();

            /// <summary>
            /// Stores special features for some objects:
            /// - health (0 to 100) for IHasHealth objects
            /// - text for IHasText objects
            /// </summary>
            public Dictionary<string, string> Features = new Dictionary<string, string>();


            /// <summary>
            /// Children of this object. If object has a parent, this object should be passed in this dictionary
            /// </summary>
            public Dictionary<string, PlayerGameObject>? Children { get; set; } = null;

            public static PlayerGameObject FromGameObject(GameObject gameObject, IGraphicLibraryProvider graphicLibrary)
            {
                var result = new PlayerGameObject();

                result.Angle = gameObject.Angle;
                result.Position = gameObject.Position;
                result.Velocity = gameObject.Velocity;

                var spriteRenderer = gameObject.GetComponent<SpriteComponent>();



                result.GraphicInfo.TargetSize = spriteRenderer.CurrentGraphicInfo.TargetSize;
                result.GraphicInfo.GraphicLibraryEntryId = graphicLibrary.NameToId(spriteRenderer.CurrentGraphicInfo.GraphicLibraryEntryName);

                if (spriteRenderer.CurrentGraphicInfo.ObjectAnimationInfo != null)
                    result.GraphicInfo.ObjectAnimationInfo = new PlayerGraphicInfo.PlayerAnimationInfo() { SecondsCompleted = spriteRenderer.CurrentGraphicInfo.ObjectAnimationInfo.SecondsCompleted };


                if (gameObject.HasComponent<IHasHealth>())
                {
                    if (gameObject.parent != null)
                        throw new InvalidOperationException($"Object {gameObject.Name} can not have its IHasHealth displayed, because it has a parent object. Only objects on top of the hierarchy can have their health displayed.");

                    var component = gameObject.GetComponent<IHasHealth>();
                    result.Features["health"] = ((int)(component.Health / (float)component.MaxHealth * 100f)).ToString();
                }
                if (gameObject.HasComponent<IHasText>())
                {
                    var component = gameObject.GetComponent<IHasText>();
                    result.Features["text"] = component.Name;
                }

                if (gameObject.Children.Count > 0)
                {
                    result.Children = new Dictionary<string, PlayerGameObject>();

                    foreach (var child in gameObject.Children)
                    {
                        result.Children.Add(child.Id.ToString(), FromGameObject(child, graphicLibrary));
                    }
                }
                else
                {
                    result.Children = null;
                }


                return result;
            }

        }

        /// <summary>
        /// Represents an information about graphic object that is being sent to player,
        /// where part of the information is represented as id of the object in graphic library
        /// </summary>    
        public class PlayerGraphicInfo
        {
            /// <summary>
            /// Represents an information about graphic object that is being sent to player,
            /// where part of the information is represented as id of the object in graphic library
            /// </summary>    
            public class PlayerAnimationInfo
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
            public PlayerAnimationInfo? ObjectAnimationInfo { get; set; } = null;

            /// <summary>
            /// Target sprite's size in global space. If not null, sprite will be scaled on client to fit this size
            /// </summary>
            public Vector2 TargetSize { get; set; } = Vector2.Zero;

            /// <summary>
            /// A reference to an information about graphics that is stored in GraphicLibrary
            /// </summary>
            public uint GraphicLibraryEntryId { get; set; } = 0;

        }
        
        /// <summary>
        /// Represetns all the information about sound effect that is sent to player
        /// </summary>
        public class PlayerSoundEffect
        {
            public string? AudioClipName { get; set; }

            public Vector2? Position { get; set; } = null;

            public float? Radius { get; set; } = null;

            public ulong Id { get; set; } = 0;
        }

        /// <summary>
        /// Id of the object controlled by player. Can be null
        /// </summary>
        public string? GameObjectsId { get; set; } = null;

        /// <summary>
        /// Player's state
        /// </summary>
        public Player.PlayerState State { get; set; } = Player.PlayerState.NotEntered;

        /// <summary>
        /// If player is in safe zone now? 
        /// </summary>
        public bool IsSafeZone { get; set; } = false;

        /// <summary>
        /// Points avalible to player
        /// </summary>
        public int Points { get; set; } = 0;

        /// <summary>
        /// Information about already invested points per caregory
        /// </summary>
        public Dictionary<PlayerInvestmentState.InvestmentType, int>? AlreadyInvested { get; set; } = null;

        /// <summary>
        /// Player's health
        /// </summary>
        public int Health { get; set; } = 0;

        /// <summary>
        /// Player's max health
        /// </summary>
        public int MaxHealth { get; set; } = 0;

        /// <summary>
        /// Current number of players on server
        /// </summary>
        public int PlayersCount { get; set; } = 0;

        /// <summary>
        /// Represents game objects indexed by their ids
        /// </summary>
        public Dictionary<string, PlayerGameObject> Objects { get; set; } = new Dictionary<string, PlayerGameObject>();

        /// <summary>
        /// Represents sounds that should be played one time on frontend. They are sent to player during several frames to front drops.
        /// They are always ordered by their ids increasing
        /// </summary>
        public List<PlayerSoundEffect> SoundEffectsQueue { get; set; } = new List<PlayerSoundEffect>();

    }
}