namespace WebInterface.Utils
{
    /// <summary>
    /// Provides a way to thread-safely store connection ids associated with players and game servers
    /// </summary>
    public interface IPlayersConnectionsStorage
    {
        /// <summary>
        /// Adds new connection for specified player in a specified game.
        /// Returns null if there was no connection previously. Returns previous connection id if existed for specified game and player
        /// </summary>
        Task<string?> SwitchConnection(string serverId, string playerId, string connection);


        /// <summary>
        /// Returns connection associated with serverId and playerId or null if there is no one
        /// </summary>
        Task<string?> GetPlayersConnection(string serverId, string playerId);
        /// <summary>
        /// Returns all connections associated with specified serverId
        /// </summary>
        Task<List<string>> GetAllConnections(string serverId);

        /// <summary>
        /// Removes specified server by serverId
        /// </summary>
        Task RemoveGame(string serverId);

        /// <summary>
        /// Removes all connection of specifided player from specified server
        /// </summary>
        Task RemovePlayer(string serverId, string playerId);
    }
}
