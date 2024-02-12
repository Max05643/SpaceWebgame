using Boxed.Mapping;
using GameDesign.GameState;
using GameDesign.Models;
using GameServerDefinitions;
using GameServerImplementation;
using Microsoft.AspNetCore.SignalR;
using WebInterface.ClientModels;
using WebInterface.Hubs;
using WebInterface.Models;

namespace WebInterface.Utils
{
    /// <summary>
    /// Connects SingalR + Controllers and GameServersManager
    /// </summary>
    public class FrontBackCommunication : IPlayersCommunication<PlayerUpdate>
    {

        readonly IPlayersConnectionsStorage playersConnectionsStorage;
        readonly IMapper<PlayerUpdate, ClientGameState> gameStateMapper;
        readonly IMapper<ClientInput, PlayerInput> inputMapper;
        IGameServer<GameStateManager, PlayerInput, PlayerUpdate>? gameServer = null;
        readonly IHubContext<GameHub, IGameClient> hubContext;
        readonly IServiceProvider serviceProvider;
        readonly IChatStorage chatStorage;

        IGameServer<GameStateManager, PlayerInput, PlayerUpdate> GameServer
        {
            get
            {
                if (gameServer == null)
                {
                    gameServer = serviceProvider.GetRequiredService<IGameServer<GameStateManager, PlayerInput, PlayerUpdate>>();
                }
                return gameServer;
            }
        }

        public FrontBackCommunication(IHubContext<GameHub, IGameClient> hubContext, IPlayersConnectionsStorage playersConnectionsStorage, IMapper<PlayerUpdate, ClientGameState> gameStateMapper, IMapper<ClientInput, PlayerInput> inputMapper, IChatStorage chatStorage, IServiceProvider serviceProvider)
        {
            this.hubContext = hubContext;
            this.playersConnectionsStorage = playersConnectionsStorage;
            this.gameStateMapper = gameStateMapper;
            this.inputMapper = inputMapper;
            this.serviceProvider = serviceProvider;
            this.chatStorage = chatStorage;
        }

        /// <summary>
        /// Is player currently in game?
        /// </summary>
        public async Task<bool> IsPlayerInGame(PlayerId playerId)
        {
            return await GameServer.IsPlayerInGame(playerId);
        }

        /// <summary>
        /// Stores specified connection id in storage if player is in specified game.
        /// Stops existing connection with player if exists
        /// </summary>
        public async Task SubscribeToUpdates(PlayerId playerId, string connectionId)
        {
            if (await GameServer.IsPlayerInGame(playerId))
            {
                var prevConnection = await playersConnectionsStorage.SwitchConnection(playerId.ToString(), connectionId);

                if (prevConnection != null)
                {
                    await hubContext.Clients.Client(prevConnection).ReceiveRemovalFromGameNotification();
                }
            }
        }

        /// <summary>
        /// Returns all the messages with ids greater than the argument (may return zero messages)
        /// </summary>
        public async Task<IEnumerable<ChatMessageConainer>> GetChatMessages(PlayerId playerId, long id)
        {
            if (await IsPlayerInGame(playerId))
            {
                return await chatStorage.GetChatMessages(id);
            }
            else
            {
                return new List<ChatMessageConainer>();
            }
        }

        /// <summary>
        /// Adds new message to chat. Returns true if successfully added
        /// </summary>
        public async Task<bool> AddNewChatMessage(ChatMessage message)
        {
            if (!await IsPlayerInGame(message.SenderId))
            {
                return false;
            }
            else if (!UserTextInputValidator.ValidateChatMessage(message.Message))
            {
                return false;
            }
            else
            {
                await chatStorage.AddNewChatMessage(message);
                return true;
            }
        }

        /// <summary>
        /// Adds player to the game. Returns true if player added or already in the game
        /// </summary>
        public Task<bool> JoinGame(PlayerId playerId)
        {
            return GameServer.AddPlayer(playerId);
        }

        /// <summary>
        /// Removes player from the game without notification
        /// </summary>
        public Task LeaveGame(PlayerId playerId)
        {
            return GameServer.LeaveGame(playerId);
        }


        /// <summary>
        /// Sends player's input to the server. Does nothing if operation is impossible
        /// </summary>
        public void SendInput(PlayerId playerId, ClientInput playerInput)
        {
            GameServer.AcceptPlayerInput(inputMapper.Map(playerInput), playerId);
        }


        async Task SendUpdate(PlayerUpdate update, PlayerId playerId)
        {
            var connectionId = await playersConnectionsStorage.GetPlayersConnection(playerId.ToString());

            if (connectionId == null)
                return;

            ClientGameState clientGameState = gameStateMapper.Map(update);

            await hubContext.Clients.Client(connectionId).ReceiveGameState(clientGameState);
        }

        void IPlayersCommunication<PlayerUpdate>.SendUpdate(PlayerUpdate update, PlayerId playerId)
        {
            Task.Run(() => SendUpdate(update, playerId));
        }

        async Task NotifyPlayerThatHeIsKicked(PlayerId playerId)
        {
            var connectionId = await playersConnectionsStorage.GetPlayersConnection(playerId.ToString());

            if (connectionId == null)
                return;

            await hubContext.Clients.Client(connectionId).ReceiveRemovalFromGameNotification();
        }

        void IPlayersCommunication<PlayerUpdate>.NotifyPlayerThatHeIsKicked(PlayerId playerId)
        {
            Task.Run(() => NotifyPlayerThatHeIsKicked(playerId));
        }

        void IPlayersCommunication<PlayerUpdate>.DisposePlayerConnection(PlayerId playerId)
        {
            playersConnectionsStorage.RemovePlayer(playerId.ToString());
        }
    }
}
