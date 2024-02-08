using GameDesign.GameState;
using GameDesign.Models.Components;
using GameDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    /// <summary>
    /// Constructs PlayerUpdates for players from GameStateManager
    /// </summary>
    public static class PlayerUpdateFactory
    {
        /// <summary>
        /// Represents a rect around player within which game objects will be rendered
        /// </summary>
        static readonly Vector2 viewPortHalfSize = new Vector2(80, 80);


        /// <summary>
        /// Checks if specified point is in viewport
        /// </summary>
        static bool CheckIfPointIsInViewPort(Vector2 point, Vector2 gameObjectSize, Vector2 viewPortHalfSize, Vector2 viewPortCenterPosition)
        {
            return
           (point.X + gameObjectSize.X / 2) > (viewPortCenterPosition.X - viewPortHalfSize.X) &&
           (point.X - gameObjectSize.X / 2) < (viewPortCenterPosition.X + viewPortHalfSize.X) &&
           (point.Y + gameObjectSize.Y / 2) > (viewPortCenterPosition.Y - viewPortHalfSize.Y) &&
           (point.Y - gameObjectSize.Y / 2) < (viewPortCenterPosition.Y + viewPortHalfSize.Y);
        }

        /// <summary>
        /// Returns base PlayerUpdate that is used to construct individual PlayerUpdates for players
        /// </summary>
        public static PlayerUpdate GetBaseUpdateForAllPlayers(GameStateManager gameStateManager)
        {
            return new PlayerUpdate()
            {
                Objects = gameStateManager.sceneManager.GameObjects.Where(pair => pair.Value.parent == null).ToDictionary(pair => pair.Key.ToString(), pair => PlayerUpdate.PlayerGameObject.FromGameObject(pair.Value, gameStateManager.graphicLibrary)),
                SoundEffectsQueue = gameStateManager.audioManager.GetCurrentFrameClips().Select(x => new PlayerUpdate.PlayerSoundEffect() { AudioClipName = x.AudioClipName, Id = x.Id, Position = x.Position, Radius = x.Radius }).ToList(),
                PlayersCount = gameStateManager.playersManager.Players.Count
            };
        }


        public static PlayerUpdate GetPersonalUpdateForPlayer(GameStateManager gameStateManager, Player player, PlayerUpdate baseUpdate)
        {
            if (player.State == Player.PlayerState.Alive)
            {
                var playerPos = gameStateManager.sceneManager.GameObjects[player.PlayersGameObjectId!.Value].Position;

                var personalUpdate = new PlayerUpdate()
                {
                    Objects = baseUpdate.Objects.Where(x => CheckIfPointIsInViewPort(x.Value.Position, x.Value.GraphicInfo?.TargetSize ?? Vector2.Zero, viewPortHalfSize, playerPos)).ToDictionary(x => x.Key, x => x.Value),
                    SoundEffectsQueue = baseUpdate.SoundEffectsQueue.Where(x => CheckIfPointIsInViewPort(x.Position ?? playerPos, Vector2.One * (x.Radius ?? 0), viewPortHalfSize, playerPos)).ToList(),
                    PlayersCount = baseUpdate.PlayersCount,
                    State = player.State
                };

                var playerComponent = gameStateManager.sceneManager.GameObjects[player.PlayersGameObjectId!.Value].GetComponent<PlayerControllerComponent>();

                personalUpdate.IsSafeZone = playerComponent.Status == PlayerControllerComponent.PlayerStatus.SafeZone;
                personalUpdate.Points = player.Points;
                personalUpdate.AlreadyInvested = playerComponent.AlreadyInvested.ToDictionary(pair => pair.Key, pair => pair.Value);
                personalUpdate.Health = playerComponent.Health;
                personalUpdate.MaxHealth = playerComponent.MaxHealth;
                personalUpdate.GameObjectsId = player.PlayersGameObjectId?.ToString();


                return personalUpdate;
            }
            else
            {
                var personalClientGameState = new PlayerUpdate()
                {
                    PlayersCount = baseUpdate.PlayersCount,
                    State = player.State
                };


                personalClientGameState.IsSafeZone = false;

                return personalClientGameState;
            }

        }
    }
}
