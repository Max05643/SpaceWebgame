using GameDesign.GameMaps;
using GameDesign.Models;
using GameServer.Models.ClientModels;
using GameServersManager.Models;
using Genbox.VelcroPhysics.Dynamics.Solver;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Xna.Framework;
using WebInterface.Hubs;
using WebInterface.Models;

namespace WebInterface.Utils
{
    /// <summary>
    /// Connects SingalR + Controllers and GameServersManager
    /// </summary>
    public class FrontBackCommunication : IMultiServerClientsCommunication
    {


        readonly IPlayersConnectionsStorage playersConnectionsStorage;
        readonly IServiceProvider serviceProvider;

        IGameServersManager? gameServersManager = null;
        IGameServersManager IGameServersManager
        {
            get
            {
                if (gameServersManager == null)
                    gameServersManager = serviceProvider.GetRequiredService<IGameServersManager>();

                return gameServersManager;
            }
        }

        async Task IMultiServerClientsCommunication.SendClientPersonalInfo(ClientPersonalInfo clientPersonalInfo, Guid serverId, Guid playerId)
        {
            var playersConnection = await playersConnectionsStorage.GetPlayersConnection(serverId.ToString(), playerId.ToString());

            if (playersConnection != null)
                await hubContext.Clients.Client(playersConnection).ReceivePerosnalInfo(clientPersonalInfo);
        }

        async Task IMultiServerClientsCommunication.PlayerIsRemovedFromGame(Guid serverId, Guid playerId)
        {
            var playersConnection = await playersConnectionsStorage.GetPlayersConnection(serverId.ToString(), playerId.ToString());

            if (playersConnection != null)
            {
                await hubContext.Clients.Client(playersConnection).ReceiveRemovalFromGameNotification();
            }
            
            await playersConnectionsStorage.RemovePlayer(serverId.ToString(), playerId.ToString());
        }

        async Task IMultiServerClientsCommunication.ServerIsStopped(Guid serverId)
        {
            await hubContext.Clients.Clients(await playersConnectionsStorage.GetAllConnections(serverId.ToString())).ReceiveRemovalFromGameNotification();
            await playersConnectionsStorage.RemoveGame(serverId.ToString());
        }

        /// <summary>
        /// Stores specified connection id in storage if player is in specified game.
        /// Stops the current connection with player if exists
        /// </summary>
        public async Task SubscribeToUpdates(Guid serverId, Guid playerId, string connectionId)
        {
            if (await CheckIfPlayerInGame(serverId, playerId))
            {
                var prevConnection = await playersConnectionsStorage.SwitchConnection(serverId.ToString(), playerId.ToString(), connectionId);

                if (prevConnection != null)
                {
                    await hubContext.Clients.Client(prevConnection).ReceiveRemovalFromGameNotification();
                }
            }
        }


        IHubContext<GameHub, IGameClient> hubContext;
        public FrontBackCommunication(IHubContext<GameHub, IGameClient> hubContext, IPlayersConnectionsStorage playersConnectionsStorage, IServiceProvider serviceProvider)
        {
            this.hubContext = hubContext;
            this.playersConnectionsStorage = playersConnectionsStorage;
            this.serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Returns all the messages with ids greater than the argument (may return zero messages)
        /// </summary>
        public virtual async Task<ICollection<ChatMessageConainer>?> GetChatMessages(Guid serverId, Guid playerId, long id)
        {

            if (await CheckIfPlayerInGame(serverId, playerId))
                return await IGameServersManager.GetChatMessages(serverId, id);
            else
                return null;
        }


        /// <summary>
        /// Adds new message to chat. Returns true if successfully added
        /// </summary>
        public virtual async Task<bool> AddNewChatMessage(Guid serverId, ChatMessage message)
        {
            if (await CheckIfPlayerInGame(serverId, message.SenderId))
                return await IGameServersManager.AddNewChatMessage(serverId, message);
            else
                return false;
        }




        /// <summary>
        /// Current games' ids
        /// </summary>
        public virtual ICollection<Guid> GetCurrentGames()
        {
            return IGameServersManager.GetCurrentGames();
        }
        /// <summary>
        /// Stops the game and removes it from list of existing games. Returns false if it does not exist
        /// </summary>
        public virtual Task StopTheGame(Guid serverId)
        {
            return IGameServersManager.StopTheGame(serverId);
        }


        /// <summary>
        /// Returns the number of players in a game. Will return null if server is stopped or does not exist
        /// </summary>
        public virtual Task<int?> GetCurrentPlayersCount(Guid serverId)
        {
            return IGameServersManager.GetCurrentPlayersCount(serverId);
        }

        /// <summary>
        /// Starts new game
        /// </summary>
        public virtual Task<Guid?> StartTheGame(GameServerSettings gameServerSettings)
        {
            return IGameServersManager.CreateNewGameServer(gameServerSettings);
        }

        /// <summary>
        /// Adds player to the game. Returns false if operation is impossible
        /// </summary>
        public virtual Task<bool> JoinGame(Guid serverId, Guid playerId, string nick)
        {
            return IGameServersManager.JoinGame(serverId, playerId, nick);
        }
        /// <summary>
        /// Removes player from the game. Returns false if operation is impossible
        /// </summary>
        public virtual Task<bool> RemovePlayer(Guid serverId, Guid playerId)
        {
            return IGameServersManager.RemovePlayer(serverId, playerId);
        }

        /// <summary>
        /// Checks if player is in game. Returns false if game does not exist
        /// </summary>
        public virtual Task<bool> CheckIfPlayerInGame(Guid serverId, Guid playerId)
        {
            return IGameServersManager.CheckIfPlayerInGame(serverId, playerId);
        }
        
        /// <summary>
        /// Checks if game with specified id exists
        /// </summary>
        public virtual Task<bool> CheckIfGameExists(Guid serverId)
        {
            return IGameServersManager.CheckIfGameExists(serverId);
        }


        /// <summary>
        /// Adds game objects from the specified map to game scene
        /// </summary>
        public virtual Task<bool> ApplyMap(Guid serverId, GameMap map)
        {
            return IGameServersManager.ApplyMap(serverId, map);
        }

        /// <summary>
        /// Revives the player. Returns false if operation is impossible
        /// </summary>
        public virtual Task<bool> Revive(Guid serverId, Guid playerId)
        {
            return IGameServersManager.Revive(serverId, playerId);
        }
        /// <summary>
        /// Sends player's input to the server. Does nothing if operation is impossible
        /// </summary>
        public virtual Task SendInput(Guid serverId, Guid playerId, PlayerInput playerInput)
        {
            return IGameServersManager.SendInput(serverId, playerId, playerInput);
        }

    }
}
