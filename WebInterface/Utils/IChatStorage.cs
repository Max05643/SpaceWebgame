using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebInterface.Models
{
    /// <summary>
    /// Represents a way to store chat messages that belong to particular server in a thread-safe way
    /// </summary>
    public interface IChatStorage
    {
        /// <summary>
        /// Adds new message to specified server
        /// </summary>
        Task AddNewChatMessage(Guid serverId, ChatMessage chatMessage);

        /// <summary>
        /// Returns all chat messages with ids greater than specified. May return zero messages
        /// </summary>
        Task<ICollection<ChatMessageConainer>> GetChatMessages(Guid serverId, long id);

    }
}
