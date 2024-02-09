namespace WebInterface.Utils
{
    /// <summary>
    /// Provides a way to thread-safely store connection ids associated with players and game servers
    /// </summary>
    public interface IPlayersConnectionsStorage
    {
        /// <summary>
        /// Adds new connection for specified player.
        /// Returns null if there was no connection previously. Returns previous connection id if existed for specified player
        /// </summary>
        Task<string?> SwitchConnection(string playerId, string connection);


        /// <summary>
        /// Returns connection associated with playerId or null if there is no one
        /// </summary>
        Task<string?> GetPlayersConnection(string playerId);
        
        /// <summary>
        /// Returns all connections
        /// </summary>
        Task<List<string>> GetAllConnections();

        /// <summary>
        /// Removes the connection of specified player
        /// </summary>
        Task RemovePlayer(string playerId);
    }
}
