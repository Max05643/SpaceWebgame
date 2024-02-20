using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesign.Models;
using GameDesign.Utils;
using GameServerDefinitions;

namespace GameDesign.GameState
{
    /// <summary>
    /// Combines all information about the state of the game,
    /// controls the flow of the game
    /// and serves for communication with clients
    /// </summary>
    public class GameStateManager : IGameState<PlayerInput, PlayerUpdate>
    {

        internal readonly IGraphicLibraryProvider graphicLibrary;
        internal readonly GameObjectFactory gameObjectFactory;
        internal readonly PhysicsManager physicsManager;
        internal readonly PlayersManager playersManager;
        internal readonly SceneManager sceneManager;
        internal readonly AudioManager audioManager;
        internal readonly GameSettings settings;
        public GameStateManager(GameSettings gameSettings, IGraphicLibraryProvider graphicLibrary)
        {
            this.graphicLibrary = graphicLibrary;

            settings = gameSettings;
            physicsManager = PhysicsManager.CreateWithBoundaries(gameSettings.Gravity, gameSettings.WorldSize, this);
            playersManager = new PlayersManager(this);
            sceneManager = new SceneManager(this);
            gameObjectFactory = new GameObjectFactory(this);
            audioManager = new AudioManager(this);
        }


        TimeSpan timeSinceLastSpawn = TimeSpan.Zero;

        /// <summary>
        /// Runs a game tick with specified delta time
        /// </summary>
        void TickGame(TimeSpan deltaTime, IPlayerInputProvider<PlayerInput> playerInputProvider)
        {

            timeSinceLastSpawn += deltaTime;

            if (timeSinceLastSpawn > settings.ObjectsSpawnSettings.RandomSpawnInterval)
            {
                timeSinceLastSpawn = TimeSpan.Zero;
                this.ProcessObjectsSpawn();
            }

            float deltaTimeSeconds = (float)deltaTime.TotalSeconds;

            audioManager.ClearCurrentFrameClips(deltaTimeSeconds);
            sceneManager.BeforePhysicalCalculation(deltaTimeSeconds, playerInputProvider);
            physicsManager.Update(deltaTimeSeconds);
            sceneManager.AfterPhysicalCalculation(deltaTimeSeconds, playerInputProvider);
        }

        bool IGameState<PlayerInput, PlayerUpdate>.AddPlayer(Guid playerId)
        {
            playersManager.AddPlayerToTheGame(playerId);
            return true;
        }

        void IGameState<PlayerInput, PlayerUpdate>.RemovePlayer(Guid playerId)
        {
            playersManager.RemovePlayerFromTheGame(playerId);
        }

        void IGameState<PlayerInput, PlayerUpdate>.Tick(TimeSpan deltaTime, IGameController<PlayerUpdate, PlayerInput> gameController)
        {

            foreach (var player in playersManager.Players)
            {
                if (player.Value.State != Player.PlayerState.Alive && gameController.PopPlayerInput(player.Key).ReviveRequest)
                {
                    playersManager.RevivePlayer(player.Key);
                }
            }

            TickGame(deltaTime, gameController);

            if (playersManager.Players.Count == 0)
                return;

            var basePlayerUpdate = PlayerUpdateFactory.GetBaseUpdateForAllPlayers(this);

            foreach (var player in playersManager.Players)
            {
                var personalUpdate = PlayerUpdateFactory.GetPersonalUpdateForPlayer(this, player.Value, basePlayerUpdate);
                gameController.SendUpdate(personalUpdate, player.Key);
            }
        }

        bool IGameState<PlayerInput, PlayerUpdate>.IsPlayerInGame(Guid playerId)
        {
            return playersManager.Players.ContainsKey(playerId);
        }

        IEnumerable<Guid> IGameState<PlayerInput, PlayerUpdate>.CurrentPlayers => playersManager.Players.Keys.ToList();
    }
}
