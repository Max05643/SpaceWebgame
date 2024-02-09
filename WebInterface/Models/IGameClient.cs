using WebInterface.ClientModels;

namespace WebInterface.Models
{
    /// <summary>
    /// Represents SignalR client
    /// </summary>
    public interface IGameClient
    {
        Task ReceiveGameState(ClientGameState clienGameState);

        Task ReceiveRemovalFromGameNotification();
    }
}
