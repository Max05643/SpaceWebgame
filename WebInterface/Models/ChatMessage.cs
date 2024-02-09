using MessagePack;

namespace WebInterface.Models
{
    /// <summary>
    /// Represents a message in server's chat
    /// </summary>
    [MessagePackObject]
    public class ChatMessage
    {
        [Key("senderNick")]
        public string SenderNick { get; set; } = string.Empty;
        [Key("senderId")]
        public Guid SenderId { get; set; }
        [Key("message")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a chat message sent to frontend
    /// </summary>
    [MessagePackObject]
    public class ChatMessageConainer
    {
        [Key("message")]
        public ChatMessage? Message { get; set; } = null;
        [Key("id")]
        public long Id { get; set; } = -1;
    }
}
