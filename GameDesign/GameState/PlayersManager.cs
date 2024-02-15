using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.Models;
using GameDesign.Utils;
using GameDesign.Models.Components;

namespace GameDesign.GameState
{
    public class PlayersManager
    {

        readonly GameStateManager gameStateManager;

        public PlayersManager(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }

        /// <summary>
        /// All the players present in game
        /// </summary>
        readonly Dictionary<Guid, Player> players = new Dictionary<Guid, Player>();


        /// <summary>
        /// All the players present in game
        /// </summary>
        public virtual IReadOnlyDictionary<Guid, Player> Players => players;

        /// <summary>
        /// Checks if player is in game
        /// </summary>
        public virtual bool IsPlayerInGame(Guid playerId)
        {
            return players.ContainsKey(playerId);
        }

        /// <summary>
        /// Deletes player's GameObject and changes player's state.
        /// Does nothing if player does not exist or the operation is impossible
        /// </summary>
        public virtual void PlayerDied(Guid playerId)
        {
            if (players.TryGetValue(playerId, out Player? player))
            {
                if (player.PlayersGameObjectId.HasValue)
                    gameStateManager.sceneManager.RemoveGameObject(player.PlayersGameObjectId.Value);
                player.State = Player.PlayerState.Dead;
                player.PlayersGameObjectId = null;
            }
        }

        /// <summary>
        /// Deletes player and his GameObject if necessary.
        /// Does nothing if player is not presented in game
        /// </summary>
        public virtual void RemovePlayerFromTheGame(Guid playerId)
        {
            if (players.TryGetValue(playerId, out Player? player))
            {
                if (player.PlayersGameObjectId.HasValue)
                    gameStateManager.sceneManager.RemoveGameObject(player.PlayersGameObjectId.Value);
                players.Remove(playerId);
            }
        }


        /// <summary>
        /// Adds player to the game. The player's State will be NotEntered.
        /// Does nothing if player exists
        /// </summary>
        public virtual void AddPlayerToTheGame(Guid playerId)
        {
            if (!players.ContainsKey(playerId))
            {
                players.Add(playerId, new Player() { State = Player.PlayerState.NotEntered });
            }
        }

        /// <summary>
        /// Creates player's GameObject at specified position and changes his status to Alive.
        /// Doesn nothing if player is not present or operion is impossible
        /// </summary>
        public virtual void RevivePlayer(Guid playerId)
        {
            if (players.TryGetValue(playerId, out Player? player))
            {
                if (player.State != Player.PlayerState.Alive)
                {
                    player.State = Player.PlayerState.Alive;

                    //Player is spawned in random position

                    Vector2 randomPosition = Vector2.Zero;
                    //Vector2 randomPosition = gameStateManager.Settings.GetRandomPosition();

                    player.PlayersGameObjectId = gameStateManager.gameObjectFactory.CreatePlayerGameObject(randomPosition, playerId).Id;
                }
            }
        }

    }
}
