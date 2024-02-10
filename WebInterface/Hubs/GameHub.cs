using Microsoft.AspNetCore.SignalR;
using WebInterface.Models;
using WebInterface.Utils;
using WebInterface.ClientModels;

namespace WebInterface.Hubs
{
    /// <summary>
    /// Provides real-time connection between clients and game servers
    /// </summary>
    public class GameHub : Hub<IGameClient>
    {

        readonly IServiceProvider services;
        readonly ILogger<GameHub> logger;

        private FrontBackCommunication? frontBackCommunication;
        FrontBackCommunication FrontBackCommunicationInstance
        {
            get
            {
                if (frontBackCommunication == null)
                {
                    frontBackCommunication = services.GetRequiredService<FrontBackCommunication>();
                }
                return frontBackCommunication;
            }
        }


        /// <summary>
        /// Returns current player's id or null if it is not present or can not be obtained
        /// </summary>
        /// <returns></returns>
        Guid? GetPlayerId()
        {
            var httpContext = Context.GetHttpContext();

            if (httpContext == null)
            {
                return null;
            }
            var playerId = httpContext.GetPlayerId();

            return playerId;
        }

        /// <summary>
        /// Returns current player's nick or null if it is not present or can not be obtained
        /// </summary>
        /// <returns></returns>
        string? GetPlayerNick()
        {
            var httpContext = Context.GetHttpContext();

            if (httpContext == null)
            {
                return null;
            }
            var playerNick = httpContext.GetPlayerNick();

            return playerNick;
        }

        public GameHub(IServiceProvider services, ILogger<GameHub> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        public async Task SubscribeToUpdates()
        {
            var playerId = GetPlayerId();


            if (playerId == null)
                return;

            await FrontBackCommunicationInstance.SubscribeToUpdates(playerId.Value, Context.ConnectionId);
        }

        public void SendInput(ClientInput clientInput)
        {
            var playerId = GetPlayerId();

            if (playerId == null)
                return;

            FrontBackCommunicationInstance.SendInput(playerId.Value, clientInput);
        }

        public async Task<bool> AddChatMessage(string message)
        {

            if (!UserTextInputValidator.ValidateChatMessage(message))
                return false;

            var playerId = GetPlayerId();

            if (playerId == null)
                return false;

            return await FrontBackCommunicationInstance.AddNewChatMessage(new ChatMessage() { Message = message, SenderId = playerId.Value, SenderNick = GetPlayerNick() ?? "Player" });
        }
        public async Task<IEnumerable<ChatMessageConainer>?> GetChatMessages(long id)
        {
            var playerId = GetPlayerId();

            if (playerId == null)
                return null;

            return await FrontBackCommunicationInstance.GetChatMessages(playerId.Value, id);
        }



    }
}
