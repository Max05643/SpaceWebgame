using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServerImplementation.ServerEvents;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Channels;

namespace GameServerImplementation
{
    /// <summary>
    /// Game server for any web-based game
    /// </summary>
    /// <typeparam name="PlayerInput">Type that stores input of the player</typeparam>
    /// <typeparam name="PlayerUpdate">Type that stores information that are sent to players on every tick</typeparam>
    public class GameServer<GameState, PlayerInput, PlayerUpdate> : IGameServer<GameState, PlayerInput, PlayerUpdate>, IGameController<PlayerUpdate, PlayerInput> where GameState : IGameState<PlayerInput, PlayerUpdate>
    {
        private readonly IPlayersCommunication<PlayerUpdate> playersCommunication;
        private readonly GameState gameState;
        private readonly GameServerSettings serverSettings;
        private readonly PlayerInputStorage<PlayerInput> playerInputStorage;
        private readonly ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger;

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Channel<ServerEvent<GameState, PlayerInput, PlayerUpdate>> serverEventQueue;

        public GameServer(IGameStateFactory<GameState, PlayerInput, PlayerUpdate> gameStateFactory, IPlayersCommunication<PlayerUpdate> playersCommunication, IPlayerInputProcessor<PlayerInput> playerInputProcessor, GameServerSettings serverSettings, IPlayerInputStorageFactory<PlayerInput> playerInputStorageFactory, ILogger<GameServer<GameState, PlayerInput, PlayerUpdate>> logger)
        {
            cancellationTokenSource = new CancellationTokenSource();

            serverEventQueue = Channel.CreateUnbounded<ServerEvent<GameState, PlayerInput, PlayerUpdate>>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = false });

            this.playersCommunication = playersCommunication;
            this.serverSettings = serverSettings;
            playerInputStorage = playerInputStorageFactory.CreateNewStorage(playerInputProcessor);
            this.logger = logger;

            gameState = gameStateFactory.StartNewGame();

            // Start the game loop
            var mainGameLoopTask = new Task(MainGameLoop, TaskCreationOptions.LongRunning);
            mainGameLoopTask.Start();

            Task.Run(PlayersTimeoutCheckLoop);
        }

        /// <summary>
        /// Checks whether any players are timed out and kicks them
        /// </summary>
        async Task PlayersTimeoutCheckLoop()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(serverSettings.PlayerKickTimeout, cancellationTokenSource.Token);
                    var taskCompletionSource = new TaskCompletionSource();
                    await serverEventQueue.Writer.WriteAsync(new TimeoutCheckEvent<GameState, PlayerInput, PlayerUpdate>(taskCompletionSource));
                    await taskCompletionSource.Task;
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("PlayersTimeoutCheckLoop stopped");
                }
            }

        }


        void MainGameLoop()
        {
            var gameTickStopWatch = new Stopwatch();

            gameTickStopWatch.Start();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {

                    ServerEvent<GameState, PlayerInput, PlayerUpdate>? currentEvent = null;

                    var timeLeftMs = Math.Max(0, serverSettings.TargetTickTimeMs - (int)gameTickStopWatch.ElapsedMilliseconds - 5);

                    if (timeLeftMs <= 10)
                    {
                        Thread.Sleep(timeLeftMs);
                        currentEvent = new GameTickEvent<GameState, PlayerInput, PlayerUpdate>(gameTickStopWatch, this);
                    }
                    else if (serverEventQueue.Reader.TryRead(out var newEvent))
                    {
                        currentEvent = newEvent;
                    }

                    currentEvent?.Perform(gameState, serverSettings, playersCommunication, playerInputStorage);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("The game server has been stopped");
                }
                catch (Exception ex)
                {
                    logger.LogError("Error happened in game loop:\n{ex}", ex);
                }
            }
        }

        bool IGameServer<GameState, PlayerInput, PlayerUpdate>.IsRunning => !cancellationTokenSource.IsCancellationRequested;

        async Task<IEnumerable<PlayerId>> IGameServer<GameState, PlayerInput, PlayerUpdate>.GetCurrentPlayers()
        {
            var taskCompletionSource = new TaskCompletionSource<IEnumerable<PlayerId>>();

            await serverEventQueue.Writer.WriteAsync(new GetPlayersEvent<GameState, PlayerInput, PlayerUpdate>(taskCompletionSource));

            try
            {
                return await taskCompletionSource.Task.WaitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                return Enumerable.Empty<PlayerId>();
            }
        }
        
        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.AcceptPlayerInput(PlayerInput playerInput, PlayerId playerId)
        {
            if (await IsPlayerInGame(playerId))
            {
                playerInputStorage.StoreNewInput(playerInput, playerId);
            }
        }

        async Task<bool> IGameServer<GameState, PlayerInput, PlayerUpdate>.AddPlayer(PlayerId playerId)
        {
            var taskCompletionSource = new TaskCompletionSource();

            await serverEventQueue.Writer.WriteAsync(new AddPlayerEvent<GameState, PlayerInput, PlayerUpdate>(playerId, taskCompletionSource));

            try
            {
                await taskCompletionSource.Task.WaitAsync(cancellationTokenSource.Token);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public async Task<bool> IsPlayerInGame(PlayerId playerId)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            await serverEventQueue.Writer.WriteAsync(new CheckIfInGameEvent<GameState, PlayerInput, PlayerUpdate>(playerId, taskCompletionSource));

            try
            {
                return await taskCompletionSource.Task.WaitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.KickPlayer(PlayerId playerId)
        {
            var taskCompletionSource = new TaskCompletionSource();

            await serverEventQueue.Writer.WriteAsync(new KickPlayerEvent<GameState, PlayerInput, PlayerUpdate>(playerId, taskCompletionSource));

            try
            {
                await taskCompletionSource.Task.WaitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        async Task IGameServer<GameState, PlayerInput, PlayerUpdate>.LeaveGame(PlayerId playerId)
        {
            var taskCompletionSource = new TaskCompletionSource();

            await serverEventQueue.Writer.WriteAsync(new LeaveGameEvent<GameState, PlayerInput, PlayerUpdate>(playerId, taskCompletionSource));

            try
            {
                await taskCompletionSource.Task.WaitAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        PlayerInput IPlayerInputProvider<PlayerInput>.PopPlayerInput(PlayerId playerId)
        {
            return playerInputStorage.PopPlayerInput(playerId);
        }

        void IGameController<PlayerUpdate, PlayerInput>.SendUpdate(PlayerUpdate playerUpdate, PlayerId playerId)
        {
            playersCommunication.SendUpdate(playerUpdate, playerId);
        }

        void IGameServer<GameState, PlayerInput, PlayerUpdate>.StopGame()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
