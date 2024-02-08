using GameServer.Models.ClientModels;

namespace WebInterface.Models
{
    /// <summary>
    /// Represents SignalR client
    /// </summary>
    public interface IGameClient
    {
        Task ReceivePerosnalInfo(ClientPersonalInfo clientPersonalInfo);

        Task ReceiveRemovalFromGameNotification();
    }
}
