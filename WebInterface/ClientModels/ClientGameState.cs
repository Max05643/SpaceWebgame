using GameDesign.Models;
using MessagePack;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebInterface.ClientModels
{
    /// <summary>
    /// Represents a game state that is sent to client on every update
    /// </summary>
    [MessagePackObject]
    public class ClientGameState
    {
        [MessagePackObject]
        public class ClientSoundEffect
        {
            [Key("_acn")]
            public string? AudioClipName { get; set; }

            [Key("_ps")]
            public Vector2? Position { get; set; } = null;

            [Key("_rd")]
            public float? Radius { get; set; } = null;

            [Key("id")]
            public ulong Id { get; set; } = 0;
        }

        /// <summary>
        /// Current number of players on server
        /// </summary>
        [Key("_pc")]
        public int PlayersCount { get; set; } = 0;


        /// <summary>
        /// Represents game objects indexed by their ids
        /// </summary>
        [Key("_js")]
        public Dictionary<string, ClientGameObject> Objects { get; set; }

        /// <summary>
        /// Represents sounds that should be played one time on frontend. They are sent to player during several frames to front drops.
        /// They are always ordered by their ids increasing
        /// </summary>
        [Key("_seq")]
        public List<ClientSoundEffect> SoundEffectsQueue { get; set; }

        /// <summary>
        /// Id of the object controlled by player. Can be null
        /// </summary>
        [Key("_gid")]
        public string? GameObjectsId { get; set; } = null;

        /// <summary>
        /// Player's state
        /// </summary>
        [Key("_st")]
        public Player.PlayerState State { get; set; } = Player.PlayerState.NotEntered;

        /// <summary>
        /// If player is in safe zone now? 
        /// </summary>
        [Key("_isz")]
        public bool IsSafeZone { get; set; } = false;

        /// <summary>
        /// Points avalible to player
        /// </summary>
        [Key("_p")]
        public int Points { get; set; } = 0;

        /// <summary>
        /// Information about already invested points per caregory
        /// </summary>
        [Key("_inv")]
        public Dictionary<PlayerInvestmentState.InvestmentType, int>? AlreadyInvested { get; set; } = null;

        /// <summary>
        /// Player's health
        /// </summary>
        [Key("_h")]
        public int Health { get; set; } = 0;

        /// <summary>
        /// Player's max health
        /// </summary>
        [Key("_mh")]
        public int MaxHealth { get; set; } = 0;

        public ClientGameState(Dictionary<string, ClientGameObject> objects, List<ClientSoundEffect> soundEffectsQueue, int playersCount)
        {
            Objects = objects;
            SoundEffectsQueue = soundEffectsQueue;
            PlayersCount = playersCount;
        }
    }
}
